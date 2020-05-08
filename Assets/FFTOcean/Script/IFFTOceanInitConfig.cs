using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IFFTOceanInitConfig : ScriptableObject
{
    // Start is called before the first frame update
    [SerializeField]
    public SpectrumUtil.InitParam SpectrumParam;
    
    [SerializeField]
    public IFFTUtil.InitParam IFFTParam;
    
    [SerializeField]
    public FFTOceanMonoComponent.MatParam OceanMatParam;
}
