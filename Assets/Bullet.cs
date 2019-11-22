using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb2d;

    public float trampolineBounceMultiplier;

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
        // ignore breakableblock collisions
        // note that we can't have the bullet notify the breakableblock that it was hit because breakableblock colliders are composited into one massive collider
        if (other.CompareTag("Ground") && other.GetComponent<BreakableBlock>() == null) {
            DestroyBullet();
        }
    }

    public void BouncedOnTrampoline(float bounceStrength) {
        rb2d.velocity = rb2d.velocity.WithY(bounceStrength * trampolineBounceMultiplier);
    }


    public void DestroyBullet() {
        Destroy(gameObject);
    }
}
