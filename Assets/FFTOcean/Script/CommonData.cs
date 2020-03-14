using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData
{
    static public string ComputeKernelName = "CSMain";
    static public string ComputeInputBufferName = "InputTex";
    static public string ComputeOutputBufferName = "OutputTex";
    static public Vector3Int ComputeThreadSize = new Vector3Int(1024, 1, 1);
    static public Vector2Int TexSize = new Vector2Int(1024, 1024);
    static public string ComputeStageName = "Stage";
    static public string ComputeStageGroupName = "GroupSize";
}