using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.PlayerSettings;

public class MenuText : MonoBehaviour
{
    public ImageDisplayer FadeImage;
    public GameObject optionsMenu;
    public Camera renderTexCamera;
    public Texture2D currentStoredTexture;
    TextMeshProUGUI nameText;
    SaveSerial saveSerial;
    TextureStorer storer;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerSettings.WebGL.emscriptenArgs = "-s WASM_MEM_MAX=1024MB";
        TextureStorer[] storers = FindObjectsOfType<TextureStorer>();
        int count = 0;
        if (storers.Length > 1){
            renderTexCamera.gameObject.SetActive(false);
            foreach (TextureStorer textureStorer in storers){
                if (textureStorer.storedTexture == null){
                    Destroy(textureStorer.gameObject);
                    count++;
                } else {
                    if (currentStoredTexture == null){
                        currentStoredTexture = new Texture2D(512, 512);
                    }
                    if (textureStorer.storedTextureOld == null){
                        Graphics.CopyTexture(textureStorer.storedTexture,currentStoredTexture);
                    } else {
                        Graphics.CopyTexture(textureStorer.storedTextureOld,currentStoredTexture);
                    }
                    storer = textureStorer;
                    //Graphics
                    //textureStorer.storedTexture = textureStorer.storedTextureOld;
                }
                if (storers.Length-count == 1){
                    break;
                }
            }
        } else {
            renderTexCamera.gameObject.SetActive(true);
        }

        nameText = GetComponent<TextMeshProUGUI>();
        saveSerial = FindObjectOfType<SaveSerial>();
        //saveSerial.ResetData();
        saveSerial.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (storer != null){
            Graphics.CopyTexture(currentStoredTexture, storer.storedTexture);
        }
    }

    public void Highlight(string text){
        nameText.text = text;
    }

    public void Unhighlight(){
        nameText.text = "Click An Instrument To Start!";
    }

    public void PlayGame(){
        if (SaveSerial.HasSaveFile()){
            //Debug.Log("found save file");
            //saveSerial.LoadGame();
            GameObject go = new GameObject("Battle Message");
            go.AddComponent<BattleMessage>().message = "LOAD DATA";
        }
        TextBoxControl.cleanMode = false;
        //Debug.Log("ready for coroutine");
        StartCoroutine(PlayGameCO());
    }

    public void PlayGameClean(){
        if (SaveSerial.HasSaveFile()){
            //Debug.Log("found save file");
            //saveSerial.LoadGame();
            GameObject go = new GameObject("Battle Message");
            go.AddComponent<BattleMessage>().message = "LOAD DATA";
        }
        TextBoxControl.cleanMode = true;
        //Debug.Log("ready for coroutine");
        StartCoroutine(PlayGameCO());
    }

    public void OpenOptions(){
        optionsMenu.SetActive(true);
    }

    IEnumerator PlayGameCO(){
        FadeImage.GetComponent<ImageDisplayer>().SwapSprite("Backgrounds/BlackBackground", 0.5f);
        nameText.enabled = false;

        //Debug.Log("starting wait");

        yield return new WaitForSeconds(1f);

        //Debug.Log("ending wait");

        SceneManager.LoadScene("VisualNovelScene");
    }

    public void OpenGallery(){
        //Debug.Log("ready for coroutine");
        StartCoroutine(OpenGalleryCO());
    }

    IEnumerator OpenGalleryCO(){
        FadeImage.GetComponent<ImageDisplayer>().SwapSprite("Backgrounds/BlackBackground", 0.5f);
        nameText.enabled = false;

        //Debug.Log("starting wait");

        yield return new WaitForSeconds(1f);

        //Debug.Log("ending wait");

        SceneManager.LoadScene("GalleryScene");
    }
}
