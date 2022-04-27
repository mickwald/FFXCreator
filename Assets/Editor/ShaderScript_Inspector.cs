using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ShaderScript))]
public class ShaderScript_Inspector : Editor
{
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Shader Script label");

        ShaderScript ssScript = (ShaderScript)target;
        if(GUILayout.Button("Click Me!")){
            ssScript.Button();
        }
        ssScript.tempMain = (Texture2D)EditorGUILayout.ObjectField("TextureLayer", ssScript.tempMain, typeof(Texture2D), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Texture");
        
    }
}