using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class bl_IAPManager : MonoBehaviour {

    [SerializeField] private Text coins;

    public void MainMenu () {
        SceneManager.LoadScene("Menu");
    }

    
    void FixedUpdate () {
        coins.text = PlayerPrefs.GetInt("COINS", 0).ToString();
    }

    public void ShowRewardAdd () {
        bl_Ad.Instance.ShowRewardVideo();
    }

    public void addCoins (int amount) {
        PlayerPrefs.SetInt("COINS", PlayerPrefs.GetInt("COINS", 0) + amount);
    }

    public void removeAds () {
        PlayerPrefs.SetInt(KeyMasters.Ads, 1);
    }

}
