using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bl_Start : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(openGame());
    }

    IEnumerator openGame () {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Menu");
    }

}
