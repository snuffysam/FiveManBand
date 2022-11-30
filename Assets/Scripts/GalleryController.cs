using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GalleryController : MonoBehaviour
{
    public GalleryData lockedEntry;
    public GalleryData[] galleryEntries;
    public Image artwork;
    public RawImage playerLogo;
    public TextMeshProUGUI nameText, taglineText, descriptionText;
    int currentIndex = 0;
    int artworkIndex = 0;
    TextureStorer textureStorer;
    // Start is called before the first frame update
    void Start()
    {
        SetGallery();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetGallery(){
        if (textureStorer == null){
            textureStorer = FindObjectOfType<TextureStorer>();
        }

        GalleryData data = galleryEntries[currentIndex];
        if (!IsUnlocked(data.unlockConditions)){
            data = lockedEntry;
        }

        if (data.allSprites.Length > 0){
            artwork.gameObject.SetActive(true);
            artwork.sprite = data.allSprites[artworkIndex%data.allSprites.Length];
            artwork.GetComponent<RectTransform>().sizeDelta = new Vector2(artwork.sprite.bounds.size.x, artwork.sprite.bounds.size.y)*100f;
            if (artwork.GetComponent<RectTransform>().sizeDelta.x > 1450f && data.galleryName == "Roderick McNormous"){
                artwork.transform.localScale = Vector3.one*0.33f;
            } else if (artwork.GetComponent<RectTransform>().sizeDelta.x < 1000f && artwork.GetComponent<RectTransform>().sizeDelta.y < 1000f) {
                artwork.transform.localScale = Vector3.one*1f;
            } else {
                artwork.transform.localScale = Vector3.one*0.45f;
            }
            playerLogo.gameObject.SetActive(false);
        } else if (textureStorer != null){
            playerLogo.gameObject.SetActive(true);
            playerLogo.texture = textureStorer.storedTexture;
            artwork.gameObject.SetActive(false);
        } else {
            artwork.gameObject.SetActive(true);
            playerLogo.gameObject.SetActive(false);
        }

        nameText.text = ReplaceText(data.galleryName);
        taglineText.text = ReplaceText(data.tagline);
        descriptionText.text = ReplaceText(data.description);

    }
    bool IsUnlocked(string conditions){
        if (TextBoxControl.gameData == null){
            return false;
        }
        string[] eachCondition = conditions.Split(", ");
        foreach (string con in eachCondition){
            if (!TextBoxControl.GetData(con)){
                return false;
            }
        }
        return true;
    }
    public void ClickedForward(){
        if (galleryEntries.Length > 0){
            currentIndex = (currentIndex+1)%galleryEntries.Length;
        }
        artworkIndex = 0;
        SetGallery();
    }
    public void ClickedBack(){
        if (galleryEntries.Length > 0){
            currentIndex = (currentIndex+galleryEntries.Length-1)%galleryEntries.Length;
        }
        artworkIndex = 0;
        SetGallery();
    }
    public void CycledArt(){
        if (galleryEntries[currentIndex].allSprites.Length > 0){
            artworkIndex = (artworkIndex+1)%galleryEntries[currentIndex].allSprites.Length;
        }
        SetGallery();
    }
    public void ExitGallery(){
        //Debug.Break();
        SceneManager.LoadScene("MainMenu");
    }

    string ReplaceText(string original){
        int indexHigh = original.IndexOf("]");
        int indexLow = indexHigh;
        for (int i = indexHigh; i >= 0; i--){
            if (original[i] == '['){
                indexLow = i;
                break;
            }
        }

        while (indexHigh > indexLow){
            string toReplace = original.Substring(indexLow+1, indexHigh-indexLow-1);
            string[] parms = toReplace.Split(", ");
            if (indexLow < 0){
                indexLow = 0;
            }
            original = original.Substring(0, indexLow) + GetReplacement(parms) + original.Substring(indexHigh+1);

            indexHigh = original.IndexOf("]");
            indexLow = original.IndexOf("[");
        }

        return original;
    }

    string GetReplacement(string[] parms){
        string swap = parms[0];
        if (TextBoxControl.replacements != null && TextBoxControl.replacements.ContainsKey(swap)){
            swap = TextBoxControl.replacements[swap];
        }
        return swap;
    }
}
