using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using TMPro;

public class TextBoxControl : MonoBehaviour
{
    public TextAsset gameScript;
    public GameObject textBoxObject;
    public TextMeshProUGUI blockText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI autoplayStatus;
    public GameObject OptionSelectParent;
    public List<GameObject> optionsToSpawn;
    public Image left1, left2, right1, right2, background;
    public AudioSource musicSource;
    public NameTypeScript nameTypeScript;
    public TutorialController tutorialController;
    public NameTypeScript bandNameScript;
    public GameObject pauseMenu;
    public Camera renderTexCamera;

    public AudioSource debugSource;

    public static string[] fullScript;
    public static Dictionary<string, string> replacements;
    public static Dictionary<string, bool> gameData;
    public static Dictionary<string, float> recordTimes;
    public static bool autoplayEnabled = false;
    public static float musicVolume = 0.5f;
    public static float sfxVolume = 0.5f;
    public static List<string> adventureLog;
    public static float textSpeed = 2f;
    public static float autoplayDelay = 1.5f;
    public static string textSoundName;
    public static int textSoundMaxIndex;
    public static bool cleanMode;
    public static List<string> badWords;

    bool textBoxActive = false;
    float growSpeed = 2.5f;
    int currentLine = 0;
    string currentName;
    string nextBlockText;
    string currentBlockText;
    bool canPressMouse;
    bool canAutoplay = false;
    float autoplayBrokeTimer = 0f;
    float musicTrackVolume = 1f;
    GameObject options;
    SaveSerial saveSerial;
    GameObject typeSfxSource;
    bool didRenderTexture = false;
    bool didRenderArtTexture = false;

    // Start is called before the first frame update
    void Start()
    {
        if (debugSource != null){
            //debugSource.Play();
        }
        if (fullScript == null && gameScript != null){
            string content = gameScript.text;
            fullScript = content.Split("\n");
        }

        if (adventureLog == null){
            adventureLog = new List<string>();
        }

        if (replacements == null){
            replacements = new Dictionary<string, string>();
            replacements.Add("Maria", "Maria");
            replacements.Add("Pines Masters", "Pines Masters");
            replacements.Add("Kristophf", "Kristophf");
            replacements.Add(";", ",");
        }

        if (gameData == null){
            gameData = new Dictionary<string, bool>();
        }

        if (recordTimes == null){
            recordTimes = new Dictionary<string, float>();
        }

        if (badWords == null){
            badWords = new List<string>();
            badWords.Add("energy");
            badWords.Add("conquer");
            badWords.Add("visit");
            badWords.Add(" car ");
            badWords.Add(" car?");
            badWords.Add("breathe");
            badWords.Add("treat");
            badWords.Add("fuck");
            badWords.Add("play guitar");
            badWords.Add("string parts");
            badWords.Add("play");
            badWords.Add("hell ");
            badWords.Add("hell,");
            badWords.Add("hell.");
            badWords.Add("hell?");
            badWords.Add("chew");
            badWords.Add("stupid");
            badWords.Add("pocket");
            badWords.Add("pants");
            badWords.Add("pines");
            badWords.Add("clothes");
            badWords.Add("tummy");
            badWords.Add("tittie");
            badWords.Add("girls gone wild");
            badWords.Add("Girls Gone Wild");
            badWords.Add("boob");
            badWords.Add("bitch");
            badWords.Add("in bed");
            badWords.Add("omelette");
            badWords.Add("sexy");
            badWords.Add("sex");
            badWords.Add("step");
            badWords.Add("face");
            badWords.Add("cry");
            badWords.Add("damn");
            badWords.Add("paint");
            badWords.Add("vanquish");
            badWords.Add("frat party");
            badWords.Add("kick");
            badWords.Add("butt");
            badWords.Add("shit");
            badWords.Add("piss");
            badWords.Add("idiot");
            badWords.Add("groupie");
            badWords.Add("bozo");
            badWords.Add("cunt");
            badWords.Add("piercing");
            badWords.Add(" ass,");
            badWords.Add("weirdo");
            badWords.Add("slut");
            badWords.Add("organ");
            badWords.Add("trash");
            badWords.Add("marri");
            badWords.Add("practice");
            badWords.Add("nike");
        }

        saveSerial = FindObjectOfType<SaveSerial>();

        BattleMessage[] bm = FindObjectsOfType<BattleMessage>();
        if (bm.Length > 0){
            foreach (BattleMessage battleMessage in bm){
                if (battleMessage.message == "LOAD DATA" && saveSerial != null){
                    //Debug.Log("ready to load");
                    saveSerial.LoadGame();
                } else {
                    GoToLine(battleMessage.message);
                }
                Destroy(battleMessage.gameObject);
            }
        } else {
            ExecuteLine(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Screen.fullScreen){
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        } else {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }*/

        if (textBoxActive){
            if (textBoxObject.GetComponent<Image>().color.a < 1f){
                Color c = textBoxObject.GetComponent<Image>().color;
                textBoxObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, Mathf.Min(c.a+Time.deltaTime*growSpeed,1f));
            }
        } else {
            if (textBoxObject.GetComponent<Image>().color.a > 0f){
                Color c = textBoxObject.GetComponent<Image>().color;
                textBoxObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, Mathf.Max(c.a-Time.deltaTime*growSpeed,0f));
            }
        }
        float scale = textBoxObject.GetComponent<Image>().color.a;
        textBoxObject.transform.localScale = new Vector3(scale, Mathf.Min(scale*3f, 1f), scale);

