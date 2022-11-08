using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public MusicianData[] playerTeam;
    public MusicianData[] enemyTeam;
    public AudioClip playerMusic;
    public AudioClip enemyMusic;
    public int totalHype = 100;
    public int startingHype = 50;
    public int playerHealth = 5;
    public int enemyHealth = 5;
    public float AIDelay = 1f;
    public Sprite[] audienceSprites = new Sprite[3];
    public Sprite backgroundSprite;
    public Color playerColor;
    public Color enemyColor;
    public float startTime;
    public float musicBPM;
    public float stageHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
