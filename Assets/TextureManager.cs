using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    public bool EditorScroll;


    private Material material;
    public Material textureMaterial;
    public Texture2D baseTex;


    public Vector2 scroll = new Vector2(.5f, .25f);

    public Vector2 offset = new Vector2(.5f, .5f);
    

    void Update()
    {
        offset += scroll * Time.deltaTime;
        textureMaterial.mainTextureOffset = offset;

    }

    private void OnValidate()
    {

        if (baseTex == null || textureMaterial == null)
        {
            return;
        }

        ApplyTexture();
    }
    private void OnDrawGizmosSelected()
    {
        if (EditorScroll) { ScrollTexture(); }
        textureMaterial.mainTextureOffset = offset;
    }

    private void ScrollTexture()
    {
        offset = scroll * Time.realtimeSinceStartup;

    }

    private void ApplyTexture()
    {
        textureMaterial.mainTexture = baseTex;

    }
}
