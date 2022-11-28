using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;
    public bool fullTexture = true;
    TextureStorer textureStorer;
    // Start is called before the first frame update
    void Start()
    {
        textureStorer = FindObjectOfType<TextureStorer>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(TakeSnapshot());
    }

    public IEnumerator TakeSnapshot()
    {
        yield return new WaitForEndOfFrame();
        if (fullTexture){
            Destroy(textureStorer.storedTexture);
            textureStorer.storedTexture = ToTexture2D(renderTexture);
        } else {
            Destroy(textureStorer.storedArt);
            textureStorer.storedArt = ToTexture2D(renderTexture);
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
