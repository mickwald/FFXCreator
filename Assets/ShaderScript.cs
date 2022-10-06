using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class ShaderScript : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    private const int NUMBER_OF_LAYERS = 32;

    public int currentLayer = 0;
    private double lastRedraw = 0;

    //Global settings

    //Texture arrays
    public Texture2D[] textures = new Texture2D[NUMBER_OF_LAYERS];
    public Texture2DArray textureArray;


    //Layer settings
    public Color[] color = new Color[NUMBER_OF_LAYERS];
    public Vector2[] scrollDirection = new Vector2[NUMBER_OF_LAYERS];
    public Vector2[] scrollOffset = new Vector2[NUMBER_OF_LAYERS];
    public float[] layerWeight = new float[NUMBER_OF_LAYERS];
    public float[] oldLayerWeight = new float[NUMBER_OF_LAYERS];
    public float[] loopTime = new float[NUMBER_OF_LAYERS];
    public float[] displacementID = new float[NUMBER_OF_LAYERS];
    public float[] textureScale = new float[NUMBER_OF_LAYERS];


    public Texture2D noiseTexture;

    public enum NoiseType { PERLIN, UNIFORM };
    [System.Serializable]
    public struct NoiseData
    {
        public NoiseData(NoiseType type = default, Vector2 off = default, float freq = default, int oct = 1, float af = default, float cont = 1, bool alpha = default)
        {
            NoiseType = type;
            offset = off;
            frequency = freq;
            octaves = oct;
            amplitudeFalloff = af;
            contrast = cont;
            generateAlpha = alpha;
        }
        public NoiseType NoiseType;
        public Vector2 offset;
        public float frequency;
        public int octaves;
        public float amplitudeFalloff;
        public float contrast;
        public bool generateAlpha;

        public static NoiseData init => new NoiseData(NoiseType.PERLIN);
    }
    public NoiseData noiseData;
    private NoiseData generatedNoiseData;



    private void OnDrawGizmos()
    {
        if (Time.realtimeSinceStartupAsDouble - lastRedraw > 1.0)
        {
            ReloadShader();
        }
        lastRedraw = Time.realtimeSinceStartupAsDouble;
        if (shader == null || mat == null) return;
        float[] temp = new float[NUMBER_OF_LAYERS];
        for (int i = 0; i < NUMBER_OF_LAYERS; i++)
        {
            //loopTime[i] = (float)GCD(scrollDirection[i].x, scrollDirection[i].y);
            if (loopTime[i] != 0)
            {
                temp[i] = 1 / loopTime[i];
                temp[i] = (float)(Time.realtimeSinceStartupAsDouble % (double)(temp[i] / textureScale[i]));
            }
            else
            {
                temp[i] = 0;
            }
        }
        mat.SetFloatArray("_scrollTimer", temp);
    }


    private void OnValidate()
    {
        if (mat == null || shader == null) return;
        mat.shader = shader;
        if (textures == null) return;
        if (textures[0] != null)
        {
            textureArray = new Texture2DArray(textures[0].width, textures[0].height, NUMBER_OF_LAYERS, TextureFormat.RGBA32, true, true);
            textureArray.hideFlags = HideFlags.DontSave;
        }

        UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += ReloadOnSave;
        ReloadShader();
    }

    public int GetNumberOfLayers()
    {
        return NUMBER_OF_LAYERS;
    }

    private void Start()
    {
        ReloadShader();
    }
    private void Update()
    {
        if (shader == null || mat == null) return;
        float[] temp = new float[NUMBER_OF_LAYERS];
        for (int i = 0; i < NUMBER_OF_LAYERS; i++)
        {
            loopTime[i] = (float)GCD(scrollDirection[i].x, scrollDirection[i].y);
            if (loopTime[i] != 0)
            {
                temp[i] = 1 / loopTime[i];
            }
            temp[i] = Time.time % (temp[i]/textureScale[i]);
        }
        mat.SetFloatArray("_scrollTimer", temp);
    }

    private void ReloadOnSave(UnityEngine.SceneManagement.Scene scene)
    {
        ReloadShader();
    }

    public void ReloadShader()
    {
        if (textureArray == null)
        {
            if (textures[0] != null)
            {
                textureArray = new Texture2DArray(textures[0].width, textures[0].height, NUMBER_OF_LAYERS, TextureFormat.RGBA32, true, true);
                textureArray.hideFlags = HideFlags.DontSave;
            }
            else
            {
                if(this.mat != null) Debug.Log("Assign texture to layer 1. GameObject: " + this.gameObject.name);
                return;
            }
        }
        Color[] colors = new Color[textureArray.width * textureArray.height];
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
            {
                colors = textures[i].GetPixels();
                textureArray.SetPixels(colors, i);
            }

        }
        textureArray.Apply();
        mat.SetTexture("_textureArray", textureArray);

        float[] scrollDirX = new float[NUMBER_OF_LAYERS];
        float[] scrollDirY = new float[NUMBER_OF_LAYERS];
        float[] scrollOffX = new float[NUMBER_OF_LAYERS];
        float[] scrollOffY = new float[NUMBER_OF_LAYERS];

        for (int i = 0; i < NUMBER_OF_LAYERS; i++)
        {
            scrollDirX[i] = scrollDirection[i].x;
            scrollDirY[i] = scrollDirection[i].y;
            scrollOffX[i] = scrollOffset[i].x;
            scrollOffY[i] = scrollOffset[i].y;
        }
        mat.SetColorArray("_color", color);
        mat.SetFloatArray("_textureScale", textureScale);
        mat.SetFloatArray("_scrollDirectionX", scrollDirX);
        mat.SetFloatArray("_scrollDirectionY", scrollDirY);
        mat.SetFloatArray("_scrollOffsetX", scrollOffX);
        mat.SetFloatArray("_scrollOffsetY", scrollOffY);
        mat.SetFloatArray("_displacementIndex", displacementID);
        float totalWeight = 0f;
        for (int i = 0; i < NUMBER_OF_LAYERS; i++)
        {
            totalWeight += layerWeight[i];
        }
        mat.SetFloat("_totalWeight", totalWeight);
        mat.SetFloatArray("_layerWeight", layerWeight);

    }

    private void Reset()
    {
        textures = new Texture2D[NUMBER_OF_LAYERS];
        color = new Color[NUMBER_OF_LAYERS];
        textureArray = null;
        scrollDirection = new Vector2[NUMBER_OF_LAYERS];
        scrollOffset = new Vector2[NUMBER_OF_LAYERS];
        layerWeight = new float[NUMBER_OF_LAYERS];
        oldLayerWeight = new float[NUMBER_OF_LAYERS];
        loopTime = new float[NUMBER_OF_LAYERS];
        displacementID = new float[NUMBER_OF_LAYERS];
        textureScale = new float[NUMBER_OF_LAYERS];
        noiseData = NoiseData.init;
        noiseTexture = null;
        generatedNoiseData = default;

    }

    public void SetCurrentTexture(int layer)
    {
        if (textures == null || textures[layer] == null) return;
        Color[] colors = new Color[textureArray.width * textureArray.height];
        colors = textures[layer].GetPixels();
        textureArray.SetPixels(colors, layer);
        textureArray.Apply();
        mat.SetTexture("_textureArray", textureArray);
    }
    public void GetCurrentTexture(int layer)
    {
        Debug.Log("Getting");
        if (textureArray == null) return;
        Debug.Log("TexArr not null");
        Color[] colors = new Color[textureArray.width * textureArray.height];
        colors = textureArray.GetPixels(layer);
        textures[layer].SetPixels(colors);
        
    }


    public float GCD(float aIn, float bIn)
    {
        float intFactor = 100000f;
        float epsilon = 0.00002f;
        aIn += (aIn > 0) ? epsilon : -epsilon;
        bIn += (bIn > 0) ? epsilon : -epsilon;
        int a = Math.Abs((int)(aIn * intFactor));
        int b = Math.Abs((int)(bIn * intFactor));
        int Remainder;
        //Debug.Log("A: " + a + ", B: " + b + "Input: a=" + aIn + ", b=" + bIn);
        while (b != 0)
        {
            Remainder = a % b;
            a = b;
            b = Remainder;
        }
        return a/intFactor;


    }

    public void ApplyMaterial()
    {
        if (shader != null)
        {
            mat.shader = shader;
        }
        this.GetComponent<Renderer>().material = this.mat;
    }

    public Texture2D GenerateNoise()
    {
        return noiseData.NoiseType switch
        {
            NoiseType.PERLIN => GeneratePerlinNoise(),
            NoiseType.UNIFORM => GenerateUniformNoise(),
            _ => null,
        };
    }

    public Texture2D GeneratePerlinNoise()
    {
        int width, height;
        if (textures != null)
        {
            width = textures[0].width;
            height = textures[0].height;
        } else
        {
            width = height = 512;
        }
        Texture2D temp = new Texture2D(width, height, TextureFormat.RGBA32, true, true);
        for (int i = 0; i < temp.width; i++)
        {
            float x = (float)i / (float)width;
            for (int j = 0; j < height; j++)
            {
                float y = (float)j / (float)height;
                float greyScale = 0;
                float powSum = 0f;
                for (int octave = 1; octave <= noiseData.octaves; octave++)
                {
                    float amplitudePow = (float)Math.Pow(noiseData.amplitudeFalloff, octave - 1);
                    powSum += amplitudePow;
                    //if (i == 200 && j == 200) Debug.Log(((amplitudeFalloff * 1) - amplitudeFalloff * greyScale) / (powSum));
                    greyScale += ((amplitudePow * Mathf.PerlinNoise(noiseData.offset.x + x * noiseData.frequency * octave, noiseData.offset.y + y * noiseData.frequency * octave)) - noiseData.amplitudeFalloff*greyScale) / (powSum);
                }
                greyScale -= .5f;
                greyScale *= noiseData.contrast;
                greyScale += .5f;
                temp.SetPixel(i, j, new Color(greyScale, greyScale, greyScale, noiseData.generateAlpha ? greyScale : 1));
            }
        }
        temp.Apply();
        noiseTexture = temp;
        noiseTexture.hideFlags = HideFlags.DontSave;
        generatedNoiseData = noiseData;
        return noiseTexture;
    }

    public Texture2D GenerateUniformNoise()
    {
        Texture2D temp = new Texture2D(512, 512, TextureFormat.RGBA32, true, true);
        for (int i = 0; i < temp.width; i++)
        {
            for (int j = 0; j < temp.height; j++)
            {
                float greyScale = UnityEngine.Random.Range(0f, 1f);
                greyScale -= .5f;
                greyScale *= noiseData.contrast;
                greyScale += .5f;
                //temp.SetPixel(i, j, new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                temp.SetPixel(i, j, new Color(greyScale, greyScale, greyScale, noiseData.generateAlpha ? greyScale : 1));

            }
        }
        temp.Apply();
        noiseTexture = temp;
        noiseTexture.hideFlags = HideFlags.DontSave;
        generatedNoiseData = noiseData;
        return noiseTexture;
    }

    public String SaveNoiseTex()
    {
        Texture2D saveTex = noiseTexture;
        byte[] textureData = saveTex.EncodeToPNG();
        string path = Application.dataPath + "/NoiseTextures/";
        if (!System.IO.Directory.Exists(path))
        {
            Debug.Log("Tried creating Path");
            System.IO.Directory.CreateDirectory(path);
        }
        string timestamp = System.DateTime.Now.ToString("yyMMdd_HHmmss");
        string filePathName = path + GetString(generatedNoiseData.NoiseType) + "Noise_";
        if (noiseData.NoiseType == NoiseType.PERLIN)
        {
            filePathName += "off" + generatedNoiseData.offset + "_freq" + generatedNoiseData.frequency + "_oct" + generatedNoiseData.octaves + "_af" + generatedNoiseData.amplitudeFalloff + "_cont" + generatedNoiseData.contrast + "_alpha" + (generatedNoiseData.generateAlpha?"on":"off");
        }
        else
        {
            filePathName += timestamp;
        }
        filePathName += ".png";
        System.IO.File.WriteAllBytes(filePathName, textureData);
        System.IO.File.SetAttributes(filePathName, System.IO.File.GetAttributes(filePathName) & ~System.IO.FileAttributes.ReadOnly);
        UnityEditor.AssetDatabase.Refresh();
        string texturePath = filePathName;
        string dataPath = Application.dataPath;
        dataPath = dataPath.Replace("Assets", "");
        texturePath = texturePath.Replace(dataPath, "");
        TextureImporter imp = (TextureImporter)TextureImporter.GetAtPath(texturePath);
        imp.isReadable = true;
        imp.SaveAndReimport();
        ReloadShader();
        return filePathName;
    }

    public void ApplyNoiseToLayer(int targetLayer)
    {
        if (noiseTexture == null) return;
        string texturePath = SaveNoiseTex();
        string dataPath = Application.dataPath;
        dataPath = dataPath.Replace("Assets", "");
        texturePath = texturePath.Replace(dataPath, "");
        textures[targetLayer] = (Texture2D) AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
        textureScale[targetLayer] = 1;
    }

    public static string GetString(NoiseType type)
    {
        return type switch
        {   
            NoiseType.PERLIN => "Perlin",
            NoiseType.UNIFORM => "Uniform",
            _ => "Undefined",
        };
    }
}

