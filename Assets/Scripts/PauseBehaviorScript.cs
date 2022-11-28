using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseBehaviorScript : MonoBehaviour
{
    public Slider textSpeedSlider;
    public Slider autoplaySpeedSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public GameObject logMenu;
    public TextMeshProUGUI logText;
    public Slider logPercentSlider;
    public GameObject areYouSureMenu;
    int logLines = 9;
    float logPercent = 1f;
    float textSpdSlope = 2.66f;
    float textSpdIntcpt = 0.67f;
    float autoplaySpdSlope = -2.2f;
    float autoplaySpdIntcpt = 2.6f;
    // Start is called before the first frame update
    void Start()
    {
        SetSliders();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSliders(){
        textSpeedSlider.value = RevertSlopeIntercept(TextBoxControl.textSpeed,textSpdSlope,textSpdIntcpt);
        autoplaySpeedSlider.value = RevertSlopeIntercept(TextBoxControl.autoplayDelay,autoplaySpdSlope,autoplaySpdIntcpt);
        musicVolumeSlider.value = TextBoxControl.musicVolume*2f;
        sfxVolumeSlider.value = TextBoxControl.sfxVolume*2f;
    }

    public void ResumeGame(){
        Time.timeScale = 1f;
        SaveSerial saveSerial = FindObjectOfType<SaveSerial>();
        if (saveSerial != null){
            saveSerial.SaveGame();
        }
        this.gameObject.SetActive(false);
    }

    public void SaveAndQuit(){
        SaveSerial saveSerial = FindObjectOfType<SaveSerial>();
        if (saveSerial != null){
            saveSerial.SaveGame();
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OpenLog(){
        logPercent = 1f;
        logPercentSlider.value = 1f;
        logText.text = GetLogPercent(logPercent);
        logMenu.SetActive(true);
    }

    public void CloseLog(){
        logMenu.SetActive(false);
    }

    public void OpenAreYouSure(){
        areYouSureMenu.SetActive(true);
    }

    public void CloseAreYouSure(){
        areYouSureMenu.SetActive(false);
    }

    public void DeleteSaveData(){
        SaveSerial saveSerial = FindObjectOfType<SaveSerial>();
        if (saveSerial != null){
            saveSerial.ResetData();
        }
        SetSliders();
        CloseAreYouSure();
    }

    public void MusicVolumeControl(System.Single vol){
        TextBoxControl.musicVolume = vol*0.5f;
    }

    public void SfxVolumeControl(System.Single vol){
        TextBoxControl.sfxVolume = vol*0.5f;
    }

    public void TextSpeedControl(System.Single vol){
        TextBoxControl.textSpeed = ConvertSlopeIntercept(vol,textSpdSlope,textSpdIntcpt);
    }

    public void AutoplaySpeedControl(System.Single vol){
        TextBoxControl.autoplayDelay = ConvertSlopeIntercept(vol,autoplaySpdSlope,autoplaySpdIntcpt);
    }
    public void LogPercentControl(System.Single vol){
        logPercent = vol;
        logText.text = GetLogPercent(logPercent);
    }

    public string GetLogPercent(float percent){
        if (TextBoxControl.adventureLog == null){
            return "";
        }
        int charPerLine = 40;
        int totalLines = 0;
        foreach (string str in TextBoxControl.adventureLog){
            totalLines += (str.Length/charPerLine)+2;
        }

        int maxIndex = totalLines-logLines;
        if (maxIndex < 0){
            maxIndex = 0;
        }
        int percentageLines = (int)(maxIndex*percent);
        int index = 0;

        int lineCount = 0;
        for (int i = 0; i < TextBoxControl.adventureLog.Count;i++){
            lineCount += (TextBoxControl.adventureLog[i].Length/charPerLine)+2;
            if (lineCount >= percentageLines){
                index = i;
                break;
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        
        string logDisplay = "";
        for (int i = index; i < TextBoxControl.adventureLog.Count; i++){
            logDisplay += TextBoxControl.adventureLog[i] + "\n\n";
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        return logDisplay;
    }

    float ConvertSlopeIntercept(float original, float slope, float intercept){
        return (original*slope)+intercept;
    }

    float RevertSlopeIntercept(float original, float slope, float intercept){
        return (original-intercept)/slope;
    }
}
