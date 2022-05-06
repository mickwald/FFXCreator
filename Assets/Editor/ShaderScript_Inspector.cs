using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ShaderScript))]
public class ShaderScript_Inspector : Editor
{
    int currentLayer = 1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--------------------------------------------------------------------------");
        EditorGUILayout.Space();

        //Shader and material inputs:
        ShaderScript ssScript = (ShaderScript)target;
        if (ssScript.shader == null) { EditorGUILayout.HelpBox("Please assign the Texture Shader to the field below.", MessageType.Warning, true); }
        ssScript.shader = (Shader)EditorGUILayout.ObjectField("Texture Shader", ssScript.shader, typeof(Shader), false);
        if (ssScript.mat == null) { EditorGUILayout.HelpBox("Please assign a dedicated material to the field below.", MessageType.Warning, true); }
        ssScript.mat = (Material)EditorGUILayout.ObjectField("Material", ssScript.mat, typeof(Material), false);

        //Script settings:
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Color");
        ssScript.color = EditorGUILayout.ColorField(ssScript.color);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current layer:");
        currentLayer = EditorGUILayout.IntSlider(currentLayer, 1, (ssScript.GetNumberOfLayers()));
        ssScript.currentLayer = currentLayer;
        EditorGUILayout.EndHorizontal();
        if (ssScript.textures != null)
        {
            ssScript.textures[currentLayer-1] = (Texture2D)EditorGUILayout.ObjectField("Texture", ssScript.textures[currentLayer-1], typeof(Texture2D), true);
        }
        ssScript.scrollDirection[currentLayer - 1] = (Vector2)EditorGUILayout.Vector2Field("Scroll Direction", ssScript.scrollDirection[currentLayer - 1]);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Loop time: ");
        GUIStyle loopTimeStyle = new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleCenter};
        EditorGUILayout.LabelField((1/ssScript.loopTime[currentLayer - 1]).ToString(),loopTimeStyle);
        EditorGUILayout.EndHorizontal();
        ssScript.layerWeight[currentLayer - 1] = EditorGUILayout.FloatField("Layer Weight",ssScript.layerWeight[currentLayer-1]);
        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh shader"))
        {
            ssScript.Button();
        }
    }

}