using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IFFTUtil
{
    #region var
    public struct InitParam
    {
        public ComputeShader ComputeShader; //用来并行计算的shader
        public int Size; //输入变量大小
        public string PingTexName;
        public string PongTexName;
        public string BufferFlyLutTexName;
    }

    public bool Done
    {
        get;
        private set;
    }

    public RenderTexture ResTex
    {
        get;
        private set;
    }
    InitParam m_param;
    int m_stage_count = 0;

    int m_kernel;
    RenderTexture m_ping_tex;
    RenderTexture m_pong_tex;
    #endregion

    #region  method
    public void InitData(in InitParam param)
    {
        m_param = param;
        InitComputeShaderData();
        OnInit();
    }

    void InitComputeShaderData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.IFFTComputeKernelName);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeSizeName, m_param.Size);
    }

    public void Begin()
    {
        Done = false;
    }

    public void Update()
    {
        int i = 0;
        //计算行
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeCalLineName, 1);
        for(; i<m_stage_count; ++i)
        {
            CalStageOutput(i);
        }
        //计算列
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeCalLineName, 0);
        for(i = 0; i<m_stage_count; ++i)
        {
            CalStageOutput(i);
        }
        ResTex = (i-1) % 2 != 0 ? m_ping_tex : m_pong_tex;
        OnDone();
    }

    void CalStageOutput(int stage)
    {
        bool even = stage % 2 != 0;
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageName, (int)stage);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageGroupName, (int)Mathf.Exp(stage+1));
        //设置ping pong坐标翻转操作
        if(!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeInputBufferName, m_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeOutputBufferName, m_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeInputBufferName, m_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeOutputBufferName, m_ping_tex);
        }
        m_param.ComputeShader.Dispatch(m_kernel, CommonData.IFFTComputeThreadSize.x / 8, CommonData.IFFTComputeThreadSize.y / 8, CommonData.IFFTComputeThreadSize.z);
    }

    void OnDone()
    {
        Done = true;
    }

    void InitTex(RenderTexture rt)
    {
        if(null == rt || rt.width != m_param.Size || rt.height != m_param.Size)
        {
            if(null != rt)
            {
                RenderTexture.DestroyImmediate(rt);
            }
            rt = new RenderTexture(m_param.Size, m_param.Size, 32);
        }
    }

    void OnInit()
    {
        m_stage_count = (int)(Mathf.Log(2, m_param.Size));
        InitTex(m_ping_tex);
        InitTex(m_pong_tex);
    }

    public void Leave()
    {
        RenderTexture.DestroyImmediate(m_ping_tex);
        RenderTexture.DestroyImmediate(m_pong_tex);
    }
    #endregion
    
}