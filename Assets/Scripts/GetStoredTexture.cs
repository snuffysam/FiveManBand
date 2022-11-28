using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetStoredTexture : MonoBehaviour
{
    public bool artTexture = false;
    TextureStorer storer;
    // Start is called before the first frame update
    void Start()
    {
        storer = FindObjectOfType<TextureStorer>();
    }

    // Update is called once per frame
    void Update()
    {
        Texture2D tex = storer.storedTexture;
        if (artTexture){
            Graphics.CopyTexture(storer.storedArt,tex);
        }
        if (GetComponent<RawImage>() != null){
            GetComponent<RawImage>().texture = tex;
        } else if (GetComponent<MeshRenderer>() != null){
            GetComponent<MeshRenderer>().material.mainTexture = tex;
        }
        if (artTexture){
            Destroy(this);
        }
    }
}
