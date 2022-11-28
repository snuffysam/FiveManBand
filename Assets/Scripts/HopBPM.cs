using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopBPM : MonoBehaviour
{
    BattleRunner battleRunner;
    float danceTiming = 0f;
    bool startDancing = false;
    bool setStart = true;
    Vector3 startPos;
    Vector3 startScale;
    bool canDance = true;
    float startTime;
    float bpm;
    int currentHype;
    float jumpTimer;
    float jumpScalar = 1f;
    float lowerBound = 0.5f;
    float upperBound = 2f;
    AudioSource musicBox;
    // Start is called before the first frame update
    void Start()
    {
        battleRunner = FindObjectOfType<BattleRunner>();
        startPos = transform.position;
        startScale = transform.localScale;
        jumpScalar = Random.Range(lowerBound, upperBound);
    }

    // Update is called once per frame
    void Update()
    {
        if (setStart){
            musicBox = FindObjectOfType<MusicianData>().musicBox;
            if (musicBox.time > 0f){
                setStart = false;
                bpm = battleRunner.battleData.musicBPM;
                startTime = battleRunner.battleData.startTime;
                currentHype = battleRunner.GetCurrentHype();
            }
        }

        if (setStart){
            return;
        }

        //danceTiming += Time.deltaTime;

        danceTiming = musicBox.time;

        if (currentHype != battleRunner.GetCurrentHype()){
            canDance = false;
            currentHype = battleRunner.GetCurrentHype();
        }

        if (startDancing){
            float secondsPerBeat = 60f/bpm;
            danceTiming -= startTime;
            while (danceTiming > secondsPerBeat){
                danceTiming -= secondsPerBeat;
            }
            if (canDance){
                if (danceTiming < secondsPerBeat*0.1f || danceTiming > secondsPerBeat*0.9f){
                    transform.localScale = new Vector3(startScale.x, startScale.y*0.9f, startScale.z);
                    transform.position = startPos + Vector3.down*0.1f;
                } else {
                    transform.localScale = startScale;
                    transform.position = startPos;
                }
            } else {
                jumpTimer += Time.deltaTime*4f*jumpScalar;
                transform.position = startPos + Vector3.up*Mathf.Sin(jumpTimer)*2f;
                if (jumpTimer >= Mathf.PI){
                    jumpTimer = 0f;
                    transform.position = startPos;
                    canDance = true;
                    jumpScalar = Random.Range(lowerBound, upperBound);
                }
            }
        } else if (danceTiming > startTime){
            startDancing = true;
            danceTiming -= startTime;
        }
    }
}
