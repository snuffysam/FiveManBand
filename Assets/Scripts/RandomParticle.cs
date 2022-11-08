using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomParticle : MonoBehaviour
{
    public float timer = 1f;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));
        GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized*speed;
        StartCoroutine("WaitDelete");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitDelete()
    {
        yield return new WaitForSeconds(timer);
        Destroy(this.gameObject);
    }
}
