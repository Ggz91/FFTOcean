using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData
{
    static public string ComputeKernelName = "Main";
    static public string ComputeInputBufferName = "InputBuffer";
    static public string ComputeOutputBufferName = "OutputBuffer";
    static public Vector3Int ComputeThreadSize = new Vector3Int(1024, 1024, 1);
    static public Vector2Int TexSize = new Vector2Int(1024, 1024);
}