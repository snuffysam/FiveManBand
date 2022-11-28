using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWithoutSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!SaveSerial.HasSaveFile()){
            Destroy(this.gameObject);
        }
        TextureStorer textureStorer = FindObjectOfType<TextureStorer>();
        if (textureStorer == null || textureStorer.storedArt == null){
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
