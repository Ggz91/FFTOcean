using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class PreComputeWIndowComponent 
{
    #region  var
    LutUtil m_lut_util = new LutUtil();
    #endregion 

    #region method
    void FillLutInitParam(out LutUtil.InitParam param)
    {
        param = new LutUtil.InitParam();
        param.Size = Size.x;
        param.ComputeShader = LutComputeShader;
        Debug.Log("[GenLutTex] fill init param");
    }
    #endregion

    #region ui
    [InfoBox(@"离线计算的一些表现: 
    1. 蝶形lut")]
    [BoxGroup("蝶形lut")]
    [MinValue(1)]
    public Vector2Int Size = new Vector2Int(1024, 1024);

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
        CommonUtil.SaveRenderTexture(rt);
        m_lut_util.Leave();
        Debug.Log("=============[GenLut] Leave==============");
    }
    #endregion
}