        if (Input.GetAxis("Submit") <= 0f){
            canPressMouse = true;
        }

        if (scale >= 1f){
            nameText.text = currentName;
            blockText.text = currentBlockText;

            if (currentBlockText.Length == 0 && currentBlockText.Length < nextBlockText.Length){
                StartCoroutine(AdvanceText());
            }
            if (autoplayEnabled){
                if (currentBlockText.Length < nextBlockText.Length){
                    canAutoplay = true;
                    autoplayBrokeTimer = 0f;
                    StopCoroutine("AutoplayNext");
                } else if (canAutoplay && currentBlockText.Length > 0 && currentBlockText.Length >= nextBlockText.Length){
                    canAutoplay = false;
                    StopCoroutine("AutoplayNext");
                    StartCoroutine("AutoplayNext");
                    autoplayBrokeTimer = 0f;
                } else if (currentBlockText.Length > 0 && currentBlockText.Length >= nextBlockText.Length){
                    autoplayBrokeTimer += Time.deltaTime;
                    if (autoplayBrokeTimer > CalcAutoplay()*1.1f){
                        StopCoroutine("AutoplayNext");
                        canAutoplay = true;
                    }
                }
            }

            if (canPressMouse && Input.GetAxis("Submit") > 0f && fullScript[currentLine][0] != '>'){
                canPressMouse = false;
                if (currentBlockText.Length < nextBlockText.Length){
                    currentBlockText = nextBlockText;
                    if (autoplayEnabled){
                        StopCoroutine("AutoplayNext");
                        StartCoroutine("AutoplayNext");
                    }
                } else {
                    StopCoroutine("AutoplayNext");
                    ExecuteLine();
                }
            }
        } else {
            nameText.text = "";
            blockText.text = "";
        }

        if (autoplayEnabled){
            autoplayStatus.text = "Autoplay - ON";
        } else {
            autoplayStatus.text = "Autoplay - OFF";
        }

