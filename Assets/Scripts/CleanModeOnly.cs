using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanModeOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!TextBoxControl.cleanMode){
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
