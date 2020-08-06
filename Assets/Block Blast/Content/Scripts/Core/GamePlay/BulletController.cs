using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public string targetTag;

    private float speed;
    private float damage;
    private Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    void FixedUpdate() {

        transform.Translate(transform.forward * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, startPos) > 50) {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == targetTag) {
            other.GetComponent<Health>().Damage(damage);
            if (other.tag == "Enemy") {
            }
            Destroy(gameObject);
        }
    }


    public void setBulletAttr (float _speed, float _damage) {
        speed = _speed;
        damage = _damage;
    }

}
