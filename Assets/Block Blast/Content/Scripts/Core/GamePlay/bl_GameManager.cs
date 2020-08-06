using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.SceneManagement;

public class bl_GameManager : Singleton<bl_GameManager> {

    [Header("Global")]
    public GameMode m_GameMode = GameMode.PickUp;
    public List<bl_LevelInfo> Levels = new List<bl_LevelInfo>();
    [Header("Settings")]
    [Range(0, 1)] public float RateTimeScore = 0.25f;
    public Vector2 PerSecondPoints = new Vector2(3, 7);
    [Header("Audio")]
    public AudioClip pointSound;
    [SerializeField] private AudioClip progresivePointSound;
    public AudioClip[] musics;
    public AudioSource audioSource;
    [SerializeField] private AudioMixerSnapshot NormalSnap;
    [SerializeField] private AudioMixerSnapshot PauseSnap;
    [SerializeField] private AudioSource SfxSource;
    [SerializeField] private AudioSource VoiceSource;
    [Header("References")]
    public Transform player;
    public Material[] LevelMats;
    [SerializeField] private Text LevelText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text SubLevelText;
    [SerializeField] private Text ProgresiveScoreText;
    [SerializeField] private Image ScreenShotImage;
    [SerializeField] private Image FilledScoreImg;
    [HideInInspector] public Vector3 CacheDefaultPlayerScale;
    private Vector3 cacheDeafultPlayerPosition;
    [SerializeField] private GameObject[] GameModeScore;
    [SerializeField] private Animator ScorePAnim;
    [SerializeField] private Animator PauseAnim;

    private int _point = 0;
    private int PointByLevel = 0;
    private int totalPoints = 0;
    private int NextScoreByLevel = 0;
    private float prevRealTime;
    private float thisRealTime;
    private int CacheLastScore;
    private int CacheScore;
    private int PlayTimes;
    private int currentObstacleLevel = 0;
    private int progresiveScore = 0;
    private int CurrentCompleteLevel;
    private bool isPause = false;
    private Texture2D cacheScreenShot;
    private bool LowSettings = false;
    private float tutorialTime = 0;

    bool CR_Running = false;

    private bl_UIManager uiManager;
    AndroidJavaClass JavaClass2;

    public int point
	{
		set 
		{
			_point = value;
            CacheScore = point;

            if (!isGameOver) 
			{
                new BlockBlast.bl_GlobalEvents.OnPoint(value);
                SetPoint(value);
			}
		}
		get 
		{
			return _point;
		}
	}
    private bool isGameOver;
    private int lastLevel = 0;

	public float ScrollSpeed
	{
		get 
		{
			return GetLevelSpeed();
		}
	}

	void Awake()
	{
		isGameOver = true;
        LoadSettings();
        audioSource = GetComponent<AudioSource> ();
        uiManager = GetComponent <bl_UIManager>();
		Application.targetFrameRate = 60;
        PlayRandomMusic();
        PauseSnap.TransitionTo(1);
		RenderSettings.ambientLight = Color.white;
        FilledScoreImg.fillAmount = 0;
        LevelText.canvasRenderer.SetAlpha(0);
        SubLevelText.canvasRenderer.SetAlpha(0);
        CacheDefaultPlayerScale = player.transform.localScale;
        cacheDeafultPlayerPosition = player.localPosition;
        AddCoins(0);
    }

    void LoadSettings()
    {
        int gm = PlayerPrefs.GetInt(KeyMasters.GameMode, 1);
        m_GameMode = (GameMode)gm;
        LowSettings = BlockBlast.bl_Utils.PlayerPrefsX.GetBool(KeyMasters.Quality, true);
        bool aa = (PlayerPrefs.GetInt(KeyMasters.Sound, 1) == 1) ? true : false;
        AudioListener.volume = PlayerPrefs.GetFloat(KeyMasters.Volume, 1);
        if (!aa) { AudioListener.volume = 0; }
        ChangeQualityGame(LowSettings);

        foreach(GameObject g in GameModeScore) { g.SetActive(false); }
        GameModeScore[gm - 1].SetActive(true);

    }

