using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectrumUtil
{
    #region  var
    [System.Serializable]
    public struct InitParam
    {
        public int Resolution;
        public ComputeShader ComputeShader;
        public Vector2 Wind;
        public float Amplitude;
        public float Size;
    }
    public RenderTexture ResTex
    {
        get
        {
            return m_spectrum_tex;
        }
    }
    InitParam m_param;
    int m_kernel;
    RenderTexture m_spectrum_tex;
    ComputeBuffer m_wind_dir_buff = null;
    ComputeBuffer m_rand_pair_buff = null;
    RawImage m_raw_image;
    #endregion

    #region  method
    void InitUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject spectrum_image = canvas?.transform.GetChild(0).gameObject;
        m_raw_image = spectrum_image?.GetComponent<RawImage>();
        m_raw_image.texture = m_spectrum_tex;
    }
    public void UpdateUI()
    {
        m_raw_image.texture = m_spectrum_tex;
    }
    public void InitData(InitParam param)
    {
        m_param = param;
        InitComputeShaderStaticData();
        InitUI();
    }

    void InitComputeShaderStaticData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.SpectrumComputeKernelName);
        m_param.ComputeShader.SetInt(CommonData.SpectrumComputeResoName, m_param.Resolution);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeSizeName, m_param.Size);
        Vector2 wind_dir = m_param.Wind.normalized;
        float[] wind_dir_arr = {wind_dir.x, wind_dir.y};
        float wind_speed = m_param.Wind.magnitude;
        m_spectrum_tex = new RenderTexture(m_param.Resolution, m_param.Resolution,32);
        m_spectrum_tex.format = RenderTextureFormat.ARGBFloat;
        m_spectrum_tex.enableRandomWrite = true;
        m_spectrum_tex.Create();
        m_spectrum_tex.name = "SpectrumTex";
        m_param.ComputeShader.SetTexture(m_kernel, CommonData.SpectrumComputeOutputTexName, m_spectrum_tex);
        if(null == m_wind_dir_buff)
        {
            m_wind_dir_buff = new ComputeBuffer(2,4);
        }
        m_wind_dir_buff.SetData(wind_dir_arr);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.SpectrumComputeWindDirName, m_wind_dir_buff);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeWindSpeedName, wind_speed);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeAmplitudeName, m_param.Amplitude);

        if(null == m_rand_pair_buff)
        {
            m_rand_pair_buff = new ComputeBuffer(2, 4);
        }
    }
    
    void UpdateComputeShaderDynamicData()
    {
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeTimeName, Time.time * 1000);
        Vector2 rand_pair = MathUtil.CalGaussianRandomVariablePair();
        //Debug.Log("[SpectrumUtil] rand pair : " + rand_pair.ToString());
        float[] rand_pair_arr =  {rand_pair.x, rand_pair.y};
        m_rand_pair_buff.SetData(rand_pair_arr);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.SpectrumComputeRandPairName, m_rand_pair_buff);
    }

    public void Execute()
    {
        UpdateComputeShaderDynamicData();
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Resolution/8, m_param.Resolution/8, 1);
    }

    public void Leave()
    {
        RenderTexture.DestroyImmediate(m_spectrum_tex);
        m_rand_pair_buff.Release();
        m_rand_pair_buff = null;
        m_wind_dir_buff.Release();
        m_wind_dir_buff = null;
    }
    #endregion
}
