﻿using System.Collections;
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
    RenderTexture m_debug_tex;
    #endregion

    #region  method
    void InitUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject spectrum_image = canvas?.transform.GetChild(0).gameObject;
        m_raw_image = spectrum_image?.GetComponent<RawImage>();
        /*float scale = m_param.Size / 100;
        m_raw_image.rectTransform.localScale = new Vector3(scale, scale, scale);*/
        m_raw_image.texture = m_spectrum_tex;

        GameObject debug_image = canvas?.transform.GetChild(3).gameObject;
        RawImage debug_tex = debug_image?.GetComponent<RawImage>();
        debug_tex.texture = m_debug_tex;
    }
    public void InitData(InitParam param)
    {
        m_param = param;
        InitComputeShaderStaticData();
        InitUI();
    }
    void InitTex(ref RenderTexture rt)
    {
        rt = new RenderTexture(m_param.Resolution, m_param.Resolution, 32);
        rt.format = RenderTextureFormat.ARGBFloat;
        rt.enableRandomWrite = true;
        rt.wrapMode = TextureWrapMode.Repeat;
        rt.filterMode = FilterMode.Trilinear;
        rt.Create();
    }
    void InitComputeShaderStaticData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.SpectrumComputeKernelName);
        m_param.ComputeShader.SetInt(CommonData.SpectrumComputeResoName, m_param.Resolution);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeSizeName, m_param.Size);
        Vector2 wind_dir = m_param.Wind.normalized;
        float[] wind_dir_arr = { wind_dir.x, wind_dir.y };
        float wind_speed = m_param.Wind.magnitude;
        InitTex(ref m_spectrum_tex);
        m_spectrum_tex.name = "SpectrumTex";
        m_param.ComputeShader.SetTexture(m_kernel, CommonData.SpectrumComputeOutputTexName, m_spectrum_tex);
        InitTex(ref m_debug_tex);
        m_debug_tex.name = "DebugTex";
        m_param.ComputeShader.SetTexture(m_kernel, CommonData.SpecturmDebugTexName, m_debug_tex);
        if (null == m_wind_dir_buff)
        {
            m_wind_dir_buff = new ComputeBuffer(2, 4);
        }
        m_wind_dir_buff.SetData(wind_dir_arr);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.SpectrumComputeWindDirName, m_wind_dir_buff);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeWindSpeedName, wind_speed);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeAmplitudeName, m_param.Amplitude);

        if (null == m_rand_pair_buff)
        {
            m_rand_pair_buff = new ComputeBuffer(4, 4);
        }

        Vector2 rand_pair0 = MathUtil.CalGaussianRandomVariablePair();
        Vector2 rand_pair1 = MathUtil.CalGaussianRandomVariablePair();
        //Debug.Log("[SpectrumUtil] rand pair : " + rand_pair.ToString());
        float[] rand_pair_arr = { rand_pair0.x, rand_pair0.y, rand_pair1.x, rand_pair1.y };
        m_rand_pair_buff.SetData(rand_pair_arr);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.SpectrumComputeRandPairName, m_rand_pair_buff);
    }

    void UpdateComputeShaderDynamicData()
    {
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeTimeName, Time.time);
        /*Vector2 rand_pair = MathUtil.CalGaussianRandomVariablePair();
        //Debug.Log("[SpectrumUtil] rand pair : " + rand_pair.ToString());
        float[] rand_pair_arr =  {rand_pair.x, rand_pair.y};
        m_rand_pair_buff.SetData(rand_pair_arr);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.SpectrumComputeRandPairName, m_rand_pair_buff);*/
    }

    public void Execute()
    {
        UpdateComputeShaderDynamicData();
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Resolution / 8, m_param.Resolution / 8, 1);
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
