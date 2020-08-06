using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class bl_UpgradeManager : MonoBehaviour {

    [SerializeField] private Text coinText;
    [SerializeField] private Text firePriceText;
    [SerializeField] private Text damagePriceText;
    [SerializeField] private Text rapidFireText;
    [SerializeField] private Text damageText;
    [SerializeField] private Text notEnoughText;

    [SerializeField] private Animator loadAnimator;

    private void Start() {
        setLabels();
        notEnoughText.CrossFadeAlpha(0, 0, false);
    }

    public void UpgradeFireRate () {

        int coins = PlayerPrefs.GetInt("COINS", 0);
        int fireRate = PlayerPrefs.GetInt("FIRE_RATE", 70);
        int price = PlayerPrefs.GetInt("FIRE_PRICE", 20);

        if (price <= coins) {

            PlayerPrefs.SetInt("FIRE_RATE", fireRate + 1);
            PlayerPrefs.SetInt("FIRE_PRICE", (int)(price * 1.5f));
            PlayerPrefs.SetInt("COINS", coins - price);

        } else {
            notEnoughText.CrossFadeAlpha(1, 0, false);
            notEnoughText.CrossFadeAlpha(0, 1.5f, false);
        }
        setLabels();
    }

    public void UpgradeDamage () {

        int coins = PlayerPrefs.GetInt("COINS", 0);
        int damage = PlayerPrefs.GetInt("DAMAGE", 1);
        int price = PlayerPrefs.GetInt("DAMAGE_PRICE", 20);

        if (price <= coins) {

            PlayerPrefs.SetInt("DAMAGE", damage + 1);
            PlayerPrefs.SetInt("DAMAGE_PRICE", (int)(price * 1.5f));
            PlayerPrefs.SetInt("COINS", coins - price);

        } else {
            notEnoughText.CrossFadeAlpha(1, 0, false);
            notEnoughText.CrossFadeAlpha(0, 1.5f, false);
        }
        setLabels();

    }

    void setLabels () {
        coinText.text = PlayerPrefs.GetInt("COINS", 0).ToString();
        rapidFireText.text = (PlayerPrefs.GetInt("FIRE_RATE", 70) - 69) + "%".ToString();
        damageText.text = PlayerPrefs.GetInt("DAMAGE", 1) + "%";

        firePriceText.text = "Upgrade for\n" + PlayerPrefs.GetInt("FIRE_PRICE", 20) + " Coins";
        damagePriceText.text = "Upgrade for\n" + PlayerPrefs.GetInt("DAMAGE_PRICE", 20) + " Coins";
    }

    public void LoadMenu() {

        loadAnimator.gameObject.SetActive(true);
        StartCoroutine(WaitToLoad());

    }

    IEnumerator WaitToLoad() {
        yield return new WaitForSeconds(loadAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

}
