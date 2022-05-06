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

    public int currentLayer;

    //Texture arrays
    public Texture2D[] textures = new Texture2D[NUMBER_OF_LAYERS];
    public Texture2DArray textureArray;

    //Layer settings
    public Vector2[] scrollDirection = new Vector2[NUMBER_OF_LAYERS];
    public bool[] scrollDirInverter = new bool[NUMBER_OF_LAYERS];
    public float[] layerWeight = new float[NUMBER_OF_LAYERS];
    public float[] loopTime = new float[NUMBER_OF_LAYERS];


    private void OnDrawGizmos()
    {
        if (shader == null || mat == null) return;
        loopTime[0] = (float) GCD(scrollDirection[0].x, scrollDirection[0].y);      //TODO: Update to proper layer usage
        mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup % (1 / loopTime[0]));        //TODO: Update to proper layer usage
    }

    private void OnValidate()
    {
        if (mat == null || shader == null) return;
        mat.shader = shader;
        //Upload new texture?
        //SetCurrentTexture(currentLayer);
        
        if (textures == null) return;
        if (textures[0] != null)
        {
            textureArray = new Texture2DArray(textures[0].width, textures[0].height, NUMBER_OF_LAYERS, TextureFormat.RGBA32, true, true);
            Color[] colors = new Color[textures[0].width * textures[0].height];
        }
        //mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup);
        float[] scrollDir = new float[2];
        scrollDir[0] = scrollDirection[0].x;        //TODO: Update to proper layer usage
        scrollDir[1] = scrollDirection[0].y;       //TODO: Update to proper layer usage
        mat.SetColor("_myColor", Color.green);
        mat.SetFloatArray("_scrollDirection", scrollDir);
        mat.SetTexture("_textureArray", textureArray);

    }

    public int GetNumberOfLayers()
    {
        return NUMBER_OF_LAYERS;
    }

    private void Update()
    {
        mat.SetFloat("_scrollTimer", Time.time);
    }

    public void Button()
    {
        if (textureArray == null)
        {
            if (textures[0] != null)
            {
                //Debug.Log("Allocated Texture Array");
                Debug.Log("Texture size: " + textures[0].width);
                textureArray = new Texture2DArray(textures[0].width, textures[0].height, NUMBER_OF_LAYERS, TextureFormat.RGBA32, true, true);
            }
            else
            {
                Debug.Log("Assign texture to layer 1");
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
        //Debug.Log("Button Pressed");
        textureArray.Apply();
        mat.SetTexture("_textureArray", textureArray);
        float[] scrollDir = new float[2];
        scrollDir[0] = scrollDirection[0].x;        //TODO: Update to proper layer usage
        scrollDir[1] = scrollDirection[0].y;       //TODO: Update to proper layer usage
        mat.SetFloatArray("_scrollDirection", scrollDir);

    }

    private void Reset()
    { 
        scrollDirection = new Vector2[NUMBER_OF_LAYERS];
        scrollDirInverter = new bool[NUMBER_OF_LAYERS];
        layerWeight = new float[NUMBER_OF_LAYERS];
        loopTime = new float[NUMBER_OF_LAYERS];
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
        //Color[] colors = textures.GetPixels(layer);
        colors = textureArray.GetPixels(layer);
        textures[layer].SetPixels(colors);
        
        //currentTexture.SetPixels(colors);
    }

    private double GCD(double a, double b)
    {
        if(a < b)
        {
            return GCD(b, a);
        }

        if(Math.Abs(b) < 0.00001)
        {
            return a;
        } else
        {
            return GCD(b, a - Math.Floor(a / b) * b);
        }

    }
}
