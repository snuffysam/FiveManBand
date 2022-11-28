using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadScene(){
        BattleRunner bRunner = FindObjectOfType<BattleRunner>();
        GameObject go = new GameObject("Battle Message");
        go.AddComponent<BattleMessage>().message = bRunner.battleData.battleName;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
