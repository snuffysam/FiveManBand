using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarButtonScript : MonoBehaviour
{
    public BattleRunner battleRunner;
    bool isSelected = false;
    public Sprite starNormal;
    public Sprite starSelected;
    // Start is called before the first frame update
    void Start()
    {
        battleRunner = FindObjectOfType<BattleRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected){
            GetComponent<Image>().sprite = starSelected;
        } else {
            GetComponent<Image>().sprite = starNormal;
        }
    }

    public void ToggleSelection(){
        isSelected = !isSelected;
        battleRunner.AddRemoveBonus(isSelected);
    }
}
