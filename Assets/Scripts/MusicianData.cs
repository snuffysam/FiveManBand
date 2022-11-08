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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        yield return new WaitForSeconds(0.5f);
        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }
        battleRunner.AddHype(GetStat(0)*scalar);
        yield return new WaitForSeconds(0.5f);
        nextTurn = true;
    }

    public void StartHelp(BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(HelpCO(battleRunner, isEnemy));
    }

    IEnumerator HelpCO(BattleRunner battleRunner, bool isEnemy)
    {
        for (int i = 0; i < GetStat(1)+1; i++){
            GameObject go = Instantiate<GameObject>(starParticle);
            go.transform.position = transform.position+Vector3.up;
        }
        yield return new WaitForSeconds(0.5f);
        battleRunner.AddBonusStars(GetStat(1)+1,isEnemy);
        nextTurn = true;
    }

    public void StartAttack(GameObject attackIcon, BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(AttackCO(attackIcon, battleRunner, isEnemy));
    }

    IEnumerator AttackCO(GameObject attackIcon, BattleRunner battleRunner, bool isEnemy)
    {
        yield return new WaitForSeconds(1.5f);
        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }

        int attacks = GetStat(2);
        for (int i = 0; i < attacks; i++){
            GameObject go = Instantiate<GameObject>(attackIcon);
            go.transform.position = transform.position+Vector3.right*scalar*horizontalBuffer+Vector3.up*Random.Range(-0.5f,1.5f);
            go.GetComponent<Rigidbody2D>().velocity = new Vector2(scalar*20f,0f);
            go.GetComponent<SpriteRenderer>().flipX = isEnemy;
            go.GetComponent<AttackEffectScript>().isEnemy = isEnemy;
            go.GetComponent<AttackEffectScript>().toRun = battleRunner;
            GameObject shield = battleRunner.GetShield(isEnemy);
            go.GetComponent<AttackEffectScript>().shield = shield;
            battleRunner.RemoveShield(shield,isEnemy);
            if (shield == null){
                yield return new WaitForSeconds(0.3f);
            } else {
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(1.5f);
        nextTurn = true;
    }

    public void StartDefend(BattleRunner battleRunner, bool isEnemy)
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine(DefendCO(battleRunner, isEnemy));
    }

    IEnumerator DefendCO(BattleRunner battleRunner, bool isEnemy)
    {
        yield return new WaitForSeconds(0.5f);
        int shields = GetStat(3);
        for (int i = 0; i < shields; i++){
            battleRunner.SpawnShield(isEnemy);
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(0.75f);
        nextTurn = true;
    }
}