    void SetPoint(int p)
    {
        if (PlayerPrefs.GetInt(KeyMasters.Sound, 1) == 1)
        {
            SfxSource.PlayOneShot(pointSound);
        }

        PointByLevel++;
        totalPoints++;
        float porc = (PointByLevel * 100) / NextScoreByLevel;
        // Debug.Log(porc + "% :" + PointByLevel + " / " + NextScoreByLevel);
        FilledScoreImg.fillAmount = porc / 100;
        bl_Showwave.Instance.Play("middle", Color.black);
        if (m_GameMode == GameMode.PickUp) {
            CheckClassicAchiviements();
        }
        if (HaveMoreLevels)
        {
            if (isNewLevel())
            {
                OnSpeedLevel();
            }
        }
        if (m_GameMode == GameMode.Free)
        {
            bl_Shaker.Instance.Do(0);
        }
        else if (m_GameMode == GameMode.PickUp) { bl_Shaker.Instance.Do(4); }
    }

    void CheckClassicAchiviements()
    {
        //#if !UNITY_EDITOR && UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
        if (totalPoints >= 11 && totalPoints < 21)
        {
            bl_PlayService.Instance.UnlockAchievement(PlayServiceKey.achievement_reached_1, 1000);
        }
        else if (totalPoints >= 21 && totalPoints < 31)
        {
            bl_PlayService.Instance.UnlockAchievement(PlayServiceKey.achievement_reached_2, 2000);
        }
        else if (totalPoints >= 31 && totalPoints < 41)
        {
            bl_PlayService.Instance.UnlockAchievement(PlayServiceKey.achievement_reached_3, 3000);
        }
        else if (totalPoints >= 41 && totalPoints < 51)
        {
            bl_PlayService.Instance.UnlockAchievement(PlayServiceKey.achievement_reached_4, 4000);
        }
        else if (totalPoints >= 51 && totalPoints < 55) 
        {
            bl_PlayService.Instance.UnlockAchievement(PlayServiceKey.achievement_reached_5, 5000);
        }
        else if (totalPoints >= 55)
        {
            bl_PlayService.Instance.IncrementAchievement(PlayServiceKey.achievement_reached_5, totalPoints * 5000);
        }
//#endif
    }

    public void SetProgresivePoint(int points = 5)
    {
        if (isGameOver)
            return;

        if (PlayerPrefs.GetInt(KeyMasters.Sound, 1) == 1)
        {
            SfxSource.PlayOneShot(progresivePointSound);
        }
        progresiveScore += points;
        ProgresiveScoreText.text = progresiveScore.ToString("000000");
        bl_Shaker.Instance.Do(0);
        ScorePAnim.GetComponent<Text>().text = progresiveScore.ToString("000000");
        ScorePAnim.Play("point", 0, 0);
        bl_Showwave.Instance.Play("little", Color.black);
    }

    void OnNewHighScore()
    {
        if (m_GameMode == GameMode.PickUp)
        {
            PlayerPrefs.SetInt(KeyMasters.BestScore, point);
        }
        else
        {
            PlayerPrefs.SetInt(KeyMasters.BestScoreProgresive, progresiveScore);
        }
        new BlockBlast.bl_GlobalEvents.OnNewHighScore(true);
    }


    void OnSpeedLevel()
    {
        bl_LevelInfo l = Levels[lastLevel];
        if (Levels[lastLevel].ObstaclesLevel > currentObstacleLevel)
        {
            currentObstacleLevel = Levels[lastLevel].ObstaclesLevel;
        }
        ShowLevelText(); 
        bl_SlowMotion.Instance.DoSlow(2, 0.1f,true);

        if (m_GameMode == GameMode.PickUp)
        {
            PointByLevel = 0;
            int last = GetLastScoreNeeded();
            NextScoreByLevel = GetNextScoreNeeded() - last;
        }

        //When reached a new level
        CurrentCompleteLevel++;
        bl_Stylish.Instance.ChangeStyle(CurrentCompleteLevel);
        if (l.isNewLevel)
        {
            
        }
        if (Levels[lastLevel].m_AudioClip != null)
        {
            VoiceSource.clip = Levels[lastLevel].m_AudioClip;
            VoiceSource.PlayDelayed(1);
        }
    }

    public void ShowLoaderboard()
    {
        //show your leaderboard code here

        if (m_GameMode == GameMode.PickUp) {
            bl_PlayService.Instance.ShowLeaderboard(PlayServiceKey.leaderboard_ranking_classic);
        } else if (m_GameMode == GameMode.Free) {
            bl_PlayService.Instance.ShowLeaderboard(PlayServiceKey.leaderboard_ranking_free);
        }
        
    }

