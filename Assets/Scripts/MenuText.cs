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
    TextMeshProUGUI nameText;
    SaveSerial saveSerial;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerSettings.WebGL.emscriptenArgs = "-s WASM_MEM_MAX=1024MB";
        TextureStorer[] storers = FindObjectsOfType<TextureStorer>();
        int count = 0;
        if (storers.Length > 1){
            foreach (TextureStorer textureStorer in storers){
                if (textureStorer.storedTexture == null){
                    Destroy(textureStorer.gameObject);
                    count++;
                }
                if (storers.Length-count == 1){
                    break;
                }
            }
        }

        nameText = GetComponent<TextMeshProUGUI>();
        saveSerial = FindObjectOfType<SaveSerial>();
        //saveSerial.ResetData();
        saveSerial.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
