using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplayer : MonoBehaviour
{
    bool swapSprites;
    Sprite nextSprite;
    float fadeMaxTimer;
    float fadeTimer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (swapSprites){
            fadeTimer += Time.deltaTime;
            if (fadeTimer >= fadeMaxTimer){
                fadeTimer = fadeMaxTimer;
                fadeMaxTimer = 0f;
            }
            float fadeTimerScaled = 1f;
            if (fadeMaxTimer > 0f){
                fadeTimerScaled = fadeTimer/fadeMaxTimer;
            }

            if (fadeTimerScaled < 0.5f){
                GetComponent<Image>().color = new Color(1f, 1f, 1f, (0.5f-fadeTimerScaled)*2f);
            } else {
                GetComponent<Image>().sprite = nextSprite;
                if (nextSprite != null){
                    GetComponent<RectTransform>().sizeDelta = new Vector2(nextSprite.bounds.size.x, nextSprite.bounds.size.y)*100f;
                }
                GetComponent<Image>().color = new Color(1f, 1f, 1f, (fadeTimerScaled-0.5f)*2f);
            }
            if (fadeTimerScaled >= 1f){
                swapSprites = false;
            }
        } else {
            fadeTimer = 0f;
            fadeMaxTimer = 0f;
            nextSprite = null;
        }

        if (GetComponent<Image>().sprite == null){
            GetComponent<Image>().enabled = false;
        } else {
            GetComponent<Image>().enabled = true;
        }
    }

    public Sprite GetFutureSprite(){
        if (nextSprite == null){
            return GetComponent<Image>().sprite;
        }
        return nextSprite;
    }

    public void SwapSprite(string spriteName, float fadeTime){
        SwapSprite(Resources.Load<Sprite>(spriteName),fadeTime);
    }

    public void SwapSprite(Sprite next, float fadeTime){
        nextSprite = next;
        fadeMaxTimer = fadeTime;
        fadeTimer = 0f;
        swapSprites = true;
    }
}
