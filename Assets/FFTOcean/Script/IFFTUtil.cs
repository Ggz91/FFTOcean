using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFFTUtil
{
    #region var
    [System.Serializable]
    public struct InitParam
    {
        public ComputeShader ComputeShader; //用来并行计算的shader
        public int Size; //输入变量大小
        public RenderTexture BufferFlyLutTex;
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

    void CloneRenderTexture(RenderTexture source, ref RenderTexture dest)
    {
        if(null == dest)
        {
           InitTex(ref dest);
        }
        
        Graphics.CopyTexture(source, dest);
    }

    public void SetInputRenderTexture(RenderTexture rt)
    {
        
    #if _DEBUG_
        CloneRenderTexture(rt,ref m_ping_tex);
    #else
        m_ping_tex = rt;
    #endif
        m_ping_tex.name = "IFFTHeight";
    }
    public void UpdateUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject spectrum_image = canvas?.transform.GetChild(1).gameObject;
        RawImage m_raw_image = spectrum_image?.GetComponent<RawImage>();
        /*float scale = m_param.Size * 1.0f / 100;
        m_raw_image.rectTransform.localScale = new Vector3(scale, scale, scale);*/
        m_raw_image.texture = ResTex;
    }
    void InitComputeShaderData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.IFFTComputeKernelName);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeSizeName, m_param.Size);
        if(null != m_param.BufferFlyLutTex)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTLutTexName, m_param.BufferFlyLutTex);
        }
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
        //重新对齐一下输入
        bool even = (i-1) % 2 != 0;
        //计算列
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeCalLineName, 0);
        for(i = 0; i<m_stage_count; ++i)
        {
            CalStageOutput(i, even);
        }
        if((i-1)%2 != 0)
        {
            ResTex = even ? m_pong_tex : m_ping_tex;
        }
        else
        {
            ResTex = even ? m_ping_tex : m_pong_tex;
        }
        OnDone();
    }

    void CalStageOutput(int stage, bool reverse = false)
    {
        bool even = stage % 2 != 0;
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageName, stage);
        int GroupSize = (int)Mathf.Pow(2, stage+1);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageGroupName, GroupSize);
        //设置ping pong坐标翻转操作
        if(!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeInputBufferName, reverse ? m_pong_tex : m_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeOutputBufferName, reverse ? m_ping_tex : m_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeInputBufferName, reverse ? m_ping_tex : m_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeOutputBufferName, reverse ? m_pong_tex : m_ping_tex);
        }
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Size / 8, m_param.Size / 8, 1);
    }

    void OnDone()
    {
        Done = true;
        UpdateUI();
    }

    void InitTex(ref RenderTexture rt)
    {
        if(null == rt || rt.width != m_param.Size || rt.height != m_param.Size)
        {
            if(null != rt)
            {
                RenderTexture.DestroyImmediate(rt);
            }
            rt = new RenderTexture(m_param.Size, m_param.Size, 32);
            rt.enableRandomWrite = true;
            rt.format = RenderTextureFormat.ARGBFloat;
            rt.wrapMode = TextureWrapMode.Repeat;
            rt.filterMode = FilterMode.Point;
            rt.Create();
        }
    }

    void OnInit()
    {
        m_stage_count = (int)(Mathf.Log(m_param.Size, 2));
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeTotalStageCountName, m_stage_count);

        //InitTex(m_ping_tex);
        InitTex(ref m_pong_tex);
    }

    public void Leave()
    {
        RenderTexture.DestroyImmediate(m_ping_tex);
        RenderTexture.DestroyImmediate(m_pong_tex);
    }
    #endregion
    
}