    public void ShowAchievements () {

        if (m_GameMode == GameMode.PickUp) {
            bl_PlayService.Instance.ShowAchievements();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    void ShowLevelText(float hideIn = 3)
    {
        if (string.IsNullOrEmpty(Levels[lastLevel].SubName))
        {
            Image img;
            LevelText.CrossFadeAlpha(1, 2, true);
            LevelText.text = Levels[lastLevel].Name;
            SubLevelText.CrossFadeAlpha(0, 2, true);
        }
        else
        {
            LevelText.text = Levels[lastLevel].Name;
            LevelText.CrossFadeAlpha(1, 1.5f, true);
            SubLevelText.text = Levels[lastLevel].SubName;
            SubLevelText.CrossFadeAlpha(1, 2, true);
        }
        Invoke("HideLevelText", hideIn);
    }

    void HideLevelText() { LevelText.CrossFadeAlpha(0, 2, true); SubLevelText.CrossFadeAlpha(0, 1.5f, true); }


	public void StartGame()
    {
        NormalSnap.TransitionTo(1.5f);
        player.gameObject.SetActive(true);
        BlockBlast.bl_Event.Global.DispatchEvent(new BlockBlast.bl_GlobalEvents.OnStartPlay());
        bl_ChangerManager.Instance.Change(LevelSides.Down, false);

        //reset values
        _point = 0;
        PointByLevel = 0;
        progresiveScore = 0;

        VoiceSource.clip = Levels[0].m_AudioClip;
        VoiceSource.PlayDelayed(1);

        if (m_GameMode == GameMode.PickUp)
        {
            NextScoreByLevel = GetNextScoreNeeded();
        }
        else if (m_GameMode == GameMode.Free)
        {
            InvokeRepeating("ProgresiveScore", 1, RateTimeScore);
        }
        new BlockBlast.bl_GlobalEvents.OnPoint(0);
        isGameOver = false;
        bl_Stylish.Instance.ChangeStyle(0);
        PlayTimes++;
        ShowLevelText(1.5f);
        
        StopCoroutine("TakeScreenShot");
    }

    public void Pause()
    {
        isPause = !isPause;
        new BlockBlast.bl_GlobalEvents.OnPause(isPause);        
        if (isPause)
        {
            PauseSnap.TransitionTo(0.01f);
            PauseAnim.gameObject.SetActive(true);
            PauseAnim.SetBool("show", isPause);
        }
        else
        {
            PauseAnim.SetBool("show", isPause);
            StartCoroutine(BlockBlast.bl_Utils.AnimatorUtils.WaitAnimationLenghtForDesactive(PauseAnim));
            NormalSnap.TransitionTo(1.5f);
        }
        Time.timeScale = (isPause) ? 0 : 1;
    }


	public void GameOver()
    {
        if (isGameOver)
            return;

        StopAllCoroutines();
        if (isPause)
        {
            Pause();
        }
        PauseAnim.gameObject.SetActive(false);
        isGameOver = true;
        SetScores();
        bl_Showwave.Instance.Play("full", Color.white, bl_Showwave.Type.Full);
        //Per gamemode logic finish
        if (m_GameMode == GameMode.PickUp)
        {
        }
        else if (m_GameMode == GameMode.Free)
        {
            CancelInvoke("ProgresiveScore");
        }
        //Take Screen Shot
        ScreenShotImage.gameObject.SetActive(false);

        StartCoroutine("TakeScreenShot");
        
        PauseSnap.TransitionTo(1);
        new BlockBlast.bl_GlobalEvents.OnFailGame();
        SetLeaderboard();
        bl_Shaker.Instance.Do(1);
        Reset();
    }

    public void AddCoins (int amount) {

        int coins = PlayerPrefs.GetInt("COINS", 0) + amount;
        coinText.text = coins.ToString();
        PlayerPrefs.SetInt("COINS", coins);
    
    }

    public Texture2D LoadTexture()
    {
#if !UNITY_WEBPLAYER
        string str = "MyRecordImage.png";
        path = Application.persistentDataPath + "/" + str;
        Texture2D textured = new Texture2D(1, 1);
        if (File.Exists(path))
        {
            byte[] data = File.ReadAllBytes(path);
            textured.LoadImage(data);
        }
        return textured;
#else
        return null;
#endif
    }


    private bool takeInProgress = false;
#if !UNITY_WEBPLAYER
    string path = "";
#endif
    IEnumerator TakeScreenShot()
    {
        takeInProgress = true;
        yield return new WaitForSeconds(6);
        int width = Screen.width;
        int height = Screen.height;

        yield return new WaitForEndOfFrame();
        Texture2D textured = new Texture2D(width, height, TextureFormat.RGB24, true);
        textured.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
        textured.Apply();
        cacheScreenShot = textured;
        Sprite s = Sprite.Create(cacheScreenShot, new Rect(0, 0, cacheScreenShot.width, cacheScreenShot.height), new Vector2(0.5f, 0.5f));

        ScreenShotImage.sprite = s;
        ScreenShotImage.gameObject.SetActive(true);
        ScreenShotImage.canvasRenderer.SetAlpha(0);
        ScreenShotImage.CrossFadeAlpha(1, 0.5f, true);
        takeInProgress = false;

        byte[] dataToSave = cacheScreenShot.EncodeToPNG();
        string str = "MyImage.png";
        path = Application.persistentDataPath + "/" + str;
        File.WriteAllBytes(path, dataToSave);

    }

    public void ShareScreenshot()
    {

        if (takeInProgress) {
            return;
        }

        NativeShare nativeShare = new NativeShare();

        nativeShare.SetSubject("Block Blast");
#if UNITY_ANDROID
        nativeShare.SetText("https://play.google.com/store/apps/details?id=com.Redmoon.BlockBlast");
#endif

        nativeShare.AddFile(Application.persistentDataPath + "/" + "MyImage.png");

        nativeShare.Share();
        
    }

    void OnDisable()
    {
        if (m_GameMode == GameMode.Free)
        {
            CancelInvoke("ProgresiveScore");
        }
    }

    void Reset()
    {
        FilledScoreImg.fillAmount = 0;
        lastLevel = 0;
        _point = 0;
        PointByLevel = 0;
        CurrentCompleteLevel = 0;
        player.localPosition = cacheDeafultPlayerPosition;
    }

	void SetScores()
    {
        if (m_GameMode == GameMode.PickUp)
        {
            CacheScore = point;
            if (PlayTimes > 1)
            {
                PlayerPrefs.SetInt(KeyMasters.LastScore, CacheLastScore);
                CacheLastScore = point;
            }
            else
            {
                CacheLastScore = PlayerPrefs.GetInt(KeyMasters.LastScore, 0);
            }

            int best = PlayerPrefs.GetInt(KeyMasters.BestScore, 0);

            if (point > best)
            {
                OnNewHighScore();
            }
        }
        else if (m_GameMode == GameMode.Free)
        {
            CacheScore = progresiveScore;
            if (PlayTimes > 1)
            {
                PlayerPrefs.SetInt(KeyMasters.LastScoreProgresive, CacheLastScore);
                CacheLastScore = progresiveScore;
            }
            else
            {
                CacheLastScore = PlayerPrefs.GetInt(KeyMasters.LastScoreProgresive, 0);
            }

            int best = PlayerPrefs.GetInt(KeyMasters.BestScoreProgresive, 0);

            if (progresiveScore > best)
            {
                OnNewHighScore();
            }
        }
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

    void SetLeaderboard () {
        if (m_GameMode == GameMode.Free) {
            bl_PlayService.Instance.ShotThisLeaderboard(PlayServiceKey.leaderboard_ranking_free, progresiveScore);
        } else {
            bl_PlayService.Instance.ShotThisLeaderboard(PlayServiceKey.leaderboard_ranking_classic, totalPoints);
        }
    }

	void PlayRandomMusic()
	{
		if (PlayerPrefs.GetInt (KeyMasters.Sound, 1) == 0)
			return;
		
		audioSource.clip = musics [Random.Range (0, musics.Length)];
		audioSource.Play ();
	}

    void Update()
    {
        TimeManager();
        ScrollLevel();
        PCInputs();
        TutorialInputs();
    }

    void PCInputs()
    {
     /*   if (Speedbox.bl_Utils.IsMobile)
            return;*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void TutorialInputs () {
        //print(PlayerPrefs.GetInt(KeyMasters.Tutorial));
        if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) < 3) {

            if (BlockBlast.bl_Utils.IsMobile) {
                for (var i = 0; i < Input.touchCount; ++i) {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Ended) {

                        float buttonsize = Screen.width / 3;
                        if (touch.position.x >= 0 && touch.position.x < buttonsize) {
                            if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 0) {
                                PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                            }
                        } else if (touch.position.x >= buttonsize && touch.position.x < (buttonsize * 2)) {
                            if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 2) {
                                PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                            }
                        } else if (touch.position.x >= (buttonsize * 2)) {
                            if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 1) {
                                PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                            }
                        }
                    }
                }
            } else {

                if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 0) {
                    if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                        PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                    }
                } else if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 1) {
                    if (Input.GetKeyUp(KeyCode.RightArrow)) {
                        PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                    }
                } else if (PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) == 2) {
                    if (Input.GetKeyUp(KeyCode.Space)) {
                        PlayerPrefs.SetInt(KeyMasters.Tutorial, PlayerPrefs.GetInt(KeyMasters.Tutorial, 0) + 1);
                    }
                }

            }

            uiManager.ShowHideTutorialPanel(PlayerPrefs.GetInt(KeyMasters.Tutorial, 0));

        } else {
            uiManager.ShowHideTutorialPanel(4);
        }

    }

    void ProgresiveScore()
    {
        if (m_GameMode != GameMode.Free)
        {
            CancelInvoke("ProgresiveScore");
            return;
        }

        int ps = (int)Random.Range(PerSecondPoints.x, PerSecondPoints.y);
        progresiveScore += ps;
        ProgresiveScoreText.text = progresiveScore.ToString("000000");
        CalculateProgresiveLevel();
    }

    void CalculateProgresiveLevel()
    {
        if(progresiveScore > Levels[lastLevel].ProgresiveNeeded)
        {
            lastLevel++;
            OnSpeedLevel();
        }
    }

    void TimeManager()
    {
        prevRealTime = thisRealTime;
        thisRealTime = Time.realtimeSinceStartup;
    }

    public void ChangeQualityGame(bool low)
    {
        LowSettings = low;
        if (low)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.masterTextureLimit = 2;
            RenderSettings.fog = false;
        }
        else
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.masterTextureLimit = 2;
            RenderSettings.fog = false;
        }
    }

    void ScrollLevel()
    {
        foreach(Material m in LevelMats)
        {
            m.SetTextureOffset("_MainTex", new Vector2(0, -Time.time * ScrollSpeed));
        }
    }

    public bool HaveMoreLevels
    {
        get
        {
            return !(lastLevel > Levels.Count - 1);
        }
    }

    public int GetCurrentObstacleLevel
    {
        get
        {
            return currentObstacleLevel;
        }
    }

    public int GetLevelSpeed()
    {
        int speed = Levels[0].Speed;
        if (m_GameMode == GameMode.PickUp)
        {
            foreach (bl_LevelInfo info in Levels)
            {
                if (point > info.PointsNeeded)
                {
                    speed = info.Speed;
                }
            }
        }
        else if (m_GameMode == GameMode.Free)
        {
            speed = Levels[lastLevel].Speed;
        }
        return speed;
    }

    public int GetNextScoreNeeded()
    {
        int speed = Levels[lastLevel].PointsNeeded;
      
        return speed;
    }

    public int GetLastScoreNeeded()
    {
        int speed = (lastLevel > 0) ? Levels[lastLevel - 1].PointsNeeded : Levels[0].PointsNeeded;

        return speed;
    }

    public float deltaTime
    {
        get
        {
            if (Time.timeScale > 0f) return Time.deltaTime / Time.timeScale;
            return Time.realtimeSinceStartup - prevRealTime; // Checks realtimeSinceStartup again because it may have changed since Update was called
        }
    }

    private bool isNewLevel()
    {
       if(point > Levels[lastLevel].PointsNeeded)
        {
            lastLevel++;
            return true;
        }
        return false;
    }

    public int GetCacheScore { get { return CacheScore; } }

    public int CurrentLevel
    {
        get
        {
            return lastLevel + 1;
        }
    }

    public static bl_GameManager Instance
    {
        get
        {
            return ((bl_GameManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }

    public void displayRightControll () {
        print("press left");
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Scores")]
    void ResetScores()
    {
        PlayerPrefs.DeleteKey(KeyMasters.BestScore);
        PlayerPrefs.DeleteKey(KeyMasters.LastScore);
        PlayerPrefs.DeleteKey(KeyMasters.BestScoreProgresive);
        PlayerPrefs.DeleteKey(KeyMasters.LastScoreProgresive);
    }
#endif
}