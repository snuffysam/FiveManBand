using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public GameObject selectionParent;
    public TextBoxControl textBoxControl;
    public string nextAction;
    bool readyForNext = true;
    BattleData battleData;
    bool enableCharm = true;
    bool enableHelp = true;
    bool enableAttack = true;
    bool enableDefend = true;
    public Button CharmButton;
    public Button HelpButton;
    public Button AttackButton;
    public Button DefendButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (battleData == null){
            battleData = GetComponent<BattleRunner>().battleData;
        }

        if (textBoxControl.CanInteract()){
            selectionParent.GetComponent<RectTransform>().localPosition = Vector3.zero;
            if (selectionParent.activeInHierarchy){
                readyForNext = true;
            }
        } else {
            selectionParent.GetComponent<RectTransform>().localPosition = Vector3.up*100000f;
        }

        if (readyForNext && !selectionParent.activeInHierarchy){
            readyForNext = false;
            if (nextAction.Length > 0){
                textBoxControl.GoToLine(nextAction);
            }
        }

        CharmButton.interactable = enableCharm;
        HelpButton.interactable = enableHelp;
        AttackButton.interactable = enableAttack;
        DefendButton.interactable = enableDefend;
    }

    public void ChangeSettings(string toGoTo, bool enableCharm, bool enableHelp, bool enableAttack, bool enableDefend, float enemySpeed, bool skipRound){
        nextAction = toGoTo;
        this.enableCharm = enableCharm;
        this.enableAttack = enableAttack;
        this.enableHelp = enableHelp;
        this.enableDefend = enableDefend;
        battleData.AIDelay = enemySpeed;
        
        if (skipRound){
            //Debug.Log("starting skip turn");
            GetComponent<BattleRunner>().ForcePlayerIndex(0);
        }
    }
}
