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

    OdinMenuTree m_tree;
    PreComputeWIndowComponent m_pre_compute_component = new PreComputeWIndowComponent();
    RealTimeComputeComponent m_realtime_compute_component = new RealTimeComputeComponent();
    protected override OdinMenuTree BuildMenuTree()
    {
        m_tree = new OdinMenuTree();
        m_tree.Selection.SelectionChanged += OnSelectedChanged;
        m_tree.Selection.SupportsMultiSelect = false;
        m_tree.Add("预计算", m_pre_compute_component);
        m_tree.Add("实时计算的相关设置", m_realtime_compute_component);
        //TrySelectMenuItemWithObject(m_pre_compute_component);
        return m_tree;
    }
    void OnEnterWindow()
    {
        if(m_tree.Selection.SelectedValue.GetType() == m_pre_compute_component.GetType())
        {
            m_pre_compute_component.Enter();
        }
        else if(m_tree.Selection.SelectedValue.GetType() == m_realtime_compute_component.GetType())
        {
            m_realtime_compute_component.Enter();
        }
    }
    void OnSelectedChanged(SelectionChangedType type)
    {
        if(SelectionChangedType.ItemAdded == type)
        {
            OnEnterWindow();
        }
    }
    
   
}
