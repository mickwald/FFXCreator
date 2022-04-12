using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderScript : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    public int numberOfLayers = 1;
    public Texture2D tempMain;
    public Texture2DArray textures;
    public int textureSize = 512;



    private void OnValidate()
    {
        textures = new Texture2DArray(textureSize*2, textureSize, numberOfLayers, TextureFormat.RGBA32, true, true);
        Color[] colors = new Color[textureSize*2*textureSize];
        colors = tempMain.GetPixels();
        Debug.Log("Fetched texture from tempMain");
        textures.SetPixels(colors, 0);
        Debug.Log("Applied to 0 index in textures");
        textures.Apply();
        Debug.Log("Applied to gpu");
        mat.shader = shader;
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


}
