using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    Vector3 startPos;
    Vector3 movePos;
    float shakeTimer;
    float shakeForce;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0f){
            shakeTimer -= Time.deltaTime;

            movePos = new Vector3(Random.Range(-1f, 1f),Random.Range(-1f, 1f),0f).normalized;
            movePos += (startPos-transform.position).normalized*Random.Range(0f, 1f);
            movePos = movePos.normalized*Random.Range(0f, 1f);
            movePos *= shakeForce*20f*Time.deltaTime*500f;

            Vector3 deltaPos = movePos + transform.position - startPos;
            float maxDist = (shakeForce*shakeForce)*2000f;
            if (deltaPos.sqrMagnitude > maxDist){
                deltaPos *= maxDist/deltaPos.sqrMagnitude;
            }

            if (shakeForce > shakeTimer){
                shakeForce -= Time.deltaTime*2f;
            }
            transform.position = startPos+deltaPos;
        } else {
            transform.position = startPos;
        }
    }

    public void Shake(float time, float force){
        shakeTimer = time;
        shakeForce = force;
        startPos = transform.position;
    }
}
