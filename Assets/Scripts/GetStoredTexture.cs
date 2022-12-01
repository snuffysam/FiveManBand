using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetStoredTexture : MonoBehaviour
{
    public bool artTexture = false;
    public Texture2D tex;
    TextureStorer storer;
    TextBoxControl tbc;
    bool shouldGetTexture = true;
    // Start is called before the first frame update
    void Start()
    {
        storer = FindObjectOfType<TextureStorer>();
    }

    public void SetShouldGetTexture(){
        shouldGetTexture = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (tex == null){
            tex = new Texture2D(512, 512);
        }

        if (tbc == null){
            tbc = FindObjectOfType<TextBoxControl>();
        }

        if (artTexture){
            if (tex != null){
                //Debug.Log("Texture Exists!");
            } else {
                //Debug.Log("Texture Is Null!");
            }
        }

        if (artTexture){
            //Debug.Log("searching A");
        }

        if (artTexture && tex != null){
            if (tbc != null && (tbc.GetRenderedTexture() || true)){
                tbc.SetRenderedArtTexture();
                //Debug.Log("Stopped refreshing art texture!");
                return;
            }
            if (!shouldGetTexture){
                //Debug.Log("Skipped getting art texture!");
                //shouldGetTexture = true;
                return;
            }
            //Destroy(this);
        }
        
        if (artTexture){
            //Debug.Log("searching B");
        }

        if (storer.storedTexture != null){
            tex = storer.storedTexture;
        }
        if (artTexture && storer.storedArt != null){
            Graphics.CopyTexture(storer.storedArt,tex);
        }

        if (GetComponent<RawImage>() != null){
            GetComponent<RawImage>().texture = tex;
        } else if (GetComponent<MeshRenderer>() != null){
            GetComponent<MeshRenderer>().material.mainTexture = tex;
        }

        shouldGetTexture = false;

        if (artTexture && tex != null && tbc != null){
            tbc.SetRenderedArtTexture();
            //Debug.Log("Loaded art texture!");
            //Destroy(this);
        }
    }
}
