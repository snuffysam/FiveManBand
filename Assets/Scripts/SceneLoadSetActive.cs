using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadSetActive : MonoBehaviour
{
    public string newSceneName;
    public GameObject[] toActivate;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSceneLoad());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForSceneLoad(){
        while (!SceneManager.GetSceneByName(newSceneName).isLoaded || SceneManager.GetActiveScene().name != newSceneName){
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject go in toActivate){
            go.SetActive(true);
        }
    }
}
