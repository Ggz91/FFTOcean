using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class PreComputeWIndowComponent 
{
    #region  var
    LutUtil m_lut_util = new LutUtil();
    #endregion 

    #region method
    void FillLutInitParam(out LutUtil.InitParam param)
    {
        param = new LutUtil.InitParam();
        param.Size = Size;
        param.ComputeShader = LutComputeShader;
        Debug.Log("[GenLutTex] fill init param");
    }
    #endregion

    #region ui
    [InfoBox(@"离线计算的一些表现: 
    1. 蝶形lut")]
    [BoxGroup("蝶形lut")]
    [MinValue(1)]
    public int Size = 128;

    [BoxGroup("蝶形lut")]
    public ComputeShader LutComputeShader;

    [Button("生成蝶形Lut")]
    public void GenLut()
    {
        Debug.Log("=============[GenLut] Enter==============");
        LutUtil.InitParam param;
        FillLutInitParam(out param);
        m_lut_util.Init(param);
        RenderTexture rt = m_lut_util.Execute();
        CommonUtil.SaveRenderTextureToPNG(rt, UICommonData.IFFTOceanLutPNGPath);
        RenderTexture.active = null;
        CommonUtil.SaveAsset(rt, UICommonData.IFFTOceanLutTexPath);
        m_lut_util.Leave();
        Debug.Log("=============[GenLut] Leave==============");
    }

    public void Enter()
    {
        Debug.Log("================[GenLut] Enter================");
        InitDefaultValues();
    }
    void InitDefaultComputeShader()
    {
        string default_compute_shader = @"Assets/FFTOcean/Shader/LutComputeShader.compute";
        LutComputeShader = AssetDatabase.LoadAssetAtPath(default_compute_shader, typeof(ComputeShader)) as ComputeShader;
    }
    static void LoadSavedLutTex()
    {
        FileStream stream = File.Open(UICommonData.IFFTOceanLutPNGPath, FileMode.Open, FileAccess.Read);
        stream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        stream.Close();
        stream.Dispose();
        stream = null;
        Texture2D tex = new Texture2D(128, 8, TextureFormat.RGBAFloat, false);
        tex.LoadImage(bytes);
        tex.Apply();
        //debug
        RenderTexture rt = new RenderTexture(tex.width, tex.height, 32, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.Create();
        RenderTexture cur = RenderTexture.active;
        RenderTexture.active = null;
        Graphics.Blit(tex, rt);
        RenderTexture.active = cur;
        if(Directory.Exists(UICommonData.IFFTOceanLutTexPath))
        {
            Directory.Delete(UICommonData.IFFTOceanLutTexPath);
        }
        CommonUtil.SaveAsset(rt, UICommonData.IFFTOceanLutTexPath);
        Debug.Log("[LoadSavedLutTex] done");
    }
    
    public static void PreHandle()
    {
        LoadSavedLutTex();
    }

    void InitDefaultValues()
    {
        //默认的compute shader
        InitDefaultComputeShader();
        LoadSavedLutTex();
    }
    #endregion
}
