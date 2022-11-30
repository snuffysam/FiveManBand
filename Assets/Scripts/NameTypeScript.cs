using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;

public class NameTypeScript : MonoBehaviour
{
    public string key;
    public string goToAfterDone;
    public string description;
    public bool shouldOpen;
    public int maxCharacters;
    public Image portrait;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI nameText;
    float blinkTime;
    public TMP_Text bandName1, bandName2;
    TextBoxControl tbs;
    bool currentlyRendering = false;

    float growSpeed = 6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<DrawingScript>() != null){
            if (!GetComponent<DrawingScript>().controlCamera){
                GetComponent<DrawingScript>().renderTexCamera.gameObject.SetActive(true);
            } else if (shouldOpen){
                GetComponent<DrawingScript>().renderTexCamera.gameObject.SetActive(true);
            } else {
                GetComponent<DrawingScript>().renderTexCamera.gameObject.SetActive(false);
            }
        }

        if (shouldOpen && transform.localScale.y < 1f){
            transform.localScale += Vector3.up * Time.deltaTime*growSpeed;
            if (transform.localScale.y > 1f){
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            }
        } else if (!shouldOpen && transform.localScale.y > 0f) {
            transform.localScale -= Vector3.up * Time.deltaTime*growSpeed;
            if (transform.localScale.y < 0f){
                transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            }
        }
        blinkTime += Time.deltaTime*4f;

        descriptionText.text = description;
        string str = "";
        if (TextBoxControl.replacements != null && TextBoxControl.replacements.ContainsKey(key)){
            str = TextBoxControl.replacements[key];
        }
        
        if ((int)(blinkTime%2) == 0){
            nameText.text = str + "|";
        } else {
            nameText.text = str;
        }

        if (shouldOpen && TextBoxControl.replacements != null && TextBoxControl.replacements.ContainsKey(key)){
            string inString = Input.inputString;
            foreach (char c in inString)
            {
                AudioSource audioSource = nameText.GetComponent<AudioSource>();
                if (c == '\b' && TextBoxControl.replacements[key].Length > 0) // has backspace/delete been pressed?
                {
                    TextBoxControl.replacements[key] = TextBoxControl.replacements[key].Substring(0,TextBoxControl.replacements[key].Length-1);
                    audioSource.Stop();
                    audioSource.volume = TextBoxControl.sfxVolume;
                    audioSource.Play();
                }
                else if ((c != '\b') && (c != '\n') && (c != '\r') && (c != '\t') && TextBoxControl.replacements[key].Length < maxCharacters)
                {
                    TextBoxControl.replacements[key] += c;
                    audioSource.Stop();
                    audioSource.volume = TextBoxControl.sfxVolume;
                    audioSource.Play();
                }
            }
            if (inString.Length > 0 && GetComponent<DrawingScript>() != null){
                StartRefresh();
            }
        }
    }

    float GetCurvature(int length){
        return 0.019f*Mathf.Pow(length, 2f);
    }

    public void Open(string key, string goToAfterDone, string description, string spriteName, int maxCharacters){
        shouldOpen = true;
        this.key = key;
        this.goToAfterDone = goToAfterDone;
        this.description = description;
        this.portrait.sprite = Resources.Load<Sprite>(spriteName);
        this.maxCharacters = maxCharacters;
    }

    public void Open(string key, string goToAfterDone, string description, int maxCharacters){
        shouldOpen = true;
        this.key = key;
        this.goToAfterDone = goToAfterDone;
        this.description = description;
        this.maxCharacters = maxCharacters;
    }

    public void ClickDone(){
        if (shouldOpen){
            if (GetComponent<DrawingScript>() != null){
                GetComponent<DrawingScript>().SwitchToNaming();
            }
            shouldOpen = false;

            if (GetComponent<DrawingScript>() != null){
                StartCoroutine(WaitForArtwork());
            } else {
                FindObjectOfType<TextBoxControl>().GoToLine(goToAfterDone);
            }
        }
    }

    public void StartRefresh(){
        if (!currentlyRendering){
            StartCoroutine(RefreshRenderTexture());
        }
    }

    IEnumerator RefreshRenderTexture(){
        currentlyRendering = true;
        if (tbs == null){
            tbs = FindObjectOfType<TextBoxControl>();
        }
        if (tbs != null){
            tbs.ResetRenderedTexture();

            while (!tbs.GetRenderedTexture()){
                yield return new WaitForSeconds(0.1f);
            }

            TextureStorer textureStore = FindObjectOfType<TextureStorer>();
            textureStore.storedTexture = textureStore.storedTextureOld;

            tbs.ResetRenderedTexture();
            
            while (!tbs.GetRenderedTexture()){
                yield return new WaitForSeconds(0.01f);
            }
        }
        currentlyRendering = false;
    }

    IEnumerator WaitForArtwork(){
        GetComponent<DrawingScript>().StartCapturingArt();
        GetComponent<DrawingScript>().renderArtCamera.gameObject.SetActive(true);

        while (GetComponent<DrawingScript>().WaitToCaptureArt()){
            yield return new WaitForSeconds(0.1f);
        }

        GetComponent<DrawingScript>().renderArtCamera.gameObject.SetActive(false);

        FindObjectOfType<TextBoxControl>().GoToLine(goToAfterDone);
    }

    public void SwitchToDrawing(){
        GetComponent<DrawingScript>().enabled = true;
        GetComponent<DrawingScript>().SetAllActive(true);
        SetAllActive(false);
        this.enabled = false;
    }

    public void SetAllActive(bool isActive){
        descriptionText.gameObject.SetActive(isActive);
        nameText.gameObject.SetActive(isActive);
    }
}
