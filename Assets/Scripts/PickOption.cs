using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickOption : MonoBehaviour
{
    public string SelectGoTo;
    public string DisableOnBool;
    public string HideOnBool;
    TextBoxControl tbc;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        tbc = FindObjectOfType<TextBoxControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tbc.CanInteract() && !TextBoxControl.GetData(DisableOnBool)){
            if (button != null){
                button.enabled = true;
            }
            if (TextBoxControl.GetData(HideOnBool)){
                if (button != null){
                    button.enabled = false;
                }
                GetComponent<Image>().enabled = false;
            } else {
                GetComponent<Image>().enabled = true;
            }
        } else {
            if (button != null){
                button.enabled = false;
            }
            if (TextBoxControl.GetData(HideOnBool)){
                GetComponent<Image>().enabled = false;
            } else {
                GetComponent<Image>().enabled = true;
            }
        }
    }

    public void ActivateClick(){
        tbc.GoToLine(SelectGoTo);
    }
}
