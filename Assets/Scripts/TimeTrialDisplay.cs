using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeTrialDisplay : MonoBehaviour
{
    public Image artwork;
    TimeTrialData timeTrialData;
    public TextMeshProUGUI battleNameText, recordTimeText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void DisplayBattleData(TimeTrialData bd){
        this.timeTrialData = bd;

        if (timeTrialData == null || !IsUnlocked(timeTrialData.unlockConditions)){
            artwork.gameObject.SetActive(false);
            battleNameText.text = "Time Trial is Locked!";
            recordTimeText.text = "";
            return;
        }

        artwork.gameObject.SetActive(true);
        artwork.sprite = timeTrialData.battleIcon;
        battleNameText.text = timeTrialData.fightName;
        recordTimeText.text = "Record:\n" + TextBoxControl.GetRecordTime(timeTrialData.recordKey);
    }

    public void LoadBattle(){

    }
}
