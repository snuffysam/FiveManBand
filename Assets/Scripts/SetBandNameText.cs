using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;

public class SetBandNameText : MonoBehaviour
{
    public TMP_Text bandName1, bandName2;
    //public MeshRenderer bandNameLogo;
    string currentName;
    TextureStorer storer;
    // Start is called before the first frame update
    void Start()
    {
        storer = FindObjectOfType<TextureStorer>();
    }

    // Update is called once per frame
    void Update()
    {
        string str = "";
        if (TextBoxControl.replacements != null && TextBoxControl.replacements.ContainsKey("Pines Masters")){
            str = TextBoxControl.replacements["Pines Masters"];
        }

        if (str == currentName){
            return;
        }
        currentName = str;

        if (bandName1 != null && bandName2 != null){
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

        /*if (bandNameLogo != null){
            bandNameLogo.material.SetTexture("_MainTex", storer.storedArt);
        }*/
    }

    float GetCurvature(int length){
        return 0.019f*Mathf.Pow(length, 2f);
    }
}
