using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;

public class bl_Ad : MonoBehaviour {

    public static bl_Ad Instance;

    [SerializeField] private string appID;
    private string bannerID;
    [SerializeField] private string interstitialID;
    [SerializeField] private string rewardID;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    int counter = 0;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        counter = 3;
    }

    private void Start() {
        MobileAds.Initialize(appID);
        RequestBanner();
        LoadInterstitial();
        LoadRewardVideo();
    }

    private void RequestBanner() {

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    public void ShowBanner () {
        if (PlayerPrefs.GetInt(KeyMasters.Ads, 0) == 0) {
            bannerView.Show();
        }
    }

    public void HideBanner () {
        bannerView.Hide();
    }

    private void LoadInterstitial() {

        interstitial = new InterstitialAd(interstitialID);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);

        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;


    }

    public void ShowInterstitial () {

        
        print(counter);
        if (PlayerPrefs.GetInt(KeyMasters.Ads, 0) == 0) {

            if (interstitial.IsLoaded() && counter == 3) {
                counter = 0;
                interstitial.Show();
                LoadInterstitial();
            }
        
        }

        counter++;

    }


    //Interstitial Handler
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        LoadInterstitial();
    }

    public void ShowRewardVideo () {
        if (rewardedAd.IsLoaded()) {
            rewardedAd.Show();
        }
        StartCoroutine(LoadReward());
    }

    void LoadRewardVideo () {
        rewardedAd = new RewardedAd(rewardID);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);

        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    }


    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) {
        StartCoroutine(LoadReward());
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        int coins = PlayerPrefs.GetInt("COINS", 0) + 50;
        PlayerPrefs.SetInt("COINS", coins);
    }


    IEnumerator LoadReward () {
        yield return new WaitForSeconds(10);
        LoadRewardVideo();
    }

}