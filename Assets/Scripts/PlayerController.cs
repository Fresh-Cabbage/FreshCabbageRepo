using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxXSpeed;
    public float xAccel;
    public float jumpPower;
    public float verticalJerk;
    public float maxFallSpeed;
    float baseGravity;

    public float jumpBufferTime;
    public float coyoteTime;
    float jumpBufferTimer;
    float coyoteTimer;

    Rigidbody2D rb2d;
    SpriteRenderer sr;

    public CollisionCheck groundcheck;
    public CollisionCheck hazardcheck;
    bool isGrounded { get { return groundcheck.IsColliding; }}


    float xInput;
    bool jInput;
    bool oldJInput;


    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // will be deleted when the sprite is no longer attached to the same gameobject as the script
    
        baseGravity = rb2d.gravityScale;
    }

    private void Update() {
        if (hazardcheck.IsColliding)
            Die();

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

        // jump buffer timer should decrease when jump button has not been pushed down
        jumpBufferTimer = jInput && !oldJInput ? jumpBufferTime : Helpers.FixedTimer(jumpBufferTimer);
        
        // coyote timer should decrease when not on the ground
        coyoteTimer = isGrounded ? coyoteTime : Helpers.FixedTimer(coyoteTimer);

        // we know the player should jump if both timers are active simultaneously
        if (jumpBufferTimer > 0 && coyoteTimer > 0) {
            vel.y = jumpPower;

            // reset timers
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }

        if (vel.y > 0 && !jInput && oldJInput) {
            // player has stopped holding jump - decrease y velocity to let player jump shorter
            vel.y /= 2;
        }

        vel.y = Mathf.Max(vel.y, -maxFallSpeed);

        if (isGrounded) {
            // player is not falling, so set gravity scale to base level
            rb2d.gravityScale = baseGravity;
        }
        else if (vel.y < maxFallSpeed) {
            // player is not yet falling at max speed, so increase gravity scale to fall faster
            rb2d.gravityScale += verticalJerk;
        }

        rb2d.velocity = vel;

        // indicate that we have used the current jump input
        oldJInput = jInput;
    }


    private void Die() {
        GameManager.Instance?.PlayerDied();
        Destroy(gameObject);
    }
}
