using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartIconScript : MonoBehaviour
{
    public HeartIconScript previous;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HeartIconScript Remove(){
        StartCoroutine("RemoveCO");
        return previous;
    }

    IEnumerator RemoveCO()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
