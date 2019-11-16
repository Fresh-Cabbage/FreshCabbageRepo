using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb2d;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        float angle = Vector2.SignedAngle(Vector2.right, rb2d.velocity);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetDirection(Quaternion rotation, float speed) {
        if (rb2d == null)
            Start();
        rb2d.velocity = rotation * Vector2.right * speed;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            DestroyBullet();
        }
    }


    private void DestroyBullet() {
        Destroy(gameObject);
    }
}
