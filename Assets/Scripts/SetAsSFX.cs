using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetAsSFX : EventTrigger
{
    AudioSource audioSource;
    float timer;
    float startTime = 0.5f;
    bool stopAudio = true;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<AudioSource>() == null){
            audioSource = this.gameObject.AddComponent<AudioSource>();
        } else {
            audioSource = GetComponent<AudioSource>();
        }
        audioSource.clip = Resources.Load<AudioClip>("SFX/BUTTON_12");
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < startTime && (stopAudio || Time.timeScale > 0f)){
            audioSource.Stop();
        }
        //audioSource.Stop();
        audioSource.volume = TextBoxControl.sfxVolume*0.5f;
    }

    public override void OnPointerClick(PointerEventData data)
    {
        if (Time.timeScale > 0f && (timer < startTime || !audioSource.enabled || !audioSource.gameObject.activeInHierarchy)){
            return;
        }
        stopAudio = false;
        audioSource.Stop();
        audioSource.Play();
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        if (Time.timeScale > 0f && (timer < startTime || !audioSource.enabled || !audioSource.gameObject.activeInHierarchy)){
            return;
        }
        stopAudio = false;
        audioSource.Stop();
        //Debug.Log("OnPointerEnter called.");
        audioSource.Play();
    }
}
