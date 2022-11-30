using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSerial : MonoBehaviour
{
    TextBoxControl tbs;
    public static string filename = "B5MB_Save";
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SaveGame()
    {
        tbs = FindObjectOfType<TextBoxControl>();
        if (tbs == null){
            return;
        }
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + "/"+filename+".dat"); 
        SaveData data = new SaveData();
        if (tbs != null){
            data.currentLine = tbs.GetCurrentLine();
            data.currentBackground = GetName(tbs.background.GetComponent<ImageDisplayer>().GetFutureSprite());
            data.currentL1 = GetName(tbs.left1.GetComponent<ImageDisplayer>().GetFutureSprite());
            data.currentL2 = GetName(tbs.left2.GetComponent<ImageDisplayer>().GetFutureSprite());
            data.currentR1 = GetName(tbs.right1.GetComponent<ImageDisplayer>().GetFutureSprite());
            data.currentR2 = GetName(tbs.right2.GetComponent<ImageDisplayer>().GetFutureSprite());
            data.currentMusic = GetName(tbs.musicSource.clip);
            data.currentOptions = tbs.GetOptionsLoaded();
            //Debug.Log("saved options: " + data.currentOptions);
            data.currentMusicTrackVolume = tbs.musicSource.volume/TextBoxControl.musicVolume;
        }
        data.replacements = TextBoxControl.replacements;
        data.gameData = TextBoxControl.gameData;
        data.adventureLog = TextBoxControl.adventureLog;
        data.autoplayEnabled = TextBoxControl.autoplayEnabled;
        data.musicVolume = TextBoxControl.musicVolume;
        data.sfxVolume = TextBoxControl.sfxVolume;
        data.textSpeed = TextBoxControl.textSpeed;
        data.autoplayDelay = TextBoxControl.autoplayDelay;
        data.textSoundName = TextBoxControl.textSoundName;
        data.textSoundMaxIndex = TextBoxControl.textSoundMaxIndex;

        TextureStorer textureStorer = FindObjectOfType<TextureStorer>();
        if (textureStorer != null && textureStorer.storedArt != null){
            data.bandLogo = ImageConversion.EncodeToPNG (textureStorer.storedArt);
            //Debug.Log("savedTexture");
        }

        bf.Serialize(file, data);
        file.Close();
        //Debug.Log("Game data saved!");
    }

    public void SaveGameStart(){
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + "/"+filename+".dat"); 
        SaveData data = new SaveData();
        
        data.currentLine = 0;
        data.currentBackground = "na";
        data.currentL1 = "na";
        data.currentL2 = "na";
        data.currentR1 = "na";
        data.currentR2 = "na";
        data.currentMusic = "na";
        data.currentOptions = "na";
        data.textSoundName = "na";
        data.textSoundMaxIndex = 0;
        //Debug.Log("saved options: " + data.currentOptions);
        data.currentMusicTrackVolume = tbs.musicSource.volume/TextBoxControl.musicVolume;
            
        data.replacements = TextBoxControl.replacements;
        data.gameData = TextBoxControl.gameData;
        data.adventureLog = TextBoxControl.adventureLog;
        data.autoplayEnabled = TextBoxControl.autoplayEnabled;
        data.musicVolume = TextBoxControl.musicVolume;
        data.sfxVolume = TextBoxControl.sfxVolume;
        data.textSpeed = TextBoxControl.textSpeed;
        data.autoplayDelay = TextBoxControl.autoplayDelay;

        TextureStorer textureStorer = FindObjectOfType<TextureStorer>();
        if (textureStorer != null && textureStorer.storedArt != null){
            data.bandLogo = ImageConversion.EncodeToPNG (textureStorer.storedArt);
            //Debug.Log("savedTexture");
        }

        bf.Serialize(file, data);
        file.Close();
    }

    string GetName(UnityEngine.Object spr){
        if (spr == null){
            return "na";
        } else {
            return spr.name;
        }
    }

    public void LoadGame()
    {
        if (HasSaveFile())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/"+filename+".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            //Debug.Log("got file");
            TextBoxControl.replacements = data.replacements;
            TextBoxControl.gameData = data.gameData;
            Debug.Log("Loaded Game Data");
            TextBoxControl.adventureLog = data.adventureLog;
            TextBoxControl.autoplayEnabled = data.autoplayEnabled;
            TextBoxControl.musicVolume = data.musicVolume;
            TextBoxControl.sfxVolume = data.sfxVolume;
            TextBoxControl.textSpeed = data.textSpeed;
            TextBoxControl.autoplayDelay = data.autoplayDelay;
            TextBoxControl.textSoundName = data.textSoundName;
            TextBoxControl.textSoundMaxIndex = data.textSoundMaxIndex;

            TextureStorer textureStorer = FindObjectOfType<TextureStorer>();
            if (textureStorer != null && /*textureStorer.storedArt != null &&*/ data.bandLogo != null){
                if (textureStorer.storedArt == null){
                    textureStorer.storedArt = new Texture2D(512, 512);
                }
                ImageConversion.LoadImage(textureStorer.storedArt, data.bandLogo, false); 
                Debug.Log("Set Stored Texture");
                //textureStorer.storedArt = new Texture2D (textureStorer.storedArt.width, textureStorer.storedArt.height);
                //textureStorer.storedArt.LoadRawTextureData (data.bandLogo);
                GetStoredTexture gts = FindObjectOfType<GetStoredTexture>();
                if (gts.GetComponent<MeshRenderer>() != null){
                    gts.GetComponent<MeshRenderer>().material.mainTexture = textureStorer.storedArt;
                }
                //Debug.Log("loaded texture");
            }

            tbs = FindObjectOfType<TextBoxControl>();
            if (tbs != null){
                tbs.LoadData(data.currentLine, data.autoplayEnabled, data.currentBackground, data.currentL1, data.currentL2, data.currentR1, data.currentR2, data.currentMusic,
                    data.currentMusicTrackVolume, data.musicVolume, data.sfxVolume, data.textSpeed, data.autoplayDelay, data.adventureLog, data.replacements, data.gameData, data.currentOptions);
            }
        }
    }

    public static bool HasSaveFile(){
        return File.Exists(Application.persistentDataPath + "/"+filename+".dat");
    }

    public void ResetData(){
        if (HasSaveFile()){
            File.Delete(Application.persistentDataPath + "/"+filename+".dat");
        }
        TextBoxControl.replacements = null;
        TextBoxControl.gameData = null;
        TextBoxControl.adventureLog = null;
        TextBoxControl.autoplayEnabled = false;
        TextBoxControl.musicVolume = 0.5f;
        TextBoxControl.sfxVolume = 0.5f;
        TextBoxControl.textSpeed = 2f;
        TextBoxControl.autoplayDelay = 1.5f;
        TextBoxControl.textSoundName = "na";
        TextBoxControl.textSoundMaxIndex = 0;
    }

}

[Serializable]
class SaveData
{
    public int currentLine, textSoundMaxIndex;
    public bool autoplayEnabled;
    public string currentBackground, currentL1, currentL2, currentR1, currentR2;
    public string currentMusic, textSoundName;
    public string currentOptions;
    public float currentMusicTrackVolume, musicVolume, sfxVolume, textSpeed, autoplayDelay;
    public List<string> adventureLog;
    public Dictionary<string, string> replacements;
    public Dictionary<string, bool> gameData;
    public byte[] bandLogo;
}

[Serializable]
class SerializedTexture
{
    public int x, y;
    public byte[] bytes;
}