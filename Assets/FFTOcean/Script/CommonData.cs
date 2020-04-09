using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData
{
    static public string IFFTComputeKernelName = "CSMain";
    static public string IFFTPostKernelName = "PostKernel";
    static public string IFFTComputeHeightInputBufferName = "HeightInputTex";
    static public string IFFTComputeHeightOutputBufferName = "HeightOutputTex";
    static public string IFFTComputeDisplaceInputBufferName = "DisplaceInputTex";
    static public string IFFTComputeDisplaceOutputBufferName = "DisplaceOutputTex";
    static public string IFFTComputeJacobInputBufferName = "JacobInputTex";
    static public string IFFTComputeJacobOutputBufferName = "JacobOutputTex";
    static public string IFFTComputeNormalInputBufferName = "NormalInputTex";
    static public string IFFTComputeNormalOutputBufferName = "NormalOutputTex";
    static public string IFFTComputeStageName = "Stage";
    static public string IFFTComputeStageGroupName = "GroupSize";
    static public string LutComputeKernelName = "CSMain";
    static public string LutComputeBufferName = "LutTex";
    static public string LutComputeSizeName = "Size";
    static public string SpectrumComputeKernelName = "CSMain";
    static public string SpectrumComputeResoName = "Resolution";
    static public string SpectrumComputeSizeName = "Size";
    static public string SpectrumComputeTimeName = "Time";
    static public string SpectrumComputeWindDirName = "WindDir";
    static public string SpectrumComputeWindSpeedName = "WindSpeed";
    static public string SpectrumComputeRandPairName = "RandomPair";
    static public string SpectrumComputeAmplitudeName = "Amplitude";
    static public string SpectrumComputeOutputTexName = "SpectrumTex";
    static public string IFFTComputeSizeName = "Size";
    static public string IFFTComputeLenghtName = "Length";
    static public string IFFTComputeCalLineName = "ComputeLine";
    static public string IFFTComputeTotalStageCountName = "TotalStageCount";
    static public string IFFTLutTexName = "LutTex";
    static public string OceanMatHeightTexName = "_OceanHeightMap";
    static public string OceanMatDisplaceTexName = "_OceanDisplaceMap";
    static public string OceanMatNormalTexName = "_OceanNormalMap";
    static public string OceanMatScaleName = "_OceanScale";
    /*#if _DEBUG_
        static public string IFFTDebugTexName = "DebugTex";
    #endif*/
    static public string SpecturmDebugTexName = "DebugTex";

}