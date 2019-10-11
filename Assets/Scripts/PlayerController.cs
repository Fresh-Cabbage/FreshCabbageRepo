using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementProfile normalMovement;
    public MovementProfile totemMovement;

    MovementState moveState;

    float baseGravity;

    public float jumpBufferTime;
    public float coyoteTime;
    float jumpBufferTimer;
    float coyoteTimer;
    float minimumJumpTimer;

    Rigidbody2D rb2d;
    SpriteRenderer sr;

    public CollisionCheck groundcheck;
    public CollisionCheck hazardcheck;
    public CollisionCheck totemcheck;
    bool isGrounded { get { return groundcheck.IsColliding; }}


    TotemContainer heldTotem;
    public Vector2 totemThrowVel;
    public Vector2 totemHoldPosition;


    float xInput;
    bool jInput;
    bool tInput;
    bool oldJInput;
    bool oldTInput;
    bool dInput;

    bool tPressed { get { return tInput && !oldTInput; }}

    
    // events!
    public delegate void PlayerAction();
    public static PlayerAction OnDeath;

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
        dInput = Input.GetAxisRaw("Vertical") < 0;

        oldTInput = tInput;
        tInput = Input.GetAxisRaw("Interact") > 0;

        // check for flip when player is moving in direction of input
        if (xInput * rb2d.velocity.x > 0) {
            // flip when signs of velocity and local scale do not match
            if (Mathf.Sign(rb2d.velocity.x) != Mathf.Sign(transform.localScale.x)) {
                transform.localScale = transform.localScale.WithX(transform.localScale.x * -1);
            }
        }

        if (tPressed) {
            DoTotem();
        }
    }

    private void FixedUpdate() {
        MovementProfile mov = heldTotem ? totemMovement : normalMovement;

        Vector2 vel = rb2d.velocity;

        // first, handle general movement state stuff
        if (moveState == MovementState.IDLE) {
            // player ducked: transition to DUCK state
            if (dInput && Mathf.Abs(vel.x) < 0.05f) {
                sr.color = sr.color.WithAlpha(0.5f);
                moveState = MovementState.DUCK;
            }

            // player is moving: transition to WALK state
            else if (xInput != 0 && vel.x != 0) {
                moveState = MovementState.WALK;
            }
        } else if (moveState == MovementState.WALK) {
            if (Mathf.Abs(vel.x) < 0.05f) {
                moveState = MovementState.IDLE;
            }
        } else if (moveState == MovementState.DUCK) {
            // player unducked: transition to IDLE state
            if (!dInput) {
                sr.color = sr.color.WithAlpha(1f);
                moveState = MovementState.IDLE;
            }
        }
        Debug.Log(moveState);

        if (moveState != MovementState.DUCK && xInput != 0) {
            // accelerate player in the direction of motion
            vel.x += xInput * mov.xAccel;
            vel.x = Mathf.Clamp(vel.x, -mov.maxXSpeed, mov.maxXSpeed);
        }
        else {
            if (Mathf.Abs(vel.x) < mov.xAccel) {
                // player should stop
                vel.x = 0;
            }
            else {
                // player's speed should decrease
                vel.x -= Mathf.Sign(vel.x) * mov.xAccel;
            }
        }

        // jump buffer timer should decrease when jump button has not been pushed down
        jumpBufferTimer = jInput && !oldJInput ? jumpBufferTime : Helpers.FixedTimer(jumpBufferTimer);
        
        // coyote timer should decrease when not on the ground
        coyoteTimer = isGrounded ? coyoteTime : Helpers.FixedTimer(coyoteTimer);

        // minimum jump timer should just always decrease
        minimumJumpTimer = Helpers.FixedTimer(minimumJumpTimer);

        // we know the player should jump if both timers are active simultaneously
        if (jumpBufferTimer > 0 && coyoteTimer > 0 && moveState.CanGoToJump()) {
            moveState = MovementState.JUMP;
            vel.y = mov.jumpPower;

            // start minimum jump timer
            minimumJumpTimer = 0.02f;

            // reset timers
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }

        if (minimumJumpTimer == 0 && moveState == MovementState.JUMP && (!jInput || rb2d.velocity.y < 0.2f)) {
            // player's jump has terminated (either by releasing the jump button or by starting to move down)
            moveState = MovementState.FALL;
            vel.y /= 2;
        }

        vel.y = Mathf.Max(vel.y, -mov.maxFallSpeed);

        if (isGrounded) {
            // player is not falling, so set gravity scale to base level
            rb2d.gravityScale = baseGravity;

            if ((moveState == MovementState.JUMP && minimumJumpTimer == 0) || moveState == MovementState.FALL) {
                // player has landed
                moveState = MovementState.IDLE;
            }
        }
        else if (vel.y < mov.maxFallSpeed) {
            // player is not yet falling at max speed, so increase gravity scale to fall faster
            rb2d.gravityScale += mov.verticalJerk;

            if (moveState != MovementState.JUMP) {
                moveState = MovementState.FALL;
            }
        }

        rb2d.velocity = vel;

        // indicate that we have used the current jump input
        oldJInput = jInput;
    }


    void DoTotem() {
        if (heldTotem == null) {
            // player is trying to grab a totem
            // reject if the player can't grab in current state
            if (!isGrounded || Mathf.Abs(rb2d.velocity.x) > totemMovement.maxXSpeed) return;

            // reject if there is no totem in front of the player
            if (totemcheck.collider == null) return;

            Totem theTotem = totemcheck.collider.GetComponent<Totem>();

            // reject if the totem is not grabbale in its current state
            if (theTotem.rb2d.velocity.magnitude > 0.1f) return;
            
            heldTotem = totemcheck.collider.GetComponent<Totem>()?.parent;
            heldTotem?.HoldTotem(transform, totemHoldPosition.ToVector3());
        }
        else {
            // totem held: release it
            heldTotem.ReleaseTotem(totemThrowVel.WithX(totemThrowVel.x * Mathf.Sign(transform.localScale.x)));
            heldTotem = null;
        }
    }

    private void Die() {
        OnDeath();
        GameManager.Instance?.PlayerDied();

        Destroy(gameObject);
    }


    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + totemHoldPosition.ToVector3(), 0.2f);
    }
}


[System.Serializable]
public struct MovementProfile {
    public float maxXSpeed;
    public float xAccel;
    public float jumpPower;
    public float verticalJerk;
    public float maxFallSpeed;
}