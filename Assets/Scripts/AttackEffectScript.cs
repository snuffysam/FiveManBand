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
                Destroy(this.gameObject);
            } else if (!isEnemy && transform.position.x > 4.5f){
                toRun.DealDamage(1,!isEnemy);
                Destroy(this.gameObject);
            }
        } else {
            if (isEnemy && transform.position.x < shield.transform.position.x){
                Destroy(shield.gameObject);
                Destroy(this.gameObject);
            } else if (!isEnemy && transform.position.x > shield.transform.position.x){
                Destroy(shield.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
