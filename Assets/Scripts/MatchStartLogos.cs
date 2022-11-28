using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchStartLogos : MonoBehaviour
{
    public float waitBeforeFade;
    public float fadeInTime;
    public float teleportX;
    public float waitTimeBeforeShake;
    public float waitTimeAfterShake;
    public float fadeOutTime;
    public string sfxOnShake;
    float timer;
    int mode = 4;
    float startX;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startX = rectTransform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (mode == 0){
            float alpha = 1f;
            if (fadeInTime > 0f){
                alpha = timer/fadeInTime;
            }
            alpha = Mathf.Clamp(alpha, 0f, 1f);

            rectTransform.localPosition = new Vector3(teleportX, rectTransform.localPosition.y, rectTransform.localPosition.z);
            if (GetComponent<RawImage>() != null){
                Color c = GetComponent<RawImage>().color;
                GetComponent<RawImage>().color = new Color(c.r, c.g, c.b, alpha);
            } else if (GetComponent<TextMeshProUGUI>() != null){
                Color c = GetComponent<TextMeshProUGUI>().color;
                GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, alpha);
            } else if (GetComponent<Image>() != null){
                Color c = GetComponent<Image>().color;
                GetComponent<Image>().color = new Color(c.r, c.g, c.b, alpha);
            }

            if (timer > fadeInTime){
                timer = 0f;
                mode = 1;
            }
        } else if (mode == 1){
            rectTransform.localPosition = new Vector3(teleportX+(startX-teleportX)*timer/waitTimeBeforeShake, rectTransform.localPosition.y, rectTransform.localPosition.z);

            if (timer > waitTimeBeforeShake){
                timer = 0f;
                rectTransform.localPosition = new Vector3(startX, rectTransform.localPosition.y, rectTransform.localPosition.z);
                if (GetComponent<ShakeObject>() != null){
                    StartCoroutine(PLAYSFX(sfxOnShake));
                    GetComponent<ShakeObject>().Shake(1.5f, 0.75f);
                }
                mode = 2;
            }
        } else if (mode == 2){
            if (timer > waitTimeAfterShake){
                timer = 0f;
                mode = 3;
            }
        } else if (mode == 4){
            if (timer > waitBeforeFade){
                timer = 0f;
                mode = 0;
            }
        } else {
            float alpha = (fadeOutTime-timer)/fadeOutTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);

            if (GetComponent<RawImage>() != null){
                Color c = GetComponent<RawImage>().color;
                GetComponent<RawImage>().color = new Color(c.r, c.g, c.b, alpha);
            } else if (GetComponent<TextMeshProUGUI>() != null){
                Color c = GetComponent<TextMeshProUGUI>().color;
                GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, alpha);
            } else if (GetComponent<Image>() != null){
                Color c = GetComponent<Image>().color;
                GetComponent<Image>().color = new Color(c.r, c.g, c.b, alpha);
            }

            if (timer > fadeOutTime){
                Destroy(this.gameObject);
            }
        }

        if (GetComponent<Image>() != null){
            //Debug.Log(GetComponent<Image>().color.a);
        }

    }

    IEnumerator PLAYSFX(string toPlay){
        yield return new WaitForSeconds(0f);

        float volumeMult = Random.Range(0.8f, 1.2f);
        float pitchMult = Random.Range(0.8f, 1.2f);

        AudioClip clip = Resources.Load<AudioClip>("SFX/" + toPlay);

        GameObject sfxSource = new GameObject();
        //sfxSource.transform.position = musicSource.transform.position;
        sfxSource.AddComponent<AudioSource>().clip = clip;
        sfxSource.GetComponent<AudioSource>().volume = TextBoxControl.sfxVolume*volumeMult*1.5f;
        sfxSource.GetComponent<AudioSource>().pitch = pitchMult;
        sfxSource.GetComponent<AudioSource>().Play();
        sfxSource.AddComponent<DestroyAudioSource>();
    }
}
