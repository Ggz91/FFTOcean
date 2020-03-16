using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class FFTOceanEidtorWindow : OdinMenuEditorWindow
{
    [MenuItem("Plugins/FFTOcean")]
    static void ShowWindow()
    {
        GetWindow<FFTOceanEidtorWindow>().Show();
    }

    PreComputeWIndowComponent m_pre_compute_component = new PreComputeWIndowComponent();
    protected override OdinMenuTree BuildMenuTree()
    {
        
        OdinMenuTree tree = new OdinMenuTree();
        tree.Add("预计算", m_pre_compute_component);
        return tree;
    }
}