        while (adventureLog.Count > 100){
            adventureLog.RemoveAt(0);
        }
        if (musicSource != null){
            musicSource.volume = musicVolume*musicTrackVolume;
        }

    }

    IEnumerator AdvanceText(){
        char nextChar = nextBlockText[currentBlockText.Length];
        currentBlockText += nextChar;
        if (nextChar == '.' || nextChar == '!' || nextChar == '?'){
            yield return new WaitForSeconds(0.15f/textSpeed);
        } else if (nextChar == ','){
            yield return new WaitForSeconds(0.1f/textSpeed);
        } else {
            yield return new WaitForSeconds(0.05f/textSpeed);
        }
        if (nextChar != ' '){
            StartCoroutine(PLAYTYPESFX());
        }
        if (currentBlockText.Length < nextBlockText.Length){
            StartCoroutine(AdvanceText());
        }
    }

    IEnumerator AutoplayNext(){
        yield return new WaitForSeconds(CalcAutoplay());
        if (autoplayEnabled && fullScript[currentLine].Length > 0 && fullScript[currentLine][0] != '>'){
            ExecuteLine();
        }
    }

    float CalcAutoplay(){
        return autoplayDelay*Mathf.Pow(nextBlockText.Length,0.125f);
    }

    public void ToggleAutoplay(){
        autoplayEnabled = !autoplayEnabled;
    }

    public void PauseGame(){
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        pauseMenu.GetComponent<PauseBehaviorScript>().SetSliders();
    }

    void ExecuteLine(){
        currentLine++;
        ExecuteLine(currentLine);
    }

    void ExecuteLine(int lineNum){
        currentLine = lineNum;
        string lineEx = fullScript[lineNum];
        ExecuteLine(lineEx);
    }

    void ExecuteLine(string lineEx){
        if (lineEx.Length == 0){
            return;
        } else if (lineEx[0] == '@'){
            ExecuteLine();
        } else if (lineEx[0] == '>'){

            string[] parameters = lineEx.Split(", ");
            string funcName = parameters[0].Substring(1);

            string[] parms = new string[parameters.Length-1];
            for (int i = 0; i < parms.Length; i++){
                parms[i] = parameters[i+1];
            }

            StartCoroutine(funcName, parms);
        } else {
            lineEx = ReplaceText(lineEx);
            if (cleanMode){
                lineEx = CensorLine(lineEx);
            }
            adventureLog.Add(lineEx);
            currentName = lineEx.Split(": ")[0];
            int startIndex = currentName.Length+2;
            if (startIndex > lineEx.Length){
                startIndex = 0;
            }
            nextBlockText = lineEx.Substring(startIndex);
            nextBlockText = LineBreakpoints(nextBlockText);
            currentBlockText = "";
            StartCoroutine(Save());
        }
    }

    IEnumerator Save(){
        yield return new WaitForSeconds(0.05f);
        if (saveSerial != null){
            saveSerial.SaveGame();
            Resources.UnloadUnusedAssets();
        }
    }

    IEnumerator OPEN(string[] parms){
        textBoxActive = true;
        currentName = "";
        nextBlockText = "";
        currentBlockText = "";

        yield return new WaitForSeconds(0.5f);

        ExecuteLine();
    }

    IEnumerator CLOSE(string[] parms){
        textBoxActive = false;

        yield return new WaitForSeconds(0.5f);
        ExecuteLine();
    }

    IEnumerator GOTO(string[] parms){
        string toFind = "@" + parms[0];
        int index = System.Array.IndexOf(fullScript, toFind);
        if (index < 0){
            index = 0;
        }

        yield return new WaitForSeconds(0f);
        ExecuteLine(index);
    }

    IEnumerator PAUSE(string[] parms){
        int milliseconds = int.Parse(parms[0]);
        //Debug.Log("Pause, " + parms[0]);

        yield return new WaitForSeconds(milliseconds/1000f);

        ExecuteLine();
    }

    IEnumerator BATTLE(string[] parms){
        yield return new WaitForSeconds(0f);

        GameObject go = new GameObject("Battle Message");
        go.AddComponent<BattleMessage>().message = parms[0];
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator OPENTUTORIAL(string[] parms){
        yield return new WaitForSeconds(0f);

        GameObject go = new GameObject("Battle Message");
        go.AddComponent<BattleMessage>().message = parms[0];

        SceneManager.LoadScene("TutorialScene");
    }

    IEnumerator SETDATA(string[] parms){
        yield return new WaitForSeconds(0f);

        bool toSet = true;
        if (parms[1][0] == '0'){
            toSet = false;
        }

        SetData(parms[0],toSet);
        ExecuteLine();
    }

    IEnumerator GOTOBOOL(string[] parms){
        yield return new WaitForSeconds(0f);

        bool canGo = true;
        for (int i = 1; i < parms.Length; i++){
            if (!GetData(parms[i])){
                canGo = false;
            }
        }

        if (canGo){
            StartCoroutine(GOTO(parms));
        } else {
            ExecuteLine();
        }
    }

    IEnumerator OPTIONS(string[] parms){
        yield return new WaitForSeconds(0f);

        Destroy(options);

        foreach (GameObject go in optionsToSpawn){
            if (go.name == parms[0]){
                options = Instantiate<GameObject>(go, OptionSelectParent.transform);
            }
        }
        ExecuteLine();
    }

    IEnumerator CLEAROPTIONS(string[] parms){
        yield return new WaitForSeconds(0f);

        Transform[] ts = OptionSelectParent.GetComponentsInChildren<Transform>();

        foreach (Transform trns in ts){
            if (trns != null){
                Destroy(trns.gameObject);
            }
        }
        ExecuteLine();
    }

    IEnumerator CHARACTER(string[] parms){
        yield return new WaitForSeconds(0f);

        int milliseconds = int.Parse(parms[2]);

        Image im = left1;

        if (parms[1] == "L1"){
            im = left1;
        } else if (parms[1] == "L2"){
            im = left2;
        } else if (parms[1] == "R1"){
            im = right1;
        } else if (parms[1] == "R2"){
            im = right2;
        }

        string suffix = "";
        if (cleanMode && parms[0].Contains("Felisha")){
            suffix = "_censored";
        }

        im.GetComponent<ImageDisplayer>().SwapSprite("Talksprites/" + parms[0] + suffix, milliseconds/1000f);

        ExecuteLine();
    }

    IEnumerator BACKGROUND(string[] parms){
        yield return new WaitForSeconds(0f);

        int milliseconds = int.Parse(parms[1]);

        background.GetComponent<ImageDisplayer>().SwapSprite("Backgrounds/" + parms[0], milliseconds/1000f);
        
        ExecuteLine();
    }

    IEnumerator SCREENSHAKE(string[] parms){
        yield return new WaitForSeconds(0f);

        int milliseconds = int.Parse(parms[0]);
        int force = int.Parse(parms[1]);

        ShakeObject[] shakeObjects = FindObjectsOfType<ShakeObject>();

        foreach (ShakeObject so in shakeObjects){
            so.Shake(milliseconds/1000f, force/1000f);
        }
        
        ExecuteLine();
    }

    IEnumerator MUSIC(string[] parms){
        yield return new WaitForSeconds(0f);

        musicTrackVolume = 1f;
        if (parms.Length > 1){
            musicTrackVolume = int.Parse(parms[1])/1000f;
        }

        AudioClip clip = Resources.Load<AudioClip>("Music/" + parms[0]);

        musicSource.clip = clip;
        musicSource.pitch = 1f;
        musicSource.volume = musicVolume*musicTrackVolume;
        musicSource.Play();
        
        ExecuteLine();
    }

    IEnumerator SFX(string[] parms){
        yield return new WaitForSeconds(0f);

        float volumeMult = 1f;
        if (parms.Length > 1){
            volumeMult = int.Parse(parms[1])/1000f;
        }

        AudioClip clip = Resources.Load<AudioClip>("SFX/" + parms[0]);

        GameObject sfxSource = new GameObject();
        //sfxSource.transform.position = musicSource.transform.position;
        sfxSource.AddComponent<AudioSource>().clip = clip;
        sfxSource.GetComponent<AudioSource>().volume = sfxVolume*volumeMult;
        sfxSource.GetComponent<AudioSource>().Play();
        sfxSource.AddComponent<DestroyAudioSource>();
        
        ExecuteLine();
    }

    IEnumerator PLAYTYPESFX(){
        yield return new WaitForSeconds(0f);

        int n = Random.Range(0,textSoundMaxIndex+1);

        float volumeMult = Random.Range(0.8f, 1.2f);
        float pitchMult = Random.Range(0.8f, 1.2f);

        AudioClip clip = Resources.Load<AudioClip>("SFX/" + textSoundName + "_" + n);

        if (typeSfxSource == null){
            typeSfxSource = new GameObject();
            typeSfxSource.AddComponent<AudioSource>();
        }
        if (typeSfxSource.GetComponent<AudioSource>().clip == null || !typeSfxSource.GetComponent<AudioSource>().isPlaying || typeSfxSource.GetComponent<AudioSource>().time > 0.1f){
            //sfxSource.transform.position = musicSource.transform.position;
            typeSfxSource.GetComponent<AudioSource>().Stop();
            typeSfxSource.GetComponent<AudioSource>().clip = clip;
            typeSfxSource.GetComponent<AudioSource>().volume = sfxVolume*volumeMult*0.25f;
            typeSfxSource.GetComponent<AudioSource>().pitch = pitchMult;
            typeSfxSource.GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator CHANGENAME(string[] parms){
        yield return new WaitForSeconds(0f);

        nameTypeScript.Open(parms[0], parms[1], parms[2], "Talksprites/" + parms[3], int.Parse(parms[4]));

        ExecuteLine();
    }

    IEnumerator BANDNAME(string[] parms){
        yield return new WaitForSeconds(0f);

        bandNameScript.Open(parms[0], parms[1], parms[2], int.Parse(parms[3]));

        ExecuteLine();
    }

    IEnumerator TUTORIALSET(string[] parms){
        yield return new WaitForSeconds(0f);

        tutorialController.ChangeSettings(parms[0], parms[1] == "true", parms[2] == "true", parms[3] == "true", parms[4] == "true", int.Parse(parms[5])/1000f, parms[6] == "true");

        ExecuteLine();
    }

    IEnumerator MAINMENU(string[] parms){
        yield return new WaitForSeconds(0f);

        TextureStorer storer = FindObjectOfType<TextureStorer>();
        if (storer.storedTextureOld == null){
            storer.storedTextureOld = new Texture2D(512, 512);
        }
        Graphics.CopyTexture(storer.storedTexture, storer.storedTextureOld);

        if (saveSerial != null){
            saveSerial.SaveGameStart();
            Resources.UnloadUnusedAssets();
        }

        SceneManager.LoadScene("MainMenu");

        ExecuteLine();
    }

    IEnumerator TYPESFX(string[] parms){
        yield return new WaitForSeconds(0f);

        textSoundName = parms[0];
        textSoundMaxIndex = int.Parse(parms[1]);

        ExecuteLine();
    }

    IEnumerator FORCEBANDLOGO(string[] parms){

        bool shouldForce1 = false;
        bool shouldForce2 = parms.Length > 0;

        DeleteWithoutSave dws = FindObjectOfType<DeleteWithoutSave>();
        if (dws != null){
            GetStoredTexture gws = dws.GetComponent<GetStoredTexture>();
            TextureStorer storer = FindObjectOfType<TextureStorer>();
            Graphics.CopyTexture(storer.storedArt,gws.tex);
            gws.GetComponent<MeshRenderer>().material.mainTexture = gws.tex;
            shouldForce2 = false;
        }

        if (shouldForce1){
            Debug.Log("stage 1");
            ResetRenderedArtTexture();

            while (!GetRenderedArtTexture()){
                yield return new WaitForSeconds(0.1f);
            }
        }

        //Debug.Log("stage 2");

        //yield return new WaitForSeconds(5f);

        //Debug.Log("stage 3");

        yield return new WaitForSeconds(0.1f);

        if (shouldForce2){
            DrawingScript drawingScript = FindObjectOfType<DrawingScript>();

            if (renderTexCamera != null){
                renderTexCamera.gameObject.SetActive(true);
            }

            if (drawingScript != null){
                drawingScript.controlCamera = false;
            }

            while (!didRenderTexture){
                yield return new WaitForSeconds(0.1f);
            }

            if (renderTexCamera != null){
                renderTexCamera.gameObject.SetActive(false);
            }

            if (drawingScript != null){
                drawingScript.controlCamera = true;
            }

        }

        //Debug.Log("stage 4");

        //yield return new WaitForSeconds(5f);

        //Debug.Log("stage 5");

        ExecuteLine();
    }

    IEnumerator CLEANLINE(string[] parms){
        yield return new WaitForSeconds(0f);
        
        if (cleanMode){
            fullScript[currentLine+1] = parms[0];
        }

        ExecuteLine();
    }

    string ReplaceText(string original){
        int indexHigh = original.IndexOf("]");
        int indexLow = indexHigh;
        for (int i = indexHigh; i >= 0; i--){
            if (original[i] == '['){
                indexLow = i;
                break;
            }
        }

        while (indexHigh > indexLow){
            string toReplace = original.Substring(indexLow+1, indexHigh-indexLow-1);
            string[] parms = toReplace.Split(", ");
            if (indexLow < 0){
                indexLow = 0;
            }
            original = original.Substring(0, indexLow) + GetReplacement(parms) + original.Substring(indexHigh+1);

            indexHigh = original.IndexOf("]");
            indexLow = original.IndexOf("[");
        }

        return original;
    }

    string GetReplacement(string[] parms){
        string swap = ReplaceText(parms[0]);
        if (replacements != null && replacements.ContainsKey(swap)){
            swap = replacements[swap];
        }
        if (parms.Length > 1){
            string[] parms2 = new string[parms.Length-1];
            parms2[0] = swap;
            for (int i = 1; i < parms2.Length; i++){
                parms2[i] = parms[i+1];
            }

            string funcName = parms[1];

            MethodInfo mInfo = typeof(TextBoxControl).GetMethod(funcName);
            swap = (string)mInfo.Invoke(this, new object[] {parms2});
        }
        return swap;
    }

    string LineBreakpoints(string str){
        int lineLen = 64;

        int charIndex = 0;
        int loops = 0;
        while (charIndex + lineLen+1 < str.Length){
            for (int i = charIndex + lineLen+1; i > charIndex; i--){
                if (str[i] == ' '){
                    str = str.Substring(0,i) + "\n" + str.Substring(i+1);
                    charIndex = i;
                    break;
                }
            }
            loops++;
            if (loops > 10000){
                break;
            }
        }
        return str;
    }

    public string CAPS(string[] parms){
        return parms[0].ToUpper();
    }

    public string CHOPCON(string[] parms){
        string chopped = parms[0];
        //string chopped = parms[0].ToUpper();
        while (!isVowel(chopped[0])){
            chopped = chopped.Substring(1);
        }
        return chopped;
    }

    public string SUBSTRING(string[] parms){
        int index = int.Parse(parms[1]);
        int length = int.Parse(parms[2]);
        return parms[0].Substring(index, length);
    }

    public string CensorLine(string lineIn){
        foreach (string badWord in badWords){
            int wordIndex = lineIn.ToLower().IndexOf(badWord);
            while (wordIndex >= 0){
                string toReplace = GenerateGrawlixes(badWord);
                lineIn = lineIn.Substring(0,wordIndex) + toReplace + lineIn.Substring(wordIndex+badWord.Length);
                wordIndex = lineIn.ToLower().IndexOf(badWord);
            }
        }

        return lineIn;
    }

    public string GenerateGrawlixes(string badWord){
        char[] replaceChar = new char[]{'#','@','!','$','%','&','*'};

        List<char> weightedChars = new List<char>();
        for (int i = 0; i < badWord.Length*4/replaceChar.Length; i++){
            foreach (char c in replaceChar){
                weightedChars.Add(c);
            }
        }

        string toReplace = "";
        for (int i = 0; i < badWord.Length; i++){
            if (badWord[i] == ' ' || badWord[i] == ',' || badWord[i] == '.' || badWord[i] == '?'){
                toReplace += badWord[i];
            } else {
                int index = Random.Range(0,weightedChars.Count);
                char c = weightedChars[index];
                weightedChars.RemoveAt(index);
                toReplace += c;
            }
        }
        return toReplace;
    }

    bool isVowel (char c){
        if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U'){
            return true;
        }
        return false;
    }

    public static bool GetData(string key){
        if (gameData == null || !gameData.ContainsKey(key)){
            return false;
        }
        return gameData[key];
    }

    public static string GetRecordTime(string key){
        if (!recordTimes.ContainsKey(key)){
            return "99:99.99";
        }
        int intTime = (int)(recordTimes[key]);
        int minutes = intTime/60;
        int seconds = intTime%60;
        int mult100 = (int)(recordTimes[key]*100);
        int decimals = mult100%100;
        return "" + minutes + ":" + seconds + "." + decimals;
    }

    public void SetData(string key, bool data){
        if (!gameData.ContainsKey(key)){
            gameData.Add(key, data);
        } else {
            gameData[key] = data;
        }
    }

    public bool CanInteract(){
        if (!textBoxActive && fullScript[currentLine].Length == 0){
            return true;
        }
        return false;
    }

    public void GoToLine(string line){
        StartCoroutine(GOTO(new string[]{line}));
    }

    public int GetCurrentLine(){
        return currentLine;
    }

    public void LoadData(int currentLine, bool autoplayEnabled, string currentBackground, string currentL1, string currentL2, string currentR1,
        string currentR2, string currentMusic, float currentMusicTrackVolume, float musicVolume, float sfxVolume, float textSpeed, float autoplayDelay, List<string> adventureLog,
        Dictionary<string, string> replacements, Dictionary<string, bool> gameData, string optionsLoaded){
            //Debug.Log("text box loading");

            TextBoxControl.autoplayEnabled = autoplayEnabled;
            TextBoxControl.musicVolume = musicVolume;
            TextBoxControl.sfxVolume = sfxVolume;
            TextBoxControl.textSpeed = textSpeed;
            TextBoxControl.autoplayDelay = autoplayDelay;
            TextBoxControl.replacements = replacements;
            TextBoxControl.gameData = gameData;

            //Debug.Log("loaded statics");
            
            background.GetComponent<ImageDisplayer>().SwapSprite("Backgrounds/" + currentBackground, 0.5f);
            left1.GetComponent<ImageDisplayer>().SwapSprite("Talksprites/" + currentL1, 0.5f);
            left2.GetComponent<ImageDisplayer>().SwapSprite("Talksprites/" + currentL2, 0.5f);
            right1.GetComponent<ImageDisplayer>().SwapSprite("Talksprites/" + currentR1, 0.5f);
            right2.GetComponent<ImageDisplayer>().SwapSprite("Talksprites/" + currentR2, 0.5f);

            //Debug.Log("loaded sprites");

            musicSource.clip = Resources.Load<AudioClip>("Music/" + currentMusic);
            musicTrackVolume = currentMusicTrackVolume;
            musicSource.volume = musicVolume*musicTrackVolume;
            musicSource.Play();

            if (optionsLoaded.Length > 0){
                Destroy(options);

                foreach (GameObject go in optionsToSpawn){
                    if (go.name == optionsLoaded){
                        options = Instantiate<GameObject>(go, OptionSelectParent.transform);
                    }
                }
            }

            //Debug.Log("loaded options: " + options);

            this.currentLine = currentLine;
            StartCoroutine(FORCERENDERTEX());
    }

    public string GetOptionsLoaded(){
        if (options != null){
            return options.name.Split("(")[0];
        }
        return "";
    }

    IEnumerator OPENLOAD(int loadLine){
        //Debug.Log("open load");

        textBoxActive = true;
        currentName = "";
        nextBlockText = "";
        currentBlockText = "";

        yield return new WaitForSeconds(0.5f);

        ExecuteLine(loadLine);
    }

    public void SetRenderedTexture(){
        if (GetRenderedArtTexture()){
            didRenderTexture = true;
        }
    }

    public void SetRenderedArtTexture(){
        didRenderArtTexture = true;
    }

    public void ResetRenderedTexture(){
        didRenderTexture = false;
    }

    public void ResetRenderedArtTexture(){
        DeleteWithoutSave dws = FindObjectOfType<DeleteWithoutSave>();
        if (dws != null){
            Destroy(dws.GetComponent<GetStoredTexture>().tex);
            dws.GetComponent<GetStoredTexture>().tex = null;
            dws.GetComponent<GetStoredTexture>().SetShouldGetTexture();
        }
        didRenderArtTexture = false;
    }

    public bool GetRenderedTexture(){
        return didRenderTexture;
    }

    public bool GetRenderedArtTexture(){
        if (FindObjectOfType<DeleteWithoutSave>() == null){
            //Debug.Log("no delete without save?");
            return true;
        }
        return didRenderArtTexture;
    }

    IEnumerator FORCERENDERTEX(){
        bool shouldForce = false;
        bool shouldForce2 = false;

        if (shouldForce){
            Debug.Log("stage 1");
            didRenderArtTexture = false;

            while (!GetRenderedArtTexture()){
                yield return new WaitForSeconds(0.1f);
            }

            Debug.Log("stage 2");
        }

        if (shouldForce2){

            didRenderTexture = false;
            //yield return new WaitForSeconds(5f);

            Debug.Log("stage 3");

            yield return new WaitForSeconds(0.1f);

            DrawingScript drawingScript = FindObjectOfType<DrawingScript>();

            if (renderTexCamera != null){
                renderTexCamera.gameObject.SetActive(true);
            }

            if (drawingScript != null){
                drawingScript.controlCamera = false;
                drawingScript.GetComponent<NameTypeScript>().key = "Pines Masters";
            }

            while (!GetRenderedTexture()){
                yield return new WaitForSeconds(0.1f);
            }

            if (drawingScript != null){
                drawingScript.controlCamera = true;
            }

            if (renderTexCamera != null){
                renderTexCamera.gameObject.SetActive(false);
            }

            Debug.Log("stage 4");

            //yield return new WaitForSeconds(5f);

            //TextureStorer textureStorer = FindObjectOfType<TextureStorer>();
            //textureStorer.storedTexture = textureStorer.storedTextureOld;

            Debug.Log("stage 5");
        }

        StartCoroutine(OPENLOAD(currentLine));
    }
}
