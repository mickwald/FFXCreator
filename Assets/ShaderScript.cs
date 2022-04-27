using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderScript : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    public int numberOfLayers = 1;
    public Texture2D tempMain;
    public Texture2D tempSecond;
    public Texture2DArray textures;
    public Vector2 scrollDirection = new Vector2(0f,0f);

    private void OnDrawGizmos()
    {
        mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup);
    }

    private void OnValidate()
    {
        
        textures = new Texture2DArray(tempMain.width, tempMain.height, numberOfLayers, TextureFormat.RGBA32, true, true);
        Color[] colors = new Color[tempMain.width*tempMain.height];
        colors = tempMain.GetPixels();
        textures.SetPixels(colors, 0);
        colors = tempSecond.GetPixels();
        textures.SetPixels(colors, 1);
        textures.Apply();
        //mat.SetFloat("_scrollTimer", Time.realtimeSinceStartup);
        float[] scrollDir = new float[2];
        scrollDir[0] = scrollDirection.x;
        scrollDir[1] = scrollDirection.y;
        mat.SetColor("_myColor", Color.green);
        mat.SetFloatArray("_scrollDirection", scrollDir);
        mat.SetTexture("_textureArray", textures);

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

}
