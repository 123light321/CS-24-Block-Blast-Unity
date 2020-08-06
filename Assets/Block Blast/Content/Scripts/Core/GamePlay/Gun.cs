using UnityEngine;
using System.Collections;
public class Gun : MonoBehaviour {

    public float maxFireRate;
    public float bulletSpeed;
    public GameObject[] bullet;
    public float shootDelay;
    int selectedBullet;

    float preShootTime;
    float damage;

    int shots;

    Vector3 bulletSize;

    [HideInInspector]
    public bool hasFireRate, hasHeavyFire, hasInvincibilty, hasDoubleShots;

    private void Start() {
        selectedBullet = PlayerPrefs.GetInt("SelectedBullet", 0);
        shootDelay = PlayerPrefs.GetFloat("FIRE_RATE", shootDelay);
        damage = PlayerPrefs.GetFloat("FIRE_DAMAGE", 1);
        shots = 4;
        bulletSize = new Vector3(0.25f, 0.25f, 0.25f);
    }

    public void shoot (int direction) {

        float delay = (maxFireRate - shootDelay) / 100;

        if (Time.time > preShootTime + delay) {
            
            preShootTime = Time.time;

            Vector3 pos = transform.position;
                        
            GameObject bt = Instantiate(bullet[selectedBullet], pos, Quaternion.identity);
            bt.transform.localScale = bulletSize;
            bt.AddComponent<BulletController>().targetTag = "Enemy";
            bt.GetComponent<BulletController>().setBulletAttr(bulletSpeed * direction, damage);
            
        }

    }

    public IEnumerator fireRatePowerup () {
        hasFireRate = true;
        shootDelay *= 1.5f;
        yield return new WaitForSeconds(5f);
        shootDelay = PlayerPrefs.GetFloat("FIRE_DAMAGE");
        hasFireRate = false;
    }

    public IEnumerator startHeavyFire () {
        hasHeavyFire = true;
        damage *= 1.2f;
        bulletSize *= 1.5f;
        yield return new WaitForSeconds(5);
        damage = PlayerPrefs.GetFloat("FIRE_DAMAGE");
        hasHeavyFire = false;
        bulletSize /= 1.5f;

    }

    public IEnumerator startDoubleShots () {

        int s = shots;
        hasDoubleShots = true;
        shots *= 4;
        yield return new WaitForSeconds(5);
        hasDoubleShots = false;
        shots = s;

    }

}
