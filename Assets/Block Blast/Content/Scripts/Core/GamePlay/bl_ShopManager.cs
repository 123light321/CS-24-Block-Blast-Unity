using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class bl_ShopManager : MonoBehaviour {

    [SerializeField] private Text priceLabel;
    [SerializeField] private Button shopButton;
    [SerializeField] private Text shopBtnLabel;
    [SerializeField] private Text coinText;
    [SerializeField] private Text notEnoughText;

    [SerializeField] private Animator loadAnimator;
    [SerializeField] private GameObject skinHolder;

    [SerializeField] private GameObject[] skinsObj;
    [SerializeField] private GameObject lockedObj;

    public Skin[] skins;

    private float currPos;
    private float nextPos;
    [SerializeField] private float tileDiff;

    Camera cam;

    int selectedSkin;
    int skinNumber;

    private void Start() {

        currPos = 0;
        notEnoughText.CrossFadeAlpha(0, 0, false);
        coinText.text = PlayerPrefs.GetInt("COINS", 0).ToString();
        cam = Camera.main;
        selectedSkin = PlayerPrefs.GetInt("SKIN", 0);

        if (SkinLocker.getLocker() == null) {
            skins = new Skin[skinsObj.Length];

            for (int i = 0; i < skins.Length; i++) {

                Skin skin = new Skin(false, false, 500 * i, i);

                if (i == 0) {
                    skin.isPurchased = true;
                    skin.isSelected = true;
                }

                skins[i] = skin;

            }

            SkinLocker.setupLocker(new AllSkins(skins));

        }

        setupSkins();

    }

    public void LoadMenu () {

        loadAnimator.gameObject.SetActive(true);
        StartCoroutine(WaitToLoad());

    }

    private void OnTriggerEnter(Collider other) {
        
        if (other.GetComponent<bl_Skin>() != null) {
            skinNumber = other.GetComponent<bl_Skin>().number;

            if (skins[skinNumber].isPurchased && skins[skinNumber].isSelected) {
                //selectedSkin = skinNumber;
                shopButton.gameObject.SetActive(false);
                priceLabel.gameObject.SetActive(false);
            } else if (skins[skinNumber].isPurchased && !skins[skinNumber].isSelected) {
                shopButton.gameObject.SetActive(true);
                priceLabel.gameObject.SetActive(false);
                shopBtnLabel.text = "Select";
            } else {
                priceLabel.gameObject.SetActive(true);
                shopButton.gameObject.SetActive(true);
                shopBtnLabel.text = "Purchase";
                priceLabel.text = "Purchase for \n" + skins[skinNumber].price + " Coins";
            }

        }

    }

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<bl_Skin>() != null) {

            other.transform.Rotate(0, 45 * Time.deltaTime, 0);

        }
    }

    IEnumerator WaitToLoad() {
        yield return new WaitForSeconds(loadAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void NextSkin() {

        StopAllCoroutines();
        currPos = nextPos;
        if (currPos > (-12 * (skins.Length-1)))
            nextPos = currPos - tileDiff;
        StartCoroutine(loadSkin(-1));

    }

    public void PreviousSkin () {
        StopAllCoroutines();
        currPos = nextPos;
        if (currPos != 0)
            nextPos = currPos + tileDiff;
        StartCoroutine(loadSkin(1));
    }

    IEnumerator loadSkin (int dir) {

        Debug.Log(skinHolder.transform.childCount);

        while (currPos != (nextPos * dir)) {
            currPos = Mathf.Lerp(currPos, nextPos, 6 * Time.deltaTime);
            skinHolder.transform.position = new Vector3(currPos, skinHolder.transform.position.y, skinHolder.transform.position.z);
            yield return null;

        }

    }

    void setupSkins () {

        if (skinHolder.transform.childCount > 0) {
            for (int i = 0; i < skinHolder.transform.childCount; i++) {
                Destroy(skinHolder.transform.GetChild(i).gameObject);
            }
        }

        skins = SkinLocker.getLocker().getSkins();

        for (int i = 0; i < skins.Length; i++) {

            GameObject obj;

            if (skins[i].isPurchased) {
                if (skins[i].isSelected) {
                    selectedSkin = i;
                }
                obj = Instantiate(skinsObj[i], skinHolder.transform.position + new Vector3(i * tileDiff, 0, 0), Quaternion.identity);
            } else {
                obj = Instantiate(lockedObj, skinHolder.transform.position + new Vector3(i * tileDiff, 0, 0), Quaternion.identity);
            }
            obj.transform.SetParent(skinHolder.transform);
            obj.transform.localScale = Vector3.one;

            //Add Box Collider
            BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(2, 2, 2);
            boxCollider.isTrigger = true;

            //Add Skin component
            obj.AddComponent<bl_Skin>().number = i;
            obj.AddComponent<Rigidbody>().isKinematic = true;

        }
    }

    public void ChooseSkin () {

        if (skins[skinNumber].isPurchased) {

            PlayerPrefs.SetInt("SKIN", skinNumber);
            skins[skinNumber].isSelected = true;
            skins[selectedSkin].isSelected = false;

        } else {

            int coins = PlayerPrefs.GetInt("COINS", 0);

            if (skins[skinNumber].price <= coins) {
                skins[skinNumber].isPurchased = true;
                skins[skinNumber].isSelected = true;
                skins[selectedSkin].isSelected = false;
                PlayerPrefs.SetInt("SKIN", skinNumber);
                PlayerPrefs.SetInt("COIN", coins - skins[skinNumber].price);
                coinText.text = PlayerPrefs.GetInt("COINS", 0).ToString();
            } else {
                notEnoughText.CrossFadeAlpha(1, 0, false);
                notEnoughText.CrossFadeAlpha(0, 1.5f, false);
            }

        }

        SkinLocker.setupLocker(new AllSkins(skins));
        setupSkins();

    }
    
}
