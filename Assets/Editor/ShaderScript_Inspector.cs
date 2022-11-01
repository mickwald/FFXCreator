using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ShaderScript))]
public class ShaderScript_Inspector : Editor
{
    int currentLayer;
    bool hideTextureInspector = false;
    bool hideNoiseInspector = false;
    // Noise vars

    public override void OnInspectorGUI()
    {
        /*DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--------------------------------------------------------------------------");
        EditorGUILayout.Space();*/

        //Shader and material inputs:
        ShaderScript ssScript = (ShaderScript)target;
        if(ssScript.shader == null)
        {
            Shader textureShader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/TextureShader.shader", typeof(Shader));
            ssScript.shader = textureShader;
        }

        if (ssScript.shader != null)
        {
            if (ssScript.mat == null)
            {
                Material material = new Material(ssScript.shader);
                string path = Application.dataPath + "/ShaderScriptMaterials/";
                if (!System.IO.Directory.Exists(path))
                {
                    Debug.Log("Tried creating Path");
                    System.IO.Directory.CreateDirectory(path);
                }
                string filename = "Assets/ShaderScriptMaterials/" + ssScript.gameObject.name + ".mat";
                AssetDatabase.CreateAsset(material, filename);
                ssScript.mat = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(material), typeof(Material));
                ssScript.ApplyMaterial();
            }
            if (ssScript.mat == null) { EditorGUILayout.HelpBox("Please assign a dedicated material to the field below.", MessageType.Warning, true); }
            EditorGUI.BeginChangeCheck();
            Material mat = (Material)EditorGUILayout.ObjectField("Material", ssScript.mat, typeof(Material), false);
            if (EditorGUI.EndChangeCheck())
            {
                ssScript.mat = mat;
                ssScript.ApplyMaterial();

            }
        }

        hideTextureInspector = EditorGUILayout.Toggle("Hide Texture section", hideTextureInspector);


        if (!hideTextureInspector) CreateLayerInspector(ssScript);

        hideNoiseInspector = EditorGUILayout.Toggle("Hide Noise section", hideNoiseInspector);

        if (!hideNoiseInspector) CreateNoiseInspector(ssScript);
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
            imp.textureCompression = TextureImporterCompression.Uncompressed;
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
                ssScript.displacementID[currentLayer - 1] = 1;
                //ssScript.oldLayerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = ssScript.layerWeight[currentLayer - 1];
                //ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = 0;
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
                //ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = ssScript.oldLayerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1];
                ssScript.displacementID[currentLayer - 1] = (float)displaceID;
                //ssScript.oldLayerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1];
                //ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = 0;
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
                //ssScript.layerWeight[(int)ssScript.displacementID[currentLayer - 1] - 1] = ssScript.oldLayerWeight[currentLayer - 1];
                ssScript.displacementID[currentLayer - 1] = (float)0;
                //ssScript.oldLayerWeight[currentLayer - 1] = 0;
                ssScript.ReloadShader();
                EditorUtility.SetDirty(ssScript);
            }
        }
        EditorGUI.BeginChangeCheck();
        Vector2 scrollDirection = EditorGUILayout.Vector2Field("Scroll Direction", ssScript.scrollDirection[currentLayer - 1]);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.scrollDirection[currentLayer - 1] = scrollDirection;
            ssScript.loopTime[currentLayer - 1] = ssScript.GCD(scrollDirection.x, scrollDirection.y);
            ssScript.ReloadShader();
            EditorUtility.SetDirty(ssScript);
        }
        EditorGUI.BeginChangeCheck();
        Vector2 scrollOffset = EditorGUILayout.Vector2Field("Scroll Offset", ssScript.scrollOffset[currentLayer - 1]);
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
        /*if (GUILayout.Button("Refresh shader"))
        {
            ssScript.ReloadShader();
        }*/
    }

    private void CreateNoiseInspector(ShaderScript ssScript)
    {

        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField("~~~~ Noise Generation ~~~~", style);
        Texture2D noiseTex;
        noiseTex = (Texture2D)EditorGUILayout.ObjectField("Noise texture", ssScript.noiseTexture, typeof(Texture2D), true);
        if(noiseTex != null) noiseTex.hideFlags = HideFlags.DontSave;

        ssScript.noiseData.NoiseType = (ShaderScript.NoiseType)EditorGUILayout.EnumPopup("NoiseType", ssScript.noiseData.NoiseType);

        switch (ssScript.noiseData.NoiseType)
        {
            case ShaderScript.NoiseType.PERLIN:
                CreatePerlinSettings(ssScript);
                break;
            case ShaderScript.NoiseType.UNIFORM:
                CreateUniformSettings(ssScript);
                break;
            default:
                break;
        }


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
        if (GUILayout.Button("Generate noise"))
        {
            ssScript.GenerateNoise();
            ssScript.ReloadShader();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CreatePerlinSettings(ShaderScript ssScript)
    {
        //Noise settings
        ssScript.noiseData.offset = EditorGUILayout.Vector2Field("Offset", ssScript.noiseData.offset);
        GUIContent freqTooltip = new GUIContent("Frequency", "Base frequency used for noise generation. Higher frequencies gives more detail while lower frequencies make for smoother effects.");
        GUIContent ampFallTooltip = new GUIContent("Amplitude Falloff", "How much the amplitude changes for each octave.\n1 for no change, less than 1 makes each octave less important the previous one, larger than one makes higher frequencies more important.");
        GUIContent contTooltip = new GUIContent("Contrast", "Contrast for the noise texture. (0-1)\n 0 results in a flat grey image while 1 gives full nosie effect.");
        GUIContent alphaTooltip = new GUIContent("Generate Alpha", "If on, sets alpha to be the same value as the greyscale.\n If off, noisetexture will be fully opaque.");
        EditorGUI.BeginChangeCheck();
        float freq = EditorGUILayout.FloatField(freqTooltip, ssScript.noiseData.frequency);
        int oct = EditorGUILayout.IntField("Octaves", ssScript.noiseData.octaves);
        float ampFalloff = EditorGUILayout.FloatField(ampFallTooltip, ssScript.noiseData.amplitudeFalloff);
        float contrast = EditorGUILayout.FloatField(contTooltip, ssScript.noiseData.contrast);
        bool generateAlpha = EditorGUILayout.Toggle(alphaTooltip, ssScript.noiseData.generateAlpha);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.noiseData.frequency = freq;
            ssScript.noiseData.octaves = oct;
            ssScript.noiseData.amplitudeFalloff = ampFalloff;
            ssScript.noiseData.contrast = contrast;
            ssScript.noiseData.generateAlpha = generateAlpha;
            if (ssScript.noiseData.contrast < 0) ssScript.noiseData.contrast = 0;
            if (ssScript.noiseData.contrast > 1f) ssScript.noiseData.contrast = 1f;
            if (ssScript.noiseData.octaves < 1) ssScript.noiseData.octaves = 1;
            EditorUtility.SetDirty(ssScript);
        }
    }

    private void CreateUniformSettings(ShaderScript ssScript)
    {
        EditorGUI.BeginChangeCheck();
        float contrast = EditorGUILayout.FloatField("Contrast", ssScript.noiseData.contrast);
        if (EditorGUI.EndChangeCheck())
        {
            ssScript.noiseData.contrast = contrast;
            if (ssScript.noiseData.contrast < 0) ssScript.noiseData.contrast = 0;
            if (ssScript.noiseData.contrast > 1f) ssScript.noiseData.contrast = 1f;
            EditorUtility.SetDirty(ssScript);
        }
    }
}