using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ShaderScript))]
public class ShaderScript_Inspector : Editor
{
    int currentLayer;
    float[] oldLayerWeight = new float[32];

    // Noise vars

    Vector2 offset = new Vector2();
    float frequency;
    int octaves;

    public override void OnInspectorGUI()
    {
        /*DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--------------------------------------------------------------------------");
        EditorGUILayout.Space();*/

        //Shader and material inputs:
        ShaderScript ssScript = (ShaderScript)target;
        if (ssScript.shader == null) { EditorGUILayout.HelpBox("Please assign the Texture Shader to the field below.", MessageType.Warning, true); }
        EditorGUI.BeginChangeCheck();
        Shader shader = (Shader)EditorGUILayout.ObjectField("Texture Shader", ssScript.shader, typeof(Shader), false);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.shader = shader;
        }

        if (ssScript.shader != null)
        {
            if (ssScript.mat == null) { EditorGUILayout.HelpBox("Please assign a dedicated material to the field below.", MessageType.Warning, true); }
            EditorGUI.BeginChangeCheck();
            Material mat = (Material)EditorGUILayout.ObjectField("Material", ssScript.mat, typeof(Material), false);
            if (EditorGUI.EndChangeCheck())
            {
                ssScript.mat = mat;
                ssScript.ApplyMaterial();

            }
        }


        CreateLayerInspector(ssScript);

        CreateNoiseInspector(ssScript);
    }

    private void CreateLayerInspector(ShaderScript ssScript)
    {
        
        //Script settings:
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();

        //Layer selection and Texture input
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current layer:");
        currentLayer = EditorGUILayout.IntSlider(ssScript.currentLayer, 1, (ssScript.GetNumberOfLayers()));
        ssScript.currentLayer = currentLayer;
        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginChangeCheck();
        Texture2D texture = null;
        if (ssScript.textures != null)
        {
            texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ssScript.textures[currentLayer - 1], typeof(Texture2D), true);

        }
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.textures[currentLayer - 1] = texture;
            ssScript.layerWeight[currentLayer - 1] = 1; //% label
            ssScript.textureScale[currentLayer - 1] = 1;
            ssScript.color[currentLayer - 1] = Color.white;
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter imp = (TextureImporter)TextureImporter.GetAtPath(path);
            imp.isReadable = true;
            imp.SaveAndReimport();
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        //Texture Settings
        EditorGUI.BeginChangeCheck();
        Color color = EditorGUILayout.ColorField("Color", ssScript.color[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.color[currentLayer - 1] = color;
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        EditorGUI.BeginChangeCheck();
        float scale = EditorGUILayout.FloatField("Scale", ssScript.textureScale[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            if (scale < 0)
            {
                scale = 0;
            }
            ssScript.textureScale[currentLayer - 1] = scale;
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        //Displacement
        if (ssScript.displacementID[currentLayer - 1] == 0)
        {
            bool temp = false;
            temp = EditorGUILayout.Toggle("Should this texture be displaced?", temp);
            if (temp)
            {
                oldLayerWeight[currentLayer - 1] = ssScript.layerWeight[currentLayer - 1];
                ssScript.displacementID[currentLayer - 1] = 1;
                ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = 0;
                ssScript.ReloadShader();
                EditorUtility.SetDirty(ssScript);
            }
        }
        if (ssScript.displacementID[currentLayer - 1] != 0)
        {
            bool temp = true;
            temp = EditorGUILayout.Toggle("Should this texture be displaced?", temp);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int displaceID = EditorGUILayout.IntSlider("Displace by what layer?", (int)ssScript.displacementID[currentLayer - 1], 1, ssScript.GetNumberOfLayers());
            if (EditorGUI.EndChangeCheck())
            {
                ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = oldLayerWeight[currentLayer - 1];
                ssScript.displacementID[currentLayer - 1] = (float)displaceID;
                oldLayerWeight[currentLayer - 1] = ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1];
                ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = 0;
                ssScript.ReloadShader();
                EditorUtility.SetDirty(ssScript);
            }
            if (GUILayout.Button("Go to"))
            {
                ssScript.currentLayer = (int)ssScript.displacementID[currentLayer - 1];
                EditorUtility.SetDirty(ssScript);
            }
            EditorGUILayout.EndHorizontal();
            if (!temp)
            {
                ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = oldLayerWeight[currentLayer - 1];
                ssScript.displacementID[currentLayer - 1] = (float)0;
                oldLayerWeight[currentLayer - 1] = 0;
                ssScript.ReloadShader();
                EditorUtility.SetDirty(ssScript);
            }
        }
        EditorGUI.BeginChangeCheck();
        Vector2 scrollDirection = (Vector2)EditorGUILayout.Vector2Field("Scroll Direction", ssScript.scrollDirection[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.scrollDirection[currentLayer - 1] = scrollDirection;
            ssScript.loopTime[currentLayer - 1] = ssScript.GCD(scrollDirection.x, scrollDirection.y);
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        EditorGUI.BeginChangeCheck();
        Vector2 scrollOffset = (Vector2)EditorGUILayout.Vector2Field("Scroll Offset (layer 1 only atm)", ssScript.scrollOffset[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.scrollOffset[currentLayer - 1] = scrollOffset;
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        /*EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Loop time: ");
        EditorGUILayout.LabelField((1 / ssScript.loopTime[currentLayer - 1]).ToString());
        EditorGUILayout.EndHorizontal();*/
        EditorGUI.BeginChangeCheck();
        float weight = EditorGUILayout.FloatField("Layer Weight", ssScript.layerWeight[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            if (weight < 0)
            {
                weight = 0;
            }
            ssScript.layerWeight[currentLayer - 1] = weight;
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh shader"))
        {
            ssScript.ReloadShader();
        }
    }

    private void CreateNoiseInspector(ShaderScript ssScript)
    {

        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField("~~~~ Noise ~~~~", style);
        Texture2D noiseTex = null;
        noiseTex = (Texture2D)EditorGUILayout.ObjectField("Noise texture", ssScript.noiseTexture, typeof(Texture2D), true);
        if(noiseTex != null) noiseTex.hideFlags = HideFlags.DontSave;
        //Noise settings
        offset = EditorGUILayout.Vector2Field("Offset", offset);
        frequency = EditorGUILayout.FloatField("Frequency", frequency);
        octaves = EditorGUILayout.IntField("Octaves", octaves);
        if (octaves < 1) octaves = 1;


        //Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply to current layer"))
        {
            ssScript.ApplyNoiseToLayer(currentLayer - 1);
            ssScript.ReloadShader();
        }
        if (GUILayout.Button("Save as PNG"))
        {
            ssScript.SaveNoiseTex();
        }
        if (GUILayout.Button("Generate Perlin noise"))
        {
            noiseTex = ssScript.GeneratePerlinNoise(offset, frequency, octaves);
            ssScript.ReloadShader();
        }
        if (GUILayout.Button("Generate noise"))
        {
            noiseTex = ssScript.GenerateNoise();
            ssScript.ReloadShader();
        }
        EditorGUILayout.EndHorizontal();
    }
}