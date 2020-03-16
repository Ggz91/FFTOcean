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
    int[] m_size = new int[2];
    int m_kernel;
    RenderTexture m_lut_rt;
    ComputeBuffer m_buffer;
    #endregion

    #region method
    public void Init(InitParam param)
    {
        Debug.Log("[LutUtil] init param : size : " + param.Size.ToString());
        m_param = param;
        InitComputeShader();
        Debug.Log("[LutUtil] init done");
    }

    void InitComputeShader()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.LutComputeKernelName);
        m_size[0] = m_param.Size;
        m_size[1] = (int)Mathf.Log(m_param.Size, 2);
        m_buffer = new ComputeBuffer(2,4);
        m_buffer.SetData(m_size);
        m_param.ComputeShader.SetBuffer(m_kernel, CommonData.LutComputeSizeName, m_buffer);
        m_lut_rt = new RenderTexture(m_size[0], m_size[1], 32);
        m_lut_rt.enableRandomWrite = true;
        m_lut_rt.Create();
        Debug.Log("[LutUtil] size : (" + m_size[0].ToString() + ", " + m_size[1].ToString() + ")");
        m_param.ComputeShader.SetTexture(m_kernel, CommonData.LutComputeBufferName, m_lut_rt);
    }
    
    public RenderTexture Execute()
    {
        m_param.ComputeShader.Dispatch(m_kernel, m_size[0], m_size[1], 1);
        Debug.Log("[LutUtil] execute done");
        return m_lut_rt;
    }

    public void Leave()
    {
        m_buffer.Release();
        Debug.Log("[LutUtil] leave");
    }
    #endregion
}
