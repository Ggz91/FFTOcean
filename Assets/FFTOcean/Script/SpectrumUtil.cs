using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumUtil
{
    #region  var
    [System.Serializable]
    public struct InitParam
    {
        public Vector2Int Size;
        public ComputeShader ComputeShader;
        public Vector2 Wind;
        public float Amplitude;
    }
    InitParam m_param;
    int m_kernel;
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
        int[] size = {m_param.Size.x, m_param.Size.y};
        m_param.ComputeShader.SetInts(CommonData.SpectrumComputeSizeName, size);
        Vector2 wind_dir = m_param.Wind.normalized;
        float[] wind_dir_arr = {wind_dir.x, wind_dir.y};
        float wind_speed = m_param.Wind.magnitude;
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

    void Execute()
    {
        UpdateComputeShaderDynamicData();
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Size.x/8, m_param.Size.y/8, 1);
    }
    #endregion
}
