using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData
{
    static public string IFFTComputeKernelName = "CSMain";
    static public string IFFTComputeInputBufferName = "InputTex";
    static public string IFFTComputeOutputBufferName = "OutputTex";
    static public Vector3Int IFFTComputeThreadSize = new Vector3Int(1024, 1, 1);
    static public Vector2Int TexSize = new Vector2Int(1024, 1024);
    static public string IFFTComputeStageName = "Stage";
    static public string IFFTComputeStageGroupName = "GroupSize";
    static public string LutComputeKernelName = "CSMain";
    static public string LutComputeBufferName = "LutTex";
    static public string LutComputeSizeName = "Size";
    static public string SpectrumComputeKernelName = "CSMain";
    static public string SpectrumComputeSizeName = "Size";
    static public string SpectrumComputeTimeName = "Time";
    static public string SpectrumComputeWindDirName = "WindDir";
    static public string SpectrumComputeWindSpeedName = "WindSpeed";
    static public string SpectrumComputeRandPairName = "RandomPair";
    static public string SpectrumComputeAmplitudeName = "Amplitude";
    static public string SpectrumComputeOutputTexName = "SpectrumTex";
    static public string IFFTComputeSizeName = "Size";
    static public string IFFTComputeCalLineName = "ComputeLine";
    static public string IFFTLutTexName = "LutTex";
    static public string OCeanMatHeightTexName = "_OceanHeightMap";
}