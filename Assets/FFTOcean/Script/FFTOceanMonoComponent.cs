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
        InitSpectrum();
    }
    void InitSpectrum()
    {
        m_spectrum_util.InitData(InitParamData.SpectrumParam);
        Debug.Log("[SpectrumUtil] init done");
    }
    public void InitData(InitParam initParam)
    {
        InitParamData = initParam;

        InitSpectrum();
    }
    void Update()
    {
        //1、生成spectrum
        GenSpectrum();
        //2、根据specturm生成高度图
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
    }
    #endregion
}
