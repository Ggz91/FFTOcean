using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class RealTimeComputeComponent
{
    [InfoBox(@"实时渲染之前的一些参数设置：
    1、Mesh相关
    2、海洋参数设置")]
    [MinValue(1)]
    public int Resolution = 256;
    
    [BoxGroup("Mesh Param")]
    public Vector3 Position = Vector3.zero;

    [BoxGroup("Mesh Param")]
    [MinValue(0.000000001f)]
    public float UnitSize = 0.5f;

    [BoxGroup("Spectrum")]
    public ComputeShader SpectrumShader;


    [BoxGroup("Spectrum")]
    public Vector2 Wind = new Vector2(16, 16);

    [BoxGroup("Spectrum")]
    public float Amplitude = 1;

    [BoxGroup("IFFT")]
    public ComputeShader IFFTShader;

    [Button("Gen Ocean")]
    void GenOcean()
    {
      
        Mesh mesh = GenMeshImp();
        GenGameObj(mesh);
        Debug.Log("[GenMesh] Done");
    }

    void AddScript(GameObject obj)
    {
        obj.AddComponent<FFTOceanMonoComponent>();
        FillObjData(obj);
    }

    void AddMaterial(GameObject obj)
    {
        Shader shader = Shader.Find(@"Unlit/FFTOcean");
        Material mat = new Material(shader);

        obj.GetComponent<MeshRenderer>().material = mat;
    }
    void GenGameObj(Mesh mesh)
    {
        //创建海面
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        obj.transform.position = Position;

        //添加FFTOCean脚本
        AddScript(obj);

        //添加材质
        AddMaterial(obj);
    }

    void FillObjData(GameObject obj)
    {
        FFTOceanMonoComponent.InitParam init_param = ScriptableObject.CreateInstance<FFTOceanMonoComponent.InitParam>();
        //填充相关数据
        FillSpectrumData(ref init_param);
        FillIFFTData(ref init_param);
        obj.GetComponent<FFTOceanMonoComponent>().InitData(init_param);

        //保存成配置
        CommonUtil.SaveAsset(init_param, UICommonData.IFFTOceanInitConfig);
    }

    void FillSpectrumData(ref FFTOceanMonoComponent.InitParam param)
    {
        param.SpectrumParam.Resolution = Resolution;
        param.SpectrumParam.Size = Resolution * UnitSize;
        param.SpectrumParam.Wind = Wind;
        param.SpectrumParam.Amplitude = Amplitude;
        param.SpectrumParam.ComputeShader = SpectrumShader;
    }

    void FillIFFTData(ref FFTOceanMonoComponent.InitParam param)
    {
        param.IFFTParam.Size = Resolution;
        param.IFFTParam.ComputeShader = IFFTShader;
        
        param.IFFTParam.BufferFlyLutTex = AssetDatabase.LoadAssetAtPath(UICommonData.IFFTOceanLutTexPath, typeof(RenderTexture)) as RenderTexture;
    }

    Mesh GenMeshImp()
    {
        Mesh mesh = new Mesh();
        mesh.name = @"FFTOceanMesh";
        List<Vector3> pos = new List<Vector3>();
        List<Vector3> normal = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indice = new List<int>();
        int index = -1;
        for(int i = -Resolution/2; i <= Resolution/2; ++i)
        {
            for(int j = -Resolution/2; j <= Resolution/2; ++j)
            {
                index++;
                //顶点位置
                Vector3 offset = new Vector3(i * UnitSize, 0, j * UnitSize);
                pos.Add(Position + offset);
                normal.Add(new Vector3(0, 1, 0));
                Vector2 uv = new Vector2((i + Resolution / 2 ) * 1.0f/(Resolution-1), (j + Resolution / 2 ) * 1.0f/(Resolution-1));
                uvs.Add(uv);
                /*Debug.Log("[GenMesh] pos : " + offset.ToString()
                + " uv : " + uv.ToString());*/
                //三角形indice,逆时针
                //最后一行和最后一列不生成
                if(Resolution/2 == i || Resolution/2 == j)
                {
                    continue;
                }
                
                int left_bottom = index;
                int right_bottom = left_bottom + 1;
                int left_top = index + Resolution + 1;
                int right_top = left_top + 1;
                /*Debug.Log("[GenMesh] vertex : " + left_bottom.ToString() + ", "
                + right_bottom.ToString() + ","
                + left_top.ToString() + ","
                + right_top.ToString() );*/
                indice.Add(left_bottom);
                indice.Add(right_top);
                indice.Add(left_top);

                indice.Add(left_bottom);
                indice.Add(right_bottom);
                indice.Add(right_top);
            }
        }

        mesh.SetVertices(pos.ToArray());
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normal.ToArray());
        mesh.SetIndices(indice.ToArray(), MeshTopology.Triangles, 0);
        Debug.Log("[GenMesh] size : " + Resolution.ToString()
        + " vertices count : " + mesh.vertexCount.ToString());
        mesh.RecalculateBounds();
        return mesh;
    }

    public void Enter()
    {
        InitDefaultValues();
    }

     void InitDefaultComputeShader()
    {
        //spectrum shader
        string default_spectrum_compute_shader = @"Assets/FFTOcean/Shader/SpectrumComputeShader.compute";
        SpectrumShader = AssetDatabase.LoadAssetAtPath(default_spectrum_compute_shader, typeof(ComputeShader)) as ComputeShader;

        //ifft shader
        string default_ifft_compute_shader = @"Assets/FFTOcean/Shader/IFFTComupteShader.compute";
        IFFTShader = AssetDatabase.LoadAssetAtPath(default_ifft_compute_shader, typeof(ComputeShader)) as ComputeShader;
    }

    void InitDefaultValues()
    {
        //默认的compute shader
        InitDefaultComputeShader();
    }
}
