using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField] private GameObject deaftFX;
    private float health;
    private int coins;
    public TextMesh healthText;

    void Start() {
        health = Mathf.CeilToInt(Random.Range(2, PlayerPrefs.GetFloat("FIRE_DAMAGE", 1) + 5));
        coins = (int)health;
        healthText.text = health.ToString();
    }

    private void Update() {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Damage (float amount) {

        health -= amount;

        healthText.text = Mathf.CeilToInt(health).ToString();

        if (health <= 0) {
            Destroy(Instantiate(deaftFX, transform.position, Quaternion.identity), 2f);
            bl_GameManager.Instance.AddCoins(coins);
            transform.parent.parent.GetComponent<bl_ObstacleRoot>().OnPickUp(transform);
            //Destroy(gameObject); 
            gameObject.SetActive(false);
            health = Random.Range(2, PlayerPrefs.GetFloat("FIRE_DAMAGE", 1) + 5);

        }

    }

    
}
