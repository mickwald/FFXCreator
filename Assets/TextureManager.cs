using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    private Material material;
    public Texture2D baseTex;

    // Start is called before the first frame update
    void Start()
    {
        ApplyTexture();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (baseTex == null)
        {
            return;
        }
        //ApplyTexture();
        //this.GetComponent<Renderer>().sharedMaterial;
    }

    private void ApplyTexture()
    {
        Material temp = new Material(this.gameObject.GetComponent<Renderer>().material);
        temp.mainTexture = baseTex;

        this.GetComponent<Renderer>().material = temp;
    }
}
