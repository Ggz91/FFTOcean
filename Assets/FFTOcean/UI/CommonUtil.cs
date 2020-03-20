using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class CommonUtil
{
    static public void SaveRenderTexture(RenderTexture rt)
    {
        RenderTexture cur_rt = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        byte[] pixels = tex.EncodeToPNG();
        string LutTexPath = @"Assets/BakedTextures";
        if(!Directory.Exists(LutTexPath))
        {
            Directory.CreateDirectory(LutTexPath);
        }
        FileStream stream = File.Open(LutTexPath + @"/LutTex.png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(pixels);
        stream.Close();
        Texture2D.DestroyImmediate(tex);
        tex = null;
        RenderTexture.active = cur_rt;
    }

    static public void SaveConfigAsset(in ScriptableObject asset, string path)
    {
        int index = path.LastIndexOf(@"/");
        string folder_path = path.Substring(0, index);
        if(!Directory.Exists(folder_path))
        {
            Debug.Log("[SaveConfigAsset] folder_path : " + folder_path);
            Directory.CreateDirectory(folder_path);
        }
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static public Object LoadConfigAsset(string path, System.Type type)
    {
        return AssetDatabase.LoadAssetAtPath(path, type);
    }
}
