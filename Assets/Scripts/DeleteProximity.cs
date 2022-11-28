using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteProximity : MonoBehaviour
{
    public float minDistance;
    public int maxNumber;
    float aliveTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        /*foreach (DeleteProximity blob in others){
            if (count < maxNumber){
                break;
            }
            if (blob == this){
                continue;
            }
            if ((transform.position-blob.transform.position).sqrMagnitude < minDistance*blob.minDistance){
                Destroy(blob.gameObject);
                transform.localScale *= 1.05f;
                minDistance *= 1.02f;
                count--;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;

        if (transform.parent == null){
            Destroy(this.gameObject);
        }
    }

    public void DeleteClosest(DeleteProximity[] others){
        int count = others.Length;
        if (count < maxNumber){
            return;
        }

        int loopCount1 = 1;
        int loopCount2 = 1;
        int loopCount3 = 1;

        while (count > maxNumber*0.75f){

            for (int x = 0; x < (loopCount2+loopCount3)*16; x++){
                if (count < maxNumber/2){
                    break;
                }

                for (int i = 0; i < others.Length; i++){
                    bool found = false;
                    for (int k = i+1; k < others.Length; k++){
                        loopCount1++;
                        Debug.Log("loop paint 1");
                        if (others[i] == null){
                            continue;
                        }
                        if (others[k] == null){
                            continue;
                        }
                        if ((others[i].transform.position-others[k].transform.position).sqrMagnitude < minDistance){
                            if (others[i].aliveTime > others[k].aliveTime){
                                others[i].transform.SetParent(null);
                                Destroy(others[i].gameObject);
                            } else {
                                others[k].transform.SetParent(null);
                                Destroy(others[k].gameObject);
                            }
                            found = true;
                            count--;
                            break;
                        }
                    }
                    if (found){
                        break;
                    }
                }
            }

            for (int k = 0; k < (loopCount1+loopCount3)/16; k++){
                if (count < maxNumber/2){
                    break;
                }

                float oldestAge = 0f;
                int index1 = 0;

                for (int i = 0; i < others.Length; i++){
                    Debug.Log("loop paint 2");
                    loopCount2++;
                    if (others[i] == null){
                        continue;
                    }
                    if (others[i].aliveTime > oldestAge){
                        oldestAge = others[i].aliveTime;
                        index1 = i;
                    }
                }
                others[index1].transform.SetParent(null);
                Destroy(others[index1].gameObject);

                count--;
            }

            for (int i = 0; i < loopCount1+loopCount2; i++){
                if (count < maxNumber/2){
                    break;
                }

                loopCount3++;
                Debug.Log("loop paint 3");
                int randIndex = Random.Range(0,others.Length);
                if (others[randIndex] != null){
                    others[randIndex].transform.SetParent(null);
                    Destroy(others[randIndex].gameObject);
                    count--;

                }
            }
            Debug.Log("count: " + count + ", max number: " + maxNumber);
        }

        Debug.Log("loop counts: " + loopCount1 + ", " + loopCount2 + ", " + loopCount3);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
