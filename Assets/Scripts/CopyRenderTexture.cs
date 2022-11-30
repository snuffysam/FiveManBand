using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;
    public bool fullTexture = true;
    TextureStorer textureStorer;
    TextBoxControl tbc;
    DrawingScript drawingScript;
    // Start is called before the first frame update
    void Start()
    {
        textureStorer = FindObjectOfType<TextureStorer>();
        tbc = FindObjectOfType<TextBoxControl>();
        drawingScript = FindObjectOfType<DrawingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("I Exist!");
        StartCoroutine(TakeSnapshot());
    }

    public IEnumerator TakeSnapshot()
    {
        yield return new WaitForEndOfFrame();
        if (fullTexture){
            if (tbc == null || !tbc.GetRenderedTexture()){
                Destroy(textureStorer.storedTextureOld);
                textureStorer.storedTextureOld = textureStorer.storedTexture;
                textureStorer.storedTexture = ToTexture2D(renderTexture);
            }
            //Debug.Log("Setting Stored Texture");
            if (tbc != null){
                tbc.SetRenderedTexture();
                //Debug.Log("Set Rendered Texture");
            }
        } else {
            Destroy(textureStorer.storedArtOld);
            textureStorer.storedArtOld = textureStorer.storedArt;
            textureStorer.storedArt = ToTexture2D(renderTexture);
            //Debug.Log("Setting Stored Texture");
            if (drawingScript != null){
                drawingScript.SetCapturedArt();
                //Debug.Log("Set Rendered Texture");
            }
            //Debug.Log("Setting Art Texture");
        }
        //Graphics.CopyTexture(tex, savedTexture);
    }

    public Texture2D ToTexture2D(RenderTexture rTex)
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rTex;
        GetComponent<Camera>().Render();
        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rTex.width, rTex.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentActiveRT;
        return tex;
    }
}
