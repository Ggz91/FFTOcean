﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumUtil
{
    #region  var
    [System.Serializable]
    public struct InitParam
    {
        public int Size;
        public ComputeShader ComputeShader;
        public Vector2 Wind;
        public float Amplitude;
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
    #endregion

    #region  method
    public void InitData(InitParam param)
    {
        m_param = param;
        InitComputeShaderStaticData();
    }

    void InitComputeShaderStaticData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.SpectrumComputeKernelName);
        m_param.ComputeShader.SetInt(CommonData.SpectrumComputeSizeName, m_param.Size);
        Vector2 wind_dir = m_param.Wind.normalized;
        float[] wind_dir_arr = {wind_dir.x, wind_dir.y};
        float wind_speed = m_param.Wind.magnitude;
        m_spectrum_tex = new RenderTexture(m_param.Size, m_param.Size,32);
        m_spectrum_tex.enableRandomWrite = true;
        m_spectrum_tex.Create();
        m_param.ComputeShader.SetTexture(m_kernel, CommonData.SpectrumComputeOutputTexName, m_spectrum_tex);
        m_param.ComputeShader.SetFloats(CommonData.SpectrumComputeWindDirName, wind_dir_arr);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeWindSpeedName, wind_speed);
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeAmplitudeName, m_param.Amplitude);
    }
    
    void UpdateComputeShaderDynamicData()
    {
        m_param.ComputeShader.SetFloat(CommonData.SpectrumComputeTimeName, Time.time * 1000);
        Vector2 rand_pair = MathUtil.CalGaussianRandomVariablePair();
        float[] rand_pair_arr = {rand_pair.x, rand_pair.y};
        m_param.ComputeShader.SetFloats(CommonData.SpectrumComputeRandPairName, rand_pair_arr);
    }

    public void Execute()
    {
        UpdateComputeShaderDynamicData();
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Size/8, m_param.Size/8, 1);
    }

    public void Leave()
    {
        RenderTexture.DestroyImmediate(m_spectrum_tex);
    }
    #endregion
}