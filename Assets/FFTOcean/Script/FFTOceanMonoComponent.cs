using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTOceanMonoComponent : MonoBehaviour
{
    #region var
    [System.Serializable]
    public struct InitParam
    {
        [SerializeField]
        public SpectrumUtil.InitParam SpectrumParam;
        [SerializeField]
        public IFFTUtil.InitParam IFFTParam;
    };

    [SerializeField]
    public InitParam InitParamData;
    SpectrumUtil m_spectrum_util = new SpectrumUtil();
    IFFTUtil m_ifft_util = new IFFTUtil();
    #endregion

    #region  method
    void Start()
    {
        InitSpectrum();
    }
    void InitSpectrum()
    {
        m_spectrum_util.InitData(InitParamData.SpectrumParam);
        Debug.Log("[SpectrumUtil] init done");
    }
    void InitIFFTUtil()
    {
        m_ifft_util.InitData(InitParamData.IFFTParam);
    }
    public void InitData(InitParam initParam)
    {
        InitParamData = initParam;

        InitSpectrum();
        InitIFFTUtil();
    }
    void Update()
    {
        //1、生成spectrum
        GenSpectrum();
        
        //2、根据specturm生成高度图
        IFFTUpdate();

        //3、更新高度图到材质
        UpdateMatHeightMap(m_ifft_util.ResTex);
    }

    void UpdateMatHeightMap(RenderTexture rt)
    {
        Material mat = GetComponent<Material>();
        mat?.SetTexture(Shader.PropertyToID(CommonData.OCeanMatHeightTexName), rt);
    }
    void IFFTUpdate()
    {
        m_ifft_util.Update();
    }
    void GenSpectrum()
    {
        m_spectrum_util.Execute();
        //Debug.Log("[SpectrumUtil] execute done. time : " + Time.time.ToString());
    }

    void OnDestory()
    {
        m_spectrum_util.Leave();
        m_spectrum_util = null;
        m_ifft_util.Leave();
        m_ifft_util = null;
    }
    #endregion
}
