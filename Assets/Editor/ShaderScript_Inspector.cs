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

        //Layer selection and Texture input
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current layer:");
        currentLayer = EditorGUILayout.IntSlider(currentLayer, 1, (ssScript.GetNumberOfLayers()));
        ssScript.currentLayer = currentLayer;
        EditorGUILayout.EndHorizontal();
        Texture2D texture = null;
        EditorGUI.BeginChangeCheck();
        if (ssScript.textures != null)
        {
            texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ssScript.textures[currentLayer-1], typeof(Texture2D), true);
        }
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.textures[currentLayer - 1] = texture;
            ssScript.ReloadShader();
        }

        //Texture Settings
        //Displacement
        if(ssScript.displacementID[currentLayer-1] == 0)
        {
            bool temp = false;
            temp = EditorGUILayout.Toggle("Should this texture be displaced?", temp);
            if (temp) { ssScript.displacementID[currentLayer - 1] = 1; }
        }
        if(ssScript.displacementID[currentLayer-1] != 0)
        {
            bool temp = true;
            temp = EditorGUILayout.Toggle("Should this texture be displaced?", temp);
            ssScript.displacementID[currentLayer - 1] = EditorGUILayout.IntSlider("Displace by what layer?", ssScript.displacementID[currentLayer - 1], 1, ssScript.GetNumberOfLayers());
            if (!temp) { ssScript.displacementID[currentLayer - 1] = 0; }
        }
        EditorGUI.BeginChangeCheck();
        Vector2 scrollDirection = (Vector2)EditorGUILayout.Vector2Field("Scroll Direction", ssScript.scrollDirection[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.scrollDirection[currentLayer - 1] = scrollDirection;
            ssScript.ReloadShader();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Loop time: ");
        EditorGUILayout.LabelField((1/ssScript.loopTime[currentLayer - 1]).ToString());
        EditorGUILayout.EndHorizontal();
        ssScript.layerWeight[currentLayer - 1] = EditorGUILayout.FloatField("Layer Weight",ssScript.layerWeight[currentLayer-1]);
        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh shader"))
        {
            ssScript.ReloadShader();
        }
    }

}