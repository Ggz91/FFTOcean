using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PreHandleComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PreComputeWIndowComponent.PreHandle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
