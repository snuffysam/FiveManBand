using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicianData : MonoBehaviour
{
    public float verticalOffset = 0f; //current default position is -0.06
    public float horizontalBuffer = 0f;
    public Sprite portrait;
    public int[] stats = new int[4]; //charm, help, attack, defend
    public float[] AI = new float[]{0.5f, 0.5f, 0.5f, 0.5f}; //charm, help, attack, defend
    public float AIUseStars = 0.5f;
    public GameObject starParticle;
    private bool nextTurn = false;
    private int bonusStars = 0;
    public float startTime;
    public float bpm;
    bool setStart = true;
    Vector3 startPos;
    Vector3 startScale;
    Quaternion startRotation;
    float danceTiming = 0f;
    bool startDancing = false;
    bool canDance = true;
    public AudioSource musicBox;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (setStart && musicBox.time > 0f){
            setStart = false;
            startPos = transform.position;
            startScale = transform.localScale;
            startRotation = transform.rotation;
        }

        if (setStart){
            return;
        }

        //danceTiming += Time.deltaTime;

        danceTiming = musicBox.time;

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
            }
        } else if (danceTiming > startTime){
            startDancing = true;
            danceTiming -= startTime;
        }
    }

    public bool IsNextTurn(){
        return nextTurn;
    }

    public void ResetTurn(){
        bonusStars = 0;
        nextTurn = false;
    }

    public void SetBonusStars(int stars){
        bonusStars = stars;
    }

    public int GetStat(int index){
        return stats[index] + bonusStars;
    }

    public void StartCharm(BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(CharmCO(battleRunner, isEnemy));
    }

    IEnumerator CharmCO(BattleRunner battleRunner, bool isEnemy)
    {
        canDance = false;
        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }

        float startup = 0.5f;
        float timeCount = 0f;
        transform.localScale = startScale;
        transform.position = startPos;
        StartCoroutine(PLAYSFX("CharmEffect"));
        while (timeCount < startup){
            float delta = Time.deltaTime;
            timeCount += delta;
            yield return new WaitForSeconds(delta);
            transform.position = startPos + Vector3.right*(Mathf.Sin(timeCount*16f)-0.5f)*scalar*0.25f;
        }
        transform.position = startPos;
        battleRunner.AddHype(GetStat(0)*scalar);
        yield return new WaitForSeconds(0.5f);
        nextTurn = true;
        canDance = true;
    }

    public void StartHelp(BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(HelpCO(battleRunner, isEnemy));
    }

    IEnumerator HelpCO(BattleRunner battleRunner, bool isEnemy)
    {
        canDance = false;
        StartCoroutine(PLAYSFX("HelpEffect"));
        for (int i = 0; i < GetStat(1)+1; i++){
            GameObject go = Instantiate<GameObject>(starParticle);
            go.transform.position = transform.position+Vector3.up;
        }
        battleRunner.AddBonusStars(GetStat(1)+1,isEnemy);

        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }

        float startup = 0.5f;
        float timeCount = 0f;
        transform.localScale = startScale;
        transform.position = startPos;
        while (timeCount < startup){
            float delta = Time.deltaTime;
            timeCount += delta;
            yield return new WaitForSeconds(delta);
            if (timeCount < startup/2f){
                transform.Rotate(new Vector3(0f, 0f, -delta*100f*scalar));
            } else {
                transform.Rotate(new Vector3(0f, 0f, delta*100f*scalar));
            }
        }
        transform.rotation = startRotation;

        nextTurn = true;
        canDance = true;
    }

    public void StartAttack(GameObject attackIcon, BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(AttackCO(attackIcon, battleRunner, isEnemy));
    }

    IEnumerator AttackCO(GameObject attackIcon, BattleRunner battleRunner, bool isEnemy)
    {
        canDance = false;
        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }

        float startup = 1f;
        float timeCount = 0f;
        transform.localScale = startScale;
        transform.position = startPos;
        while (timeCount < startup){
            float delta = Time.deltaTime;
            timeCount += delta;
            yield return new WaitForSeconds(delta);
            transform.localScale = new Vector3(startScale.x-(timeCount*startScale.x*0.5f), startScale.y, startScale.z);
        }
        transform.localScale = startScale;

        int attacks = GetStat(2);
        for (int i = 0; i < attacks; i++){
            transform.localScale = startScale;
            StartCoroutine(PLAYSFX("AttackEffect"));
            GameObject go = Instantiate<GameObject>(attackIcon);
            go.transform.position = transform.position+Vector3.right*scalar*horizontalBuffer+Vector3.up*Random.Range(-0.5f,1.5f);
            go.GetComponent<Rigidbody2D>().velocity = new Vector2(scalar*20f,0f);
            go.GetComponent<SpriteRenderer>().flipX = isEnemy;
            go.GetComponent<AttackEffectScript>().isEnemy = isEnemy;
            go.GetComponent<AttackEffectScript>().toRun = battleRunner;
            GameObject shield = battleRunner.GetShield(isEnemy);
            go.GetComponent<AttackEffectScript>().shield = shield;
            battleRunner.RemoveShield(shield,isEnemy);
            float waitTime = 0f;
            if (shield == null){
                waitTime = 0.2f*(i+1);
            } else {
                waitTime = 0.03f*(i+1);
            }
            yield return new WaitForSeconds(waitTime/2f);
            if (i < attacks-1){
                transform.localScale = new Vector3(startScale.x*0.5f, startScale.y, startScale.z);
            }
            yield return new WaitForSeconds(waitTime/2f);
        }
        transform.localScale = startScale;

        yield return new WaitForSeconds(2f);
        nextTurn = true;
        canDance = true;
    }

    public void StartDefend(BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(DefendCO(battleRunner, isEnemy));
    }

    IEnumerator DefendCO(BattleRunner battleRunner, bool isEnemy)
    {
        canDance = false;
        float startup = 0.5f;
        float timeCount = 0f;
        transform.localScale = startScale;
        transform.position = startPos;
        StartCoroutine(PLAYSFX("DefendEffect"));
        while (timeCount < startup){
            float delta = Time.deltaTime;
            timeCount += delta;
            yield return new WaitForSeconds(delta);
            transform.localScale = new Vector3(startScale.x, startScale.y-(timeCount*startScale.y*0.5f), startScale.z);
            transform.position = startPos + Vector3.down*(timeCount*startScale.y*0.5f);
        }
        transform.localScale = startScale;
        transform.position = startPos;

        int shields = GetStat(3);
        for (int i = 0; i < shields; i++){
            battleRunner.SpawnShield(isEnemy);
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(0.75f);
        nextTurn = true;
        canDance = true;
    }

    IEnumerator PLAYSFX(string toPlay){
        yield return new WaitForSeconds(0f);

        float volumeMult = Random.Range(0.8f, 1.2f);
        float pitchMult = Random.Range(0.8f, 1.2f);

        AudioClip clip = Resources.Load<AudioClip>("SFX/" + toPlay);

        GameObject sfxSource = new GameObject();
        //sfxSource.transform.position = musicSource.transform.position;
        sfxSource.AddComponent<AudioSource>().clip = clip;
        sfxSource.GetComponent<AudioSource>().volume = TextBoxControl.sfxVolume*volumeMult*1.2f;
        sfxSource.GetComponent<AudioSource>().pitch = pitchMult;
        sfxSource.GetComponent<AudioSource>().Play();
        sfxSource.AddComponent<DestroyAudioSource>();
    }
}
