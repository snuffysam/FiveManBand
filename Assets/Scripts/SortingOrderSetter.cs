using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().sortingOrder = transform.parent.GetComponent<Renderer>().sortingOrder+1;
    }
}
