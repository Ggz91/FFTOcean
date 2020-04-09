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
        public float Length; //patch尺寸大小
        public RenderTexture BufferFlyLutTex;
    }

    public bool Done
    {
        get;
        private set;
    }

    public RenderTexture ResHeightTex
    {
        get;
        private set;
    }

    public RenderTexture ResDisplaceTex
    {
        get;
        private set;
    }

    public RenderTexture ResNormalTex
    {
        get;
        private set;
    }

    InitParam m_param;
    int m_stage_count = 0;

    int m_kernel;
    int m_post_kernel;
    RenderTexture m_height_ping_tex;
    RenderTexture m_height_pong_tex;
    RenderTexture m_displace_ping_tex;
    RenderTexture m_displace_pong_tex;
    RenderTexture m_normal_ping_tex;
    RenderTexture m_normal_pong_tex;
    RenderTexture m_debug_tex;

    RenderTexture m_jacob_ping_tex;
    RenderTexture m_jacob_pong_tex;
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
        if (null == dest)
        {
            InitTex(ref dest);
        }

        Graphics.CopyTexture(source, dest);
    }

    public void SetInputRenderTexture(RenderTexture rt)
    {
        CloneRenderTexture(rt, ref m_height_ping_tex);
        CloneRenderTexture(rt, ref m_displace_ping_tex);
        CloneRenderTexture(rt, ref m_normal_ping_tex);
        CloneRenderTexture(rt, ref m_jacob_ping_tex);
        m_height_ping_tex.name = "IFFTHeight";
        m_displace_ping_tex.name = "IFFTDisplace";
        m_normal_ping_tex.name = "IFFTNormal";
        m_jacob_ping_tex.name = "IFFTJacob";
    }
    public void UpdateUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject spectrum_obj = canvas?.transform.GetChild(1).gameObject;
        RawImage spectrum_image = spectrum_obj?.GetComponent<RawImage>();
        /*float scale = m_param.Size * 1.0f / 100;
        m_raw_image.rectTransform.localScale = new Vector3(scale, scale, scale);*/
        spectrum_image.texture = ResHeightTex;

        GameObject displace_obj = canvas?.transform.GetChild(2).gameObject;
        RawImage displace_image = displace_obj?.GetComponent<RawImage>();
        /*#if !_DEBUG_
                displace_image.texture = ResDisplaceTex;
        #else
                displace_image.texture = m_debug_tex;
        #endif*/
        displace_image.texture = ResDisplaceTex;
    }

    void InitComputeShaderData()
    {
        m_kernel = m_param.ComputeShader.FindKernel(CommonData.IFFTComputeKernelName);
        m_post_kernel = m_param.ComputeShader.FindKernel(CommonData.IFFTPostKernelName);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeSizeName, m_param.Size);
        m_param.ComputeShader.SetFloat(CommonData.IFFTComputeLenghtName, m_param.Length);
        if (null != m_param.BufferFlyLutTex)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTLutTexName, m_param.BufferFlyLutTex);
        }
    }
    
    void NormalPost()
    {
        //梯度转normal
        m_param.ComputeShader.SetTexture(m_post_kernel, CommonData.IFFTComputeNormalInputBufferName, ResNormalTex);
        m_param.ComputeShader.SetTexture(m_post_kernel, CommonData.IFFTComputeNormalOutputBufferName, (ResNormalTex == m_normal_ping_tex) ? m_normal_pong_tex : m_normal_ping_tex);
        m_param.ComputeShader.Dispatch(m_post_kernel, m_param.Size / 8, m_param.Size / 8, 1);
        ResNormalTex = (ResNormalTex == m_normal_ping_tex) ? m_normal_pong_tex : m_normal_ping_tex;
    }
    void JacobPost()
    {
        //尖浪使用jacoba行列式进行判断
    }
    void PostHandle()
    {
        //后处理的一些操作
        NormalPost();
        JacobPost();
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
        for (; i < m_stage_count; ++i)
        {
            CalStageOutput(i);
        }
        //重新对齐一下输入
        bool even = (i - 1) % 2 != 0;
        //计算列
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeCalLineName, 0);
        for (i = 0; i < m_stage_count; ++i)
        {
            CalStageOutput(i, even);
        }

        if ((i - 1) % 2 != 0)
        {
            ResHeightTex = even ? m_height_pong_tex : m_height_ping_tex;
            ResDisplaceTex = even ? m_displace_pong_tex : m_displace_ping_tex;
            ResNormalTex = even ? m_normal_pong_tex : m_normal_ping_tex;
        }
        else
        {
            ResHeightTex = even ? m_height_ping_tex : m_height_pong_tex;
            ResDisplaceTex = even ? m_displace_ping_tex : m_displace_pong_tex;
            ResNormalTex = even ? m_normal_ping_tex : m_normal_pong_tex;
        }
        PostHandle();
        OnDone();
    }
    void CalDisplace(bool even, bool reverse)
    {
        //设置ping pong坐标翻转操作
        if (!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeDisplaceInputBufferName, reverse ? m_displace_pong_tex : m_displace_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeDisplaceOutputBufferName, reverse ? m_displace_ping_tex : m_displace_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeDisplaceInputBufferName, reverse ? m_displace_ping_tex : m_displace_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeDisplaceOutputBufferName, reverse ? m_displace_pong_tex : m_displace_ping_tex);
        }
    }
    void CalJacob(bool even, bool reverse)
    {
        //设置ping pong坐标翻转操作
        if (!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeJacobInputBufferName, reverse ? m_jacob_pong_tex : m_jacob_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeJacobOutputBufferName, reverse ? m_jacob_ping_tex : m_jacob_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeJacobInputBufferName, reverse ? m_jacob_ping_tex : m_jacob_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeJacobOutputBufferName, reverse ? m_jacob_pong_tex : m_jacob_ping_tex);
        }
    }
    void CalNormal(bool even, bool reverse)
    {
        //设置ping pong坐标翻转操作
        if (!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeNormalInputBufferName, reverse ? m_normal_pong_tex : m_normal_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeNormalOutputBufferName, reverse ? m_normal_ping_tex : m_normal_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeNormalInputBufferName, reverse ? m_normal_ping_tex : m_normal_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeNormalOutputBufferName, reverse ? m_normal_pong_tex : m_normal_ping_tex);
        }
    }

    void CalHeight(bool even, bool reverse = false)
    {
        //设置ping pong坐标翻转操作
        if (!even)
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeHeightInputBufferName, reverse ? m_height_pong_tex : m_height_ping_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeHeightOutputBufferName, reverse ? m_height_ping_tex : m_height_pong_tex);
        }
        else
        {
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeHeightInputBufferName, reverse ? m_height_ping_tex : m_height_pong_tex);
            m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTComputeHeightOutputBufferName, reverse ? m_height_pong_tex : m_height_ping_tex);
        }
    }
    void CalStageOutput(int stage, bool reverse = false)
    {
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageName, stage);
        int GroupSize = (int)Mathf.Pow(2, stage + 1);
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeStageGroupName, GroupSize);
        bool even = stage % 2 != 0;
        
        CalHeight(even, reverse);
        CalDisplace(even, reverse);
        CalNormal(even, reverse);
        //CalJacob(even, reverse);
        m_param.ComputeShader.Dispatch(m_kernel, m_param.Size / 8, m_param.Size / 8, 1);
    }

    void OnDone()
    {
        Done = true;
        UpdateUI();
    }

    void InitTex(ref RenderTexture rt)
    {
        if (null == rt || rt.width != m_param.Size || rt.height != m_param.Size)
        {
            if (null != rt)
            {
                RenderTexture.DestroyImmediate(rt);
            }
            rt = new RenderTexture(m_param.Size, m_param.Size, 32);
            rt.enableRandomWrite = true;
            rt.format = RenderTextureFormat.ARGBFloat;
            rt.wrapMode = TextureWrapMode.Repeat;
            rt.filterMode = FilterMode.Trilinear;
            rt.Create();
        }
    }

    void OnInit()
    {
        m_stage_count = (int)(Mathf.Log(m_param.Size, 2));
        m_param.ComputeShader.SetInt(CommonData.IFFTComputeTotalStageCountName, m_stage_count);

        //InitTex(m_ping_tex);
        InitTex(ref m_height_pong_tex);
        InitTex(ref m_displace_pong_tex);
        InitTex(ref m_normal_pong_tex);
        InitTex(ref m_jacob_pong_tex);
        /*#if _DEBUG_
                InitTex(ref m_debug_tex);
                m_param.ComputeShader.SetTexture(m_kernel, CommonData.IFFTDebugTexName, m_debug_tex);
        #endif*/
    }

    public void Leave()
    {
        RenderTexture.DestroyImmediate(m_height_ping_tex);
        RenderTexture.DestroyImmediate(m_height_pong_tex);
    }
    #endregion

}