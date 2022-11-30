using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureStorer : MonoBehaviour
{
    public Texture2D storedTexture;
    public Texture2D storedTextureOld;
    public Texture2D storedArt;
    public Texture2D storedArtOld;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
