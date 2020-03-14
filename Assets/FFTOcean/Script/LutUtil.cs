using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LutUtil 
{
    #region  var
    public struct InitParam
    {
        public int Size;
        public ComputeShader ComputeShader;
    }

    InitParam m_param;
    int[] m_size;
    int m_kernel;
    RenderTexture m_lut_rt;
    #endregion

    #region method
    public void Init(InitParam param)
    {
        m_param = param;
        InitComputeShader();
    }

    void InitComputeShader()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.LutComputeKernelName);
        m_size = new int[2];
        m_size[0] = m_param.Size;
        m_size[1] = (int)Mathf.Log(2, m_param.Size);
        m_param.ComputeShader.SetInts(CommonData.LutComputeSizeName, m_size);
        m_lut_rt = RenderTexture.GetTemporary(m_size[0], m_size[1], 32);
        int lut_id = Shader.PropertyToID(CommonData.LutComputeSizeName);
        m_param.ComputeShader.SetTexture(m_kernel, lut_id, m_lut_rt);
    }
    
    public RenderTexture Execute()
    {
        m_param.ComputeShader.Dispatch(m_kernel, m_size[0], m_size[1], 1);
        return m_lut_rt;
    }

    public void Leave()
    {
        RenderTexture.ReleaseTemporary(m_lut_rt);
    }
    #endregion
}
