using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class ShaderScript : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    public int numberOfLayers = 1;
    public Texture2D[] textures = new Texture2D[32];
    public Texture2D tempMain;
    public Texture2D tempSecond;
    public Texture2DArray textureArray;
    public Vector2 scrollDirection = new Vector2(0f,0f);

    private void OnDrawGizmos()
    {
        if (mat == null) return;
        mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup);
    }

    private void OnValidate()
    {
        if (tempMain == null) return;
        textureArray = new Texture2DArray(tempMain.width, tempMain.height, numberOfLayers, TextureFormat.RGBA32, true, true);
        Color[] colors = new Color[tempMain.width*tempMain.height];
        colors = tempMain.GetPixels();
        textures[0] = tempMain;
        textures[1] = tempSecond;
        textureArray.SetPixels(colors, 0);
        colors = tempSecond.GetPixels();
        textureArray.SetPixels(colors, 1);
        textureArray.Apply();
        //mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup);
        float[] scrollDir = new float[2];
        scrollDir[0] = scrollDirection.x;
        scrollDir[1] = scrollDirection.y;
        mat.SetColor("_myColor", Color.green);
        mat.SetFloatArray("_scrollDirection", scrollDir);
        mat.SetTexture("_textureArray", textureArray);

        //mat.SetTexture("_MainTex", tempMain);
       /* for(int i = 1; i < numberOfLayers; i++)
        {
            mat.SetTexture("_textureArray", textures[i]);
            mat.SetFloat("_totalWeight", 2);
            Debug.Log("Updated " + (i - 1));
            mat.SetFloat("_MainWeight", 1);
            mat.SetFloat("_LayerWeight", 1);
        }*/
    }

    private void Update()
    {
        mat.SetFloat("_scrollTimer", Time.time);
    }

    public void Button()
    {
        Debug.Log("Test");
    }

    private void Reset()
    {
        numberOfLayers = 3;
    }

    public void SetCurrentTexture(int layer)
    {
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
}
