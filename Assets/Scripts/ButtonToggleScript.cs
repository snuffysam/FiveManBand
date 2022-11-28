using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisible(bool visible){
        this.gameObject.SetActive(visible);
    }
}
