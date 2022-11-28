using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectScript : MonoBehaviour
{
    public bool isEnemy;
    public BattleRunner toRun;
    public GameObject shield;
    // Start is called before the first frame update
    void Start()
    {
        if (shield != null){
            transform.position = new Vector3(transform.position.x, shield.transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shield == null){
            if (isEnemy && transform.position.x < -4.5f){
                toRun.DealDamage(1,!isEnemy);
                PLAYSFX("HIT_METAL_WRENCH_HEAVIEST_02");
                Destroy(this.gameObject);
            } else if (!isEnemy && transform.position.x > 4.5f){
                toRun.DealDamage(1,!isEnemy);
                PLAYSFX("HIT_METAL_WRENCH_HEAVIEST_02");
                Destroy(this.gameObject);
            }
        } else {
            if (isEnemy && transform.position.x < shield.transform.position.x){
                Destroy(shield.gameObject);
                PLAYSFX("PUNCH_ELECTRIC_HEAVY_02");
                Destroy(this.gameObject);
            } else if (!isEnemy && transform.position.x > shield.transform.position.x){
                Destroy(shield.gameObject);
                PLAYSFX("PUNCH_ELECTRIC_HEAVY_02");
                Destroy(this.gameObject);
            }
        }
    }

    void PLAYSFX(string toPlay){

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
