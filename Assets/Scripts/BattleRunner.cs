using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleRunner : MonoBehaviour
{
    public List<BattleData> allBattles;
    public BattleData battleData;
    public Slider hypeMeter;
    public Image hypePlayer;
    public Image hypeEnemy;
    public GameObject heartIcon;
    public Image[] BattlePortraits;
    public GameObject selectUI;
    public TextMeshProUGUI[] ButtonTexts;
    public SpriteRenderer Background;
    public SpriteRenderer[] AudienceSprites;
    public GameObject attackIcon;
    public GameObject shieldIcon;
    public GameObject starButton;
    public AudioSource playerMusic, enemyMusic;
    public GameObject tempVictoryPanel;
    public TextMeshProUGUI tempVictoryText;
    public RawImage playerLogoUI, enemyLogoUI;

    private List<GameObject> spawnedPlayers = new List<GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int currentPlayerIndex = 0;
    private bool canShowSelect = false;
    private bool alreadySelected = false;
    private int currentHype;
    private bool takeAnyActions = true;
    private int healthPlayer;
    private int healthEnemy;
    private HeartIconScript heartPlayer;
    private HeartIconScript heartEnemy;
    private int currentEnemyIndex = -1;
    private float enemyAITimer = 0f;
    private bool enemyInProgress = false;
    private List<GameObject> playerShields = new List<GameObject>();
    private List<GameObject> enemyShields = new List<GameObject>();
    private int playerBonus;
    private int playerStars;
    private int enemyStars;
    private List<GameObject> starIcons = new List<GameObject>();
    private bool isMusicIntro = true;
    int indexToForce = -1;
    // Start is called before the first frame update
    void Start()
    {
        BattleMessage[] bm = FindObjectsOfType<BattleMessage>();
        foreach (BattleMessage battleMessage in bm){
            FindBattle(battleMessage.message);
            Destroy(battleMessage.gameObject);
        }

        foreach (MusicianData md in battleData.playerTeam){
            GameObject go = Instantiate<GameObject>(md.gameObject);
            spawnedPlayers.Add(go);
            go.GetComponent<MusicianData>().startTime = battleData.startTime;
            go.GetComponent<MusicianData>().bpm = battleData.musicBPM;
            go.GetComponent<MusicianData>().musicBox = playerMusic;
        }
        PlaceTeam(spawnedPlayers, true);

        foreach (MusicianData md in battleData.enemyTeam){
            GameObject go = Instantiate<GameObject>(md.gameObject);
            spawnedEnemies.Add(go);
            go.GetComponent<MusicianData>().startTime = battleData.startTime;
            go.GetComponent<MusicianData>().bpm = battleData.musicBPM;
            go.GetComponent<MusicianData>().musicBox = playerMusic;
        }
        PlaceTeam(spawnedEnemies, false);

        hypePlayer.color = battleData.playerColor;
        hypeEnemy.color = battleData.enemyColor;
        hypeMeter.maxValue = battleData.totalHype;
        hypeMeter.value = battleData.startingHype;
        currentHype = battleData.startingHype;

        heartPlayer = SpawnHearts(battleData.playerHealth, true);
        healthPlayer = battleData.playerHealth;
        heartEnemy = SpawnHearts(battleData.enemyHealth, false);
        healthEnemy = battleData.enemyHealth;

        AssignPortraits();
        StartBattle();

        Background.sprite = battleData.backgroundSprite;

        for (int i = 0; i < AudienceSprites.Length; i++){
            AudienceSprites[i].sprite = battleData.audienceSprites[i];
        }

        playerMusic.clip = battleData.playerMusic[0];
        enemyMusic.clip = battleData.enemyMusic[0];
        playerMusic.loop = false;
        enemyMusic.loop = false;
        playerMusic.Play();
        enemyMusic.Play();

        if (battleData.playerLogo == null){
            playerLogoUI.texture = FindObjectOfType<TextureStorer>().storedTexture;
        } else {
            playerLogoUI.texture = battleData.playerLogo;
        }

        if (battleData.enemyLogo == null){
            enemyLogoUI.texture = FindObjectOfType<TextureStorer>().storedTexture;
        } else {
            enemyLogoUI.texture = battleData.enemyLogo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float score = GetScore();
        playerMusic.volume = (score+1)*TextBoxControl.musicVolume/2f;
        enemyMusic.volume = (1-score)*TextBoxControl.musicVolume/2f;
        if (isMusicIntro && !playerMusic.isPlaying){
            isMusicIntro = false;

            playerMusic.clip = battleData.playerMusic[1];
            enemyMusic.clip = battleData.enemyMusic[1];
            playerMusic.loop = true;
            enemyMusic.loop = true;
            playerMusic.Play();
            enemyMusic.Play();
        }

        if (!takeAnyActions){
            return;
        }

        if (canShowSelect){
            while (indexToForce > -1 && indexToForce != currentPlayerIndex){
                ProgressTurn();
            }
            indexToForce = -1;
            //Debug.Log("index to force: " + indexToForce + ", current player index: " + currentPlayerIndex);
            selectUI.SetActive(true);
            spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().SetBonusStars(playerBonus);
            for (int i = 0; i < ButtonTexts.Length; i++){
                TextMeshProUGUI buttonText = ButtonTexts[i];
                string str = buttonText.text.Split(" ")[0];
                str = str.Split("x")[0];
                int stat = spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().GetStat(i);
                string padding = "   ";
                if (stat/10 != 0){
                    padding += " ";
                }
                if (stat/100 != 0){
                    padding += " ";
                }

                buttonText.text = str + padding + "x" + stat;
            }
        } else {
            selectUI.SetActive(false);
            if (spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().IsNextTurn()){
                ProgressTurn();
            }
        }

        hypeMeter.value = currentHype;

        EnemyAI();

        if (currentHype == battleData.totalHype || healthEnemy <= 0){
            StartCoroutine("WinBattle");
            takeAnyActions = false;
        } else if (currentHype == 0 || healthPlayer <= 0){
            StartCoroutine("LoseBattle");
            takeAnyActions = false;
        }

        //Debug.Log("bonus: " + playerBonus + ", " + playerStars);
        //add health when that's a factor...
    }

    public void ProgressTurn(){
        //Debug.Log("skipping turn " + currentPlayerIndex);
        canShowSelect = true;
        spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().ResetTurn();
        currentPlayerIndex = NextPlayerIndex(currentPlayerIndex);
        //Debug.Log("skipping turn " + currentPlayerIndex);
        AssignPortraits();
        alreadySelected = false;
        ResetStars();
        SpawnStars();
    }

    void EnemyAI(){
        if (currentEnemyIndex < 0){
            return;
        }
        
        MusicianData enemyData = spawnedEnemies[currentEnemyIndex].GetComponent<MusicianData>();

        if (enemyData.IsNextTurn()){
            enemyInProgress = false;
            enemyData.ResetTurn();
            currentEnemyIndex = NextEnemyIndex(currentEnemyIndex);
            return;
        }

        if (enemyAITimer < battleData.AIDelay){
            enemyAITimer += Time.deltaTime;
            return;
        }

        if (enemyInProgress){
            return;
        }

        int starsUsed = (int)(Mathf.Ceil(enemyStars*enemyData.AIUseStars));
        enemyStars -= starsUsed;
        enemyData.SetBonusStars(starsUsed);

        float totalStats = 0f;
        foreach (float fl in enemyData.AI){
            totalStats += fl;
        }

        float chosen = Random.Range(0f, totalStats);
        int chosenIndex = 0;

        for (int i = 0; i < enemyData.AI.Length; i++){
            if (chosen < enemyData.AI[i]){
                chosenIndex = i;
                break;
            } else {
                chosen -= enemyData.AI[i];
            }
        }

        if (chosenIndex == 0){
            enemyData.StartCharm(this,true);
        } else if (chosenIndex == 1){
            enemyData.StartHelp(this, true);
        } else if (chosenIndex == 2){
            enemyData.StartAttack(attackIcon, this, true);
        } else {
            enemyData.StartDefend(this, true);
        }

        enemyInProgress = true;
    }

    IEnumerator WinBattle(){
        playerMusic.clip = battleData.playerMusic[2];
        enemyMusic.clip = battleData.enemyMusic[2];
        playerMusic.loop = false;
        enemyMusic.loop = false;
        playerMusic.Play();
        enemyMusic.Play();
        yield return new WaitForSeconds(0.1f);
        while (playerMusic.isPlaying || enemyMusic.isPlaying){
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        if (battleData.winGoTo.Length == 0){
            tempVictoryPanel.SetActive(true);
            tempVictoryText.text = "You Win!";
        } else {
            SummonBattleMessage(battleData.winGoTo);
        }
    }

    IEnumerator LoseBattle(){
        playerMusic.clip = battleData.playerMusic[2];
        enemyMusic.clip = battleData.enemyMusic[2];
        playerMusic.loop = false;
        enemyMusic.loop = false;
        playerMusic.Play();
        enemyMusic.Play();
        yield return new WaitForSeconds(0.1f);
        while (playerMusic.isPlaying || enemyMusic.isPlaying){
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        if (battleData.loseGoTo.Length == 0){
            tempVictoryPanel.SetActive(true);
            tempVictoryText.text = "You Lose!";
        } else {
            SummonBattleMessage(battleData.loseGoTo);
        }
    }

    void SummonBattleMessage(string message){
        GameObject go = new GameObject("Battle Message");
        go.AddComponent<BattleMessage>().message = message;
        SceneManager.LoadScene("VisualNovelScene");
    }

    public void AssignPortraits(){
        for (int i = 0; i < BattlePortraits.Length; i++){
            BattlePortraits[i].sprite = spawnedPlayers[NextPlayerIndex(currentPlayerIndex, i)].GetComponent<MusicianData>().portrait;
        }
    }

    public void DealDamage(int amount, bool isEnemy){
        if (isEnemy){
            healthEnemy -= amount;
            for (int i = 0; i < amount; i++){
                if (heartEnemy == null){
                    break;
                }
                heartEnemy = heartEnemy.Remove();
            }
        } else {
            healthPlayer -= amount;
            for (int i = 0; i < amount; i++){
                if (heartPlayer == null){
                    break;
                }
                heartPlayer = heartPlayer.Remove();
            }
        }
    }

    public void AddHype(int amount){
        currentHype += amount;
        if (currentHype > battleData.totalHype){
            currentHype = battleData.totalHype;
        }
        if (currentHype < 0){
            currentHype = 0;
        }
    }

    public void Charm(){
        if (alreadySelected){
            return;
        }
        spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().StartCharm(this, false);
        StartCoroutine("TurnOffMenu");
    }

    public void Help(){
        if (alreadySelected){
            return;
        }
        spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().StartHelp(this, false);
        StartCoroutine("TurnOffMenu");
    }

    public void Attack(){
        if (alreadySelected){
            return;
        }
        spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().StartAttack(attackIcon, this, false);   
        StartCoroutine("TurnOffMenu");     
    }

    public void Defend(){
        if (alreadySelected){
            return;
        }
        spawnedPlayers[currentPlayerIndex].GetComponent<MusicianData>().StartDefend(this, false);
        StartCoroutine("TurnOffMenu");
    }

    void StartBattle()
    {
        // Start function WaitAndPrint as a coroutine
        StartCoroutine("StartBattleCO");
    }

    IEnumerator StartBattleCO()
    {
        yield return new WaitForSeconds(battleData.delayTime);
        canShowSelect = true;
        currentEnemyIndex = 0;
    }

    IEnumerator TurnOffMenu()
    {
        alreadySelected = true;
        yield return new WaitForSeconds(0.2f);
        playerStars -= playerBonus;
        playerBonus = 0;
        enemyAITimer = (enemyAITimer+battleData.AIDelay)/2.0f;
        canShowSelect = false;
    }

    int NextPlayerIndex(int num){
        int n = num+1;
        if (n >= spawnedPlayers.Count){
            n = 0;
        }
        return n;
    }

    int NextPlayerIndex(int num, int amount){
        int n = num;
        for (int i = 0; i < amount; i++){
            n = NextPlayerIndex(n);
        }
        return n;
    }

    int NextEnemyIndex(int num){
        int n = num+1;
        if (n >= spawnedEnemies.Count){
            n = 0;
        }
        return n;
    }

    int NextEnemyIndex(int num, int amount){
        int n = num;
        for (int i = 0; i < amount; i++){
            n = NextPlayerIndex(n);
        }
        return n;
    }

    void PlaceTeam(List<GameObject> team, bool isLeft){
        int scalar = 1;
        if (isLeft){
            scalar = -1;
        }

        float originX = 4.515f*scalar;

        float oddPad = 0f;
        float evenPad = 0f;
        for (int i = 1; i < team.Count; i++){
            int mult = 1;
            if (i < team.Count-2){
                mult = 2;
            }
            float pad = team[i].GetComponent<MusicianData>().horizontalBuffer*mult;
            if (i % 2 == 0){
                evenPad += pad;
            } else {
                oddPad += pad;
            }
        }

        float padDiff = (evenPad-oddPad)/2f;
        originX -= padDiff*scalar;

        for (int i = 0; i < team.Count; i++){
            float yPos = battleData.stageHeight+team[i].GetComponent<MusicianData>().verticalOffset;

            if (i == 0){
                team[i].transform.position = new Vector3(originX,yPos, 0f);
            } else {
                int direction = 1;
                if (i % 2 == 1){
                    direction = -1;
                }
                GameObject prev = team[Mathf.Max(i-2,0)];
                float xPos = prev.transform.position.x + ((prev.GetComponent<MusicianData>().horizontalBuffer+team[i].GetComponent<MusicianData>().horizontalBuffer)*direction*scalar);

                team[i].transform.position = new Vector3(xPos,yPos, 0f);
            }

            team[i].GetComponent<SpriteRenderer>().sortingOrder = (team.Count-1-i);
        }

    }

    HeartIconScript SpawnHearts(int number, bool isLeft){
        int scalar = 1;
        if (isLeft){
            scalar = -1;
        }

        float vertical = 3.0f;
        float originX = 4.515f*scalar;
        float xGap = 0.53f;

        int totalHP = number;

        HeartIconScript prevTemp = null;

        while (totalHP > 10){
            for (int i = 0; i < 10; i++){
                float xOffset = (5-i)*scalar*xGap;
                GameObject go = Instantiate<GameObject>(heartIcon);
                go.GetComponent<HeartIconScript>().previous = prevTemp;
                prevTemp = go.GetComponent<HeartIconScript>();
                go.transform.position = new Vector3(originX + xOffset, vertical, 0f);
            }

            totalHP -= 10;
            originX += xGap/2f;
            vertical -= xGap*0.75f;
        }

        if (totalHP > 0){
            for (int i = 0; i < totalHP; i++){
                float xOffset = ((totalHP/2)-i)*scalar*xGap;
                GameObject go = Instantiate<GameObject>(heartIcon);
                go.GetComponent<HeartIconScript>().previous = prevTemp;
                prevTemp = go.GetComponent<HeartIconScript>();
                go.transform.position = new Vector3(originX + xOffset, vertical, 0f);
            }
        }

        return prevTemp;
    }

    public void SpawnShield(bool isEnemy){
        int row = 9;

        List<GameObject> shieldList = playerShields;
        if (isEnemy){
            shieldList = enemyShields;
        }

        if (shieldList.Count >= row*1){
            return;
        }

        GameObject go = Instantiate<GameObject>(shieldIcon);
        go.GetComponent<SpriteRenderer>().flipX = isEnemy;

        int scalar = 1;
        if (isEnemy){
            scalar = -1;
        }

        float xPos = -1.2f*scalar+((shieldList.Count/row)*0.3f*scalar);
        float yPos = -0.7f+battleData.stageHeight+(shieldList.Count%row)*0.48f+((shieldList.Count/row)%2)*0.24f;
        go.transform.position = new Vector3(xPos,yPos,0f);
        shieldList.Add(go);
    }

    public void RemoveShield(GameObject shield, bool isEnemy){
        List<GameObject> shieldList = playerShields;
        if (!isEnemy){
            shieldList = enemyShields;
        }

        shieldList.Remove(shield);
    }

    public GameObject GetShield(bool isEnemy){
        GameObject shield = null;

        List<GameObject> shieldList = playerShields;
        if (!isEnemy){
            shieldList = enemyShields;
        }

        if (shieldList.Count > 0){
            shield = shieldList[shieldList.Count-1];
        }

        return shield;
    }

    public void AddRemoveBonus(bool selected){
        if (selected){
            playerBonus++;
        } else {
            playerBonus--;
        }
    }

    public void AddBonusStars(int amount, bool isEnemy){
        if (isEnemy){
            enemyStars += amount;
        } else {
            playerStars += amount;
        }
        if (enemyStars > 10){
            enemyStars = 10;
        }
        if (playerStars > 10){
            playerStars = 10;
        }
    }

    public void SpawnStars(){
        for (int i = 0; i < playerStars; i++){
            GameObject go = Instantiate<GameObject>(starButton,selectUI.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(100f*((playerStars+1f)/2f-(i+1)),-320f);
            starIcons.Add(go);
        }
    }

    public void ResetStars(){
        foreach (GameObject go in starIcons){
            Destroy(go);
        }
        starIcons = new List<GameObject>();
    }

    public float GetScore(){
        float healthDiff = (1f/healthEnemy)-(1f/healthPlayer);
        float hypeDiff = (currentHype-(battleData.totalHype/2))/((float)(currentHype));

        float healthScore = healthDiff/10f;
        float hypeScore = hypeDiff*2f;
        float score = healthScore+hypeScore;

        score *= 2f;

        score = Mathf.Min(score, 1f);
        score = Mathf.Max(score, -1f);


        //Debug.Log("healthEnemy: " + healthEnemy + ", currentHype" + currentHype + "/" + battleData.totalHype);
        if (healthEnemy <= 0 || currentHype >= battleData.totalHype){
            score = 1f;
        } else if (healthPlayer <= 0 || currentHype <= 0){
            score = -1f;
        }

        return score;
    }

    void FindBattle(string battleName){
        if (battleData != null){
            return;
        }
        foreach (BattleData bData in allBattles){
            if (battleName == bData.battleName){
                battleData = bData;
                break;
            }
        }
    }

    public int GetCurrentHype(){
        return currentHype;
    }

    public void ForcePlayerIndex(int index){
        indexToForce = index;
    }
}
