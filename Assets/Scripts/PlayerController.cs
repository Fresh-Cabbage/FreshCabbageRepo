using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxXSpeed;
    public float xAccel;
    public float jumpPower;

    Rigidbody2D rb2d;
    SpriteRenderer sr;


    float xInput;
    bool jInput;
    bool oldJInput;


    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // will be deleted when the sprite is no longer attached to the same gameobject as the script
    }

    private void Update() {
        // get inputs
        xInput = Input.GetAxisRaw("Horizontal");
        jInput = Input.GetAxisRaw("Jump") > 0;
    }

    private void FixedUpdate() {
        Vector2 vel = rb2d.velocity;

        if (xInput != 0) {
            // accelerate player in the direction of motion
            vel.x += xInput * xAccel;
            vel.x = Mathf.Clamp(vel.x, -maxXSpeed, maxXSpeed);
        }
        else {
            if (Mathf.Abs(vel.x) < xAccel) {
                // player should stop
                vel.x = 0;
            }
            else {
                // player's speed should decrease
                vel.x -= Mathf.Sign(vel.x) * xAccel;
            }
        }

        if (jInput && !oldJInput) {
            vel.y = jumpPower;
        }

        rb2d.velocity = vel;

        // indicate that we have used the current jump input
        oldJInput = jInput;
    }
}
