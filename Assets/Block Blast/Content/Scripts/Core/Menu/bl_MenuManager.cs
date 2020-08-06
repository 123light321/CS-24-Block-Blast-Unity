
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class bl_MenuManager : Singleton<bl_MenuManager> {
    [SerializeField] private string[] MenuNames;
    [SerializeField] private GameObject[] Menus;
    [Header("References")]
    [SerializeField] private Text MenuNameText;
    [SerializeField] private Text QualityText;
    [SerializeField] private Text AudioText;
    [SerializeField] private Animator LoadAnimator;
    [SerializeField] private Animator ButtonSelectAnim;
    [SerializeField] private Button[] GameModesButtons;
    //[SerializeField] private GameObject[] PlayServiceGO;
    [SerializeField]private GameObject GameModeUI;
    [SerializeField]private Slider VolumeSlider;
    [SerializeField]private Text NonSelectGMText;
    [SerializeField] private Text coin;
    [SerializeField] private Text bestScore;
    AndroidJavaClass JavaClass2;
    private int CurrentMenu = 0;
    private bool GMSelect = false;

    void Awake()
    {
        MenuNameText.text = MenuNames[CurrentMenu];
        LoadPrefabs();

        Time.timeScale = 1;
        //foreach(GameObject g in PlayServiceGO) { g.SetActive(false); }
        coin.text = PlayerPrefs.GetInt("COINS", 0).ToString();
        bestScore.text = PlayerPrefs.GetInt(KeyMasters.BestScore, 0).ToString();

    }

    void LoadPrefabs()
    {
        bool q = BlockBlast.bl_Utils.PlayerPrefsX.GetBool(KeyMasters.Quality, false);
        bool a = (PlayerPrefs.GetInt(KeyMasters.Sound, 1) == 1) ? true : false;
        ChangeQuality(q);
        ChangeAudio(a);
        VolumeSlider.value = PlayerPrefs.GetFloat(KeyMasters.Volume, 1);
        Volume(PlayerPrefs.GetFloat(KeyMasters.Volume, 1));
        NonSelectGMText.canvasRenderer.SetAlpha(0);
    }

    public void ChangeMenu(bool forward)
    {
        CancelInvoke("ChangeButtonText");
        if (forward)
        {
            CurrentMenu = (CurrentMenu + 1) % MenuNames.Length;
        }
        else
        {
            CurrentMenu = (CurrentMenu != 0) ? CurrentMenu = (CurrentMenu - 1) % MenuNames.Length : CurrentMenu = MenuNames.Length - 1;
        }
        ButtonSelectAnim.Play("change", 0, 0);
        Invoke("ChangeButtonText", 0.25f);
    }

    void ChangeButtonText() { MenuNameText.text = MenuNames[CurrentMenu]; }

    public void LoadOption()
    {
        MenuStates m = (MenuStates)CurrentMenu;
        switch (m)
        {
            case MenuStates.PLAY:
                GameModeUI.SetActive(true);
                break;
            case MenuStates.OPTIONS:
                ActiveMenu((int)MenuStates.OPTIONS);
                break;
            case MenuStates.CREDITS:
                ActiveMenu((int)MenuStates.CREDITS);
                break;
            case MenuStates.INFO:
                ActiveMenu((int)MenuStates.INFO);
                break;
            case MenuStates.QUIT:
                ActiveMenu((int)MenuStates.QUIT);
                break;
        }
    }

    public void SelectGameModeButton(Button b)
    {
        foreach(Button btn in GameModesButtons)
        {
            btn.interactable = true;
        }
        b.interactable = false;
    }

    public void SelectGameMode(int id)
    {
        PlayerPrefs.SetInt(KeyMasters.GameMode, id);
        GMSelect = true;
    }

    public void CloseSelectGM() { GameModeUI.SetActive(false); }

    void ActiveMenu(int id)
    {
        foreach(GameObject o in Menus) {if(o != null) o.SetActive(false); }
        Menus[id].SetActive(true);
    }

    public void RateApp()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.Redmoon.BlockBlast");
#elif UNITY_IPHONE
 Application.OpenURL("itms-apps://itunes.apple.com/app/idcom.Redmoon.BlockBlast");
#endif
    }

    public void OpenShop() {
        SceneManager.LoadScene("Shop");
    }

    public void OpenLocker () {
        SceneManager.LoadScene("Locker");
    }

    public void OpenUpgrades () {
        SceneManager.LoadScene("Upgrade");
    }

    public void LoadScene()
    {
        if (!GMSelect)
        {
            NonSelectGMText.canvasRenderer.SetAlpha(0);
            NonSelectGMText.CrossFadeAlpha(1, 1.3f, true);
            return;
        }

        LoadAnimator.gameObject.SetActive(true);
        StartCoroutine(WaitToLoad());
    }

    public void ShowLeaderboard () {
        bl_PlayService.Instance.ShowLeaderboard(PlayServiceKey.leaderboard_ranking_classic);
    }

    public void ShowAchievements () {
        bl_PlayService.Instance.ShowAchievements();
    }

    public void ChangeQuality(bool b)
    {
        string t = (b == false) ? "GOOD" : "LOW";
        QualityText.text = t;
        BlockBlast.bl_Utils.PlayerPrefsX.SetBool(KeyMasters.Quality, b);
    }

    public void ChangeAudio(bool b)
    {
        string t = (b) ? "ENABLE" : "DISABLE";
        AudioText.text = t;
        int i = (b) ? 1 : 0;
        PlayerPrefs.SetInt(KeyMasters.Sound, i);
    }

    public void Volume(float v)
    {
        PlayerPrefs.SetFloat(KeyMasters.Volume, v);
    }

    IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(LoadAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static bl_MenuManager Instance
    {
        get
        {
            return ((bl_MenuManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}