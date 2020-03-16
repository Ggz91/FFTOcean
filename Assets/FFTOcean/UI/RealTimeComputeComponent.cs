using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class RealTimeComputeComponent
{
    [InfoBox(@"实时渲染之前的一些参数设置：
    1、Mesh相关")]

    [BoxGroup("Mesh Param")]
    [MinValue(1)]
    public Vector2Int Size = new Vector2Int(2, 2);
    
    [BoxGroup("Mesh Param")]
    public Vector3 Position = Vector3.zero;

    [BoxGroup("Mesh Param")]
    [MinValue(0.000000001f)]
    public float UnitSize = 0.5f;

    [BoxGroup("Mesh Param")]
    [Button("Gen Mesh")]
    void GenMesh()
    {
        //生成mesh
        Mesh mesh = GenMeshImp();
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        obj.transform.position = Position;
        Debug.Log("[GenMesh] Done");
    }

    Mesh GenMeshImp()
    {
        Mesh mesh = new Mesh();
        List<Vector3> pos = new List<Vector3>();
        List<Vector3> normal = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> indice = new List<int>();
        int index = -1;
        for(int i = -Size.x/2; i <= Size.x/2; ++i)
        {
            for(int j = -Size.y/2; j <= Size.y/2; ++j)
            {
                index++;
                //顶点位置
                Vector3 offset = new Vector3(i * UnitSize, 0, j * UnitSize);
                pos.Add(Position + offset);
                normal.Add(new Vector3(0, 1, 0));
                uv.Add(new Vector2(i * 1.0f/Size.x, j * 1.0f/Size.y));

                //三角形indice,逆时针
                //最后一行和最后一列不生成
                if(Size.x/2 == i || Size.y/2 == j)
                {
                    continue;
                }
                
                int left_bottom = index;
                int right_bottom = left_bottom + 1;
                int left_top = index + Size.x + 1;
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
        mesh.SetUVs(0, uv);
        mesh.SetNormals(normal.ToArray());
        mesh.SetIndices(indice.ToArray(), MeshTopology.Triangles, 0);
        return mesh;
    }
}
