using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class IFFTUtil
{
    #region var
    public struct InitParam
    {
        public int RTid;    //用来更新到最好的RT id
        public ComputeShader ComputeShader; //用来并行计算的shader
        public int Size; //输入变量大小
        public List<int> Input;
        public string PingTexName;
        public string PongTexName;
    }
    public bool Done
    {
        get;
        private set;
    }
    InitParam m_param;
    int m_stage_count = 0;

    int m_kernel;
    #endregion

    #region  method
    public void Init(in InitParam param)
    {
        m_param = param;
        InitComputeShaderData();
        OnInit();
    }

    void InitComputeShaderData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.ComputeKernelName);
    }

    public void Begin()
    {
        Done = false;
    }

    public void Update()
    {
        for(int i=0; i<m_stage_count; ++i)
        {
            CalStageOutput((i % 2) == 0);
        }
        OnDone();
    }

    void CalStageOutput(bool even)
    {
        //设置ping pong坐标翻转操作
        if(even)
        {
            m_param.ComputeShader.SetTextureFromGlobal(m_kernel, CommonData.ComputeInputBufferName, m_param.PingTexName);
            m_param.ComputeShader.SetTextureFromGlobal(m_kernel, CommonData.ComputeInputBufferName, m_param.PongTexName);
        }
        else
        {
            m_param.ComputeShader.SetTextureFromGlobal(m_kernel, CommonData.ComputeInputBufferName, m_param.PongTexName);
            m_param.ComputeShader.SetTextureFromGlobal(m_kernel, CommonData.ComputeInputBufferName, m_param.PingTexName);
        }
        m_param.ComputeShader.Dispatch(m_kernel, CommonData.ComputeThreadSize.x, CommonData.ComputeThreadSize.y, CommonData.ComputeThreadSize.z);
    }

    void OnDone()
    {
        Done = true;
    }

    void OnInit()
    {
        m_stage_count = (int)(Mathf.Log(2, m_param.Size));
    }
    #endregion
    
}