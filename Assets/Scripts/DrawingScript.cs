using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawingScript : MonoBehaviour
{
    public Color inkColor;
    public GameObject paint;
    public GameObject originObject;
    public Camera renderCamera;
    public Camera renderTexCamera;
    public Camera renderArtCamera;
    public GameObject colorButtonParent;
    public bool controlCamera = true;
    Vector3 mousePrev;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlCamera){
            renderTexCamera.gameObject.SetActive(true);
        } else if (GetComponent<NameTypeScript>().shouldOpen){
            renderTexCamera.gameObject.SetActive(true);
            renderArtCamera.gameObject.SetActive(true);
        } else {
            renderTexCamera.gameObject.SetActive(false);
            renderArtCamera.gameObject.SetActive(false);
        }

        Vector3 mousePos = renderCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0f)-originObject.transform.position;

        if (Input.GetAxis("MouseClick") > 0f && mousePos.magnitude < 2f){
            AudioSource audioSource = colorButtonParent.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.volume = TextBoxControl.sfxVolume;
            audioSource.Play();

            Vector3 direction = (mousePos-mousePrev);
            float amount = direction.magnitude;
            direction = direction.normalized;

            float gap = 0.025f;

            //Debug.Log("distance, " + amount + ", number to spawn, " + (int)(amount/gap));

            int loops = 0;

            GameObject mostRecent = null;

            if (amount > -1f){
                DeleteProximity[] others = originObject.GetComponentsInChildren<DeleteProximity>();
                for (int i = 0; i < amount/gap || i < 1; i++){
                    if (others.Length > 0 && others.Length > others[0].maxNumber){
                        break;
                    }

                    loops++;
                    Debug.Log("drawing loops: " + loops);
                    if (loops > 30){
                        Debug.Log("too many drawing loops");
                        break;
                    }
                    GameObject go = Instantiate<GameObject>(paint,originObject.transform);
                    go.GetComponent<SpriteRenderer>().color = inkColor;

                    Vector3 startPos = mousePrev + direction*(gap*i);

                    go.transform.position = new Vector3(startPos.x, startPos.y, -1f)+originObject.transform.position;
                    mostRecent = go;

                    others = originObject.GetComponentsInChildren<DeleteProximity>();
                }
                mostRecent.GetComponent<DeleteProximity>().DeleteClosest(others);
                Resources.UnloadUnusedAssets();
            }

        }

        mousePrev = mousePos;
    }

    public void SwitchToNaming(){
        GetComponent<NameTypeScript>().enabled = true;
        GetComponent<NameTypeScript>().SetAllActive(true);
        SetAllActive(false);
        this.enabled = false;
    }

    public void PickColor(string hexCode){
        string htmlValue = "#" + hexCode;

        Color newCol;

        if (ColorUtility.TryParseHtmlString(htmlValue, out newCol))
        {
            inkColor = newCol;
        }
    }

    public void SetAllActive(bool isActive){
        colorButtonParent.gameObject.SetActive(isActive);
    }

    public void ClearDrawings(){
        Renderer[] sprs = originObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer spriteRenderer in sprs){
            if (spriteRenderer.gameObject != originObject && spriteRenderer.GetComponent<TMP_Text>() == null){
                Destroy(spriteRenderer.gameObject);
            }
        }

        sprs = FindObjectsOfType<SpriteRenderer>();
        foreach (Renderer spriteRenderer in sprs){
            if (spriteRenderer.name == "Paint(Clone)"){
                Destroy(spriteRenderer.gameObject);
            }
        }
        Resources.UnloadUnusedAssets();
    }
}
