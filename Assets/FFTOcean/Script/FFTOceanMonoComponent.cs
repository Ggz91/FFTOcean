using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTOceanMonoComponent : MonoBehaviour
{
    #region var
    [System.Serializable]
    public class InitParam : ScriptableObject
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
    void InitConfig()
    {
        InitParam param = CommonUtil.LoadAsset(UICommonData.IFFTOceanInitConfig, typeof(InitParam)) as InitParam;
        InitData(param);
    }

    void Start()
    {
        InitConfig();
    }

    void InitSpectrum()
    {
        m_spectrum_util.InitData(InitParamData.SpectrumParam);
        Debug.Log("[SpectrumUtil] init done");
    }

    void InitIFFTUtil()
    {
        m_ifft_util.InitData(InitParamData.IFFTParam);
        Debug.Log("[IFFTUtil] init done");
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
        UpdateMatHeightMap();
    }

    void UpdateMatHeightMap()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        CommonUtil.SaveRenderTextureToPNG(m_ifft_util.ResTex, UICommonData.IFFTOceanHeightMapPath);
        mat?.SetTexture(Shader.PropertyToID(CommonData.OCeanMatHeightTexName), m_ifft_util.ResTex);
    }

    void IFFTUpdate()
    {
        //更新高度图
        m_ifft_util.SetInputRenderTexture(m_spectrum_util.ResTex);
        m_ifft_util.Update();
    }
    
    void GenSpectrum()
    {
        m_spectrum_util.Execute();
        //CommonUtil.SaveRenderTextureToPNG(m_spectrum_util.ResTex, UICommonData.IFFTOceanHeightMapPath);

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
