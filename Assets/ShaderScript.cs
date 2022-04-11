using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderScript : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    private int numberOfLayers;
    public Texture2D[] textures;

    private void OnValidate()
    {
        mat.shader = shader;
        numberOfLayers = textures.Length;
        mat.SetTexture("_MainTex", textures[0]);
        for(int i = 1; i < numberOfLayers; i++)
        {
            mat.SetTexture("_textureArray", textures[i]);
            mat.SetFloat("_totalWeight", 2);
            Debug.Log("Updated " + (i - 1));
            mat.SetFloat("_MainWeight", 1);
            mat.SetFloat("_LayerWeight", 1);
        }
    }


}
