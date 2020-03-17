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
    };

    [SerializeField]
    public InitParam InitParamData;
    SpectrumUtil m_spectrum_util = new SpectrumUtil();
    #endregion

    #region  method
    void Start()
    {
    }
    void InitSpectrum()
    {
        m_spectrum_util.InitData(InitParamData.SpectrumParam);
    }
    public void InitData(InitParam initParam)
    {
        InitParamData = initParam;

        InitSpectrum();
    }

    void Update()
    {
        //1、生成spectrum
        
        //2、根据specturm生成高度图

        
    }

    void GenSpectrum()
    {

    }
    #endregion
}
