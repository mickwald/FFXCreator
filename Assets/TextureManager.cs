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

    

    public float scrollX = .5f;
    public float scrollY = .5f;

    private float offsetX = .5f;
    private float offsetY = .5f;


    void Update()
    {
        offsetX = scrollX * Time.time;
        offsetY = scrollY * Time.time;
        textureMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);

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
        textureMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    private void ScrollTexture()
    {
        offsetX = scrollX * Time.realtimeSinceStartup;
        offsetY = scrollY * Time.realtimeSinceStartup;

    }

    private void ApplyTexture()
    {
        textureMaterial.mainTexture = baseTex;

    }
}
