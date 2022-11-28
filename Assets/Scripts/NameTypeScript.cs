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
                GetComponent<DrawingScript>().renderArtCamera.gameObject.SetActive(true);
            } else {
                GetComponent<DrawingScript>().renderTexCamera.gameObject.SetActive(false);
                GetComponent<DrawingScript>().renderArtCamera.gameObject.SetActive(false);
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
            foreach (char c in Input.inputString)
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
        }

        if ((shouldOpen || (GetComponent<DrawingScript>() != null && !GetComponent<DrawingScript>().controlCamera)) && bandName1 != null && bandName2 != null){
            int amount = 0;
            int center = str.Length/2;
            int indexLower = 0;
            int indexHigher = str.Length-1;

            for (int i = center; i > 0; i--){
                if (str[i] == ' '){
                    indexLower = i;
                    break;
                }
            }
            for (int i = center; i < str.Length; i++){
                if (str[i] == ' '){
                    indexHigher = i;
                    break;
                }
            }

            if (center-indexLower <= indexHigher-center){
                amount = indexLower;
            } else {
                amount = indexHigher;
            }

            if (str.Length == 0){
                bandName1.text = "";
                bandName2.text = "";
            } else {
                if (amount == 0 || amount == str.Length-1){
                    bandName1.text = str;
                    bandName2.text = "";
                } else {
                    bandName1.text = str.Substring(0,amount);
                    bandName2.text = str.Substring(amount);
                }
                bandName1.GetComponent<WarpTextExample>().CurveScale = GetCurvature(bandName1.text.Length);
                bandName2.GetComponent<WarpTextExample>().CurveScale = -GetCurvature(bandName2.text.Length);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
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
            FindObjectOfType<TextBoxControl>().GoToLine(goToAfterDone);
        }
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
