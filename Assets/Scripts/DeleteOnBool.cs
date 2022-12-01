using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnBool : MonoBehaviour
{
    public string boolToCheck;
    public float delayTimer = 0f;
    float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delayTimer && !TextBoxControl.GetData(boolToCheck)){
            //Debug.Log("Deleted");
            Destroy(this.gameObject);
        }
    }
}
