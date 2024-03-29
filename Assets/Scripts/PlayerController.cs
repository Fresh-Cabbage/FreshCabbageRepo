﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementProfile normalMovement;
    public MovementProfile totemMovement;
    private MovementProfile CurrentMovement { get { return heldTotemContainer != null ? totemMovement : normalMovement; }}

    int numJumpsUsed;

    public float rollSpeed;
    public float rollTime;
    float rollTimer;

    MovementState moveState;

    float baseGravity;

    public float jumpBufferTime;
    public float coyoteTime;
    float jumpBufferTimer;
    float coyoteTimer;
    float minimumJumpTimer;

    public float interactBufferTime;
    float interactBufferTimer;

    Rigidbody2D rb2d;
    public SpriteRenderer sr;
    Animator anim;

    public CollisionCheck groundcheck;
    public CollisionCheck hazardcheck;
    public CollisionCheck totemcheck;
    public CollisionCheck checkpointcheck;
    bool isGrounded { get { return groundcheck.IsColliding(); }}

    public GameObject rollHitbox;


    TotemContainer heldTotemContainer;
    public GameObject totemHoldObject;
    public Vector2 totemThrowVel;

    public bool physicsFrozen;
    public bool inCutscene;
    Vector2 previousVelocity;

    List<Vector2> boosts;

    float xInput;
    bool jInput;
    bool rInput;
    bool tInput;
    bool oldJInput;
    bool oldRInput;
    bool oldTInput;
    bool dInput;

    bool tPressed { get { return tInput && !oldTInput; }}


    public GameObject deathParticles;

    
    // events!
    public delegate void PlayerAction();
    public static PlayerAction OnStartRoll;
    public static PlayerAction OnStopRoll;
    public static PlayerAction OnDeath;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        baseGravity = rb2d.gravityScale;

        boosts = new List<Vector2>();
    }

    private void Update() {
        if (!inCutscene && hazardcheck.IsColliding())
            Die();

        // get inputs
        UpdateInput(!inCutscene);

        // check for flip when player is moving in direction of input
        if (xInput * rb2d.velocity.x > 0) {
            // flip when signs of velocity and local scale do not match
            if (Mathf.Sign(rb2d.velocity.x) != Mathf.Sign(transform.localScale.x)) {
                transform.localScale = transform.localScale.WithX(transform.localScale.x * -1);
            }
        }

        if (!inCutscene && !physicsFrozen) {
            HandleInteractions();
        }

        anim.SetFloat("YVel", rb2d.velocity.y);
        anim.SetBool("IsDucking", moveState == MovementState.DUCK);
        anim.SetBool("Grounded", isGrounded);
    }

    private void FixedUpdate() {
        Debug.DrawRay(transform.position, rb2d.velocity, Color.green, Time.deltaTime);

        // update the validity check of being grounded to account for new position
        groundcheck.validCheck = c => {
            return rb2d.velocity.y <= 0.2f;
        };

        if (!physicsFrozen) {
            HandleMotion();
        }

        rollHitbox.SetActive(isGrounded && moveState == MovementState.ROLL);

        // indicate that we have used some of the inputs
        oldJInput = jInput;
        oldRInput = rInput;
    }


    void UpdateInput(bool inputEnabled) {
        if (inputEnabled) {
            xInput = Input.GetAxisRaw("Horizontal");
            jInput = Input.GetAxisRaw("Jump") > 0;
            dInput = Input.GetAxisRaw("Vertical") < 0;
            rInput = Input.GetAxisRaw("Roll") > 0;

            oldTInput = tInput;
            tInput = Input.GetAxisRaw("Interact") > 0;
        } else {
            xInput = 0;
            jInput = dInput = rInput = oldTInput = tInput = false;
        }
    }

    void HandleInteractions() {
        interactBufferTimer = tPressed ? interactBufferTime : Helpers.Timer(interactBufferTimer);
        if (interactBufferTimer > 0) {
            bool didInteract = DoTotem();
            if (didInteract) {
                interactBufferTimer = 0;
            }
        }
    }

    void HandleMotion() {
        MovementProfile mov = CurrentMovement;

        Vector2 vel = rb2d.velocity;

        // BOOST HANDLING
        for (int b = boosts.Count - 1; b >= 0; b--) {
            // unapply this boost to get a base velocity
            Vector2 boost = boosts[b];
            vel -= boost;
            
            // reduce the intensity of the boost over time
            boosts[b] = DeBoost(boost);
            if (boosts[b] == Vector2.zero)
                boosts.RemoveAt(b);
        }

        // GROUNDED MOVEMENT STATE HANDLING
        if (moveState == MovementState.IDLE) {
            // player ducked: transition to DUCK state
            if (dInput && Mathf.Abs(vel.x) < 0.05f) {
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
                moveState = MovementState.IDLE;
            }
        }

        // ROLL STATE AND MOVEMENT HANDLING
        if (rollTimer > 0) {
            // max out the player's base x movement speed during a roll
            vel.x = CurrentMovement.maxXSpeed * transform.localScale.x;

            rollTimer = Helpers.Timer(rollTimer);
            if (rollTimer == 0) {
                StoppedRoll();
            }
        }
        if (rInput && !oldRInput && moveState.CanGoToRoll() && heldTotemContainer == null) {
            StartedRoll();
        }

        if (rollTimer == 0) {
            // HORIZONTAL MOVEMENT HANDLING
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
        }

        
        // JUMP CHECKING

        // jump buffer timer should decrease when jump button has not been pushed down
        jumpBufferTimer = jInput && !oldJInput ? jumpBufferTime : Helpers.FixedTimer(jumpBufferTimer);
        
        // coyote timer should decrease when not on the ground
        coyoteTimer = isGrounded ? coyoteTime : Helpers.FixedTimer(coyoteTimer);

        // minimum jump timer should just always decrease
        minimumJumpTimer = Helpers.FixedTimer(minimumJumpTimer);

        // we know the player should jump if both timers are active simultaneously
        if (jumpBufferTimer > 0 && coyoteTimer > 0 && moveState.CanGoToJump() && numJumpsUsed == 0) {
            vel.y = mov.jumpPower;
            StartedJump(1);
        }
        else if (mov.maxJumps == 2 && numJumpsUsed != 2 && coyoteTimer == 0 && jumpBufferTimer > 0 && moveState.CanGoToJump()) {
            vel.y = mov.jumpPower;
            StartedJump(2);
        }

        // FALL CHECKING
        if (minimumJumpTimer == 0 && moveState == MovementState.JUMP && (!jInput || rb2d.velocity.y < 0.2f)) {
            // player's jump has terminated (either by releasing the jump button or by starting to move down)
            moveState = MovementState.FALL;
            vel.y *= mov.shortJumpMultiplier;
        }

        // FALL MOTION HANDLING
        if (isGrounded) {
            // player is not falling, so set gravity scale to base level
            rb2d.gravityScale = baseGravity;

            if ((moveState == MovementState.JUMP && minimumJumpTimer == 0) || moveState == MovementState.FALL) {
                // player has landed
                moveState = MovementState.IDLE;
                numJumpsUsed = 0;
            }
        }
        else if (vel.y < mov.maxFallSpeed) {
            // player is not yet falling at max speed, so increase gravity scale to fall faster
            rb2d.gravityScale += mov.verticalJerk;

            if (moveState != MovementState.JUMP) {
                moveState = MovementState.FALL;
            }
        }

        vel.y = Mathf.Max(vel.y, -mov.maxFallSpeed);


        // re-apply boosts before assigning new velocity
        foreach (Vector2 boost in boosts) 
            vel += boost;

        rb2d.velocity = vel;
    }

    // reduces the intensity of this boost
    Vector2 DeBoost(Vector2 boost) {
        float reducedXMagnitude = Mathf.Abs(boost.x) - CurrentMovement.xAccel / 4;
        boost.x = Mathf.Sign(boost.x) * Mathf.Max(0, reducedXMagnitude);

        float reducedYMagnitude = Mathf.Abs(boost.y) - baseGravity / 2;
        boost.y = Mathf.Sign(boost.y) * Mathf.Max(0, reducedYMagnitude);

        return boost;
    }


    void StartedJump(int jumpNum) {
        moveState = MovementState.JUMP;

        // consume a jump
        numJumpsUsed = jumpNum;

        // reset gravity
        rb2d.gravityScale = baseGravity;

        // start minimum jump timer
        minimumJumpTimer = 0.02f;

        // reset timers
        jumpBufferTimer = 0;
        coyoteTimer = 0;

        // set anim trigger
        anim.SetTrigger(jumpNum == 1 ? "Jumped" : "DoubleJumped");

        // account for roll cancel in animation
        anim.SetBool("Rolling", false);

        // cut all current vertical boosts in half
        boosts = boosts.Select(boost => boost.WithY(boost.y / 2)).ToList();
    }

    public void BouncedOnTrampoline(float bounceVel) {
        // adjust jump height
        bounceVel *= jInput ? CurrentMovement.trampolineBounceMultiplierWithJump : CurrentMovement.trampolineBounceMultiplier;

        rb2d.velocity = new Vector2(rb2d.velocity.x, bounceVel);
        StartedJump(1);

        // turn this trampoline bounce thing into a boost
        boosts.Add(new Vector2(0, bounceVel));
    }

    void StartedRoll() {
        moveState = MovementState.ROLL;
        rollTimer = rollTime;
        anim.SetBool("Rolling", true);

        boosts.Add(new Vector2(rollSpeed * transform.localScale.x, 0));
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);

        OnStartRoll?.Invoke();
    }

    void StoppedRoll() {
        moveState = MovementState.WALK;
        anim.SetBool("Rolling", false);

        OnStopRoll?.Invoke();
    }


    void StartedGrab() {
        // physics + input disabled
        SetAnimationFreeze(true);

        anim.SetTrigger("Grabbed");
    }

    void StoppedGrab() {
        // physics + input enabled
        SetAnimationFreeze(false);
    }

    void StartedThrow() {
        anim.SetTrigger("Thrown");
    }

    void StoppedThrow() {
        
    }

    void StartedDrop() {
        // physics + input disabled

        anim.SetTrigger("Dropped");
    }

    void StoppedDrop() {
        // physics + input enabled
        // tell the TotemContainer to switch active totems at this time

        Debug.Log("PLAYER: termination of drop animation");
    }


    void SetAnimationFreeze(bool isFrozen) {
        physicsFrozen = isFrozen;

        if (isFrozen) {
            previousVelocity = rb2d.velocity;
            rb2d.velocity = Vector2.zero;
            rb2d.isKinematic = true;
        } else {
            rb2d.isKinematic = false;
            rb2d.velocity = previousVelocity;
            rb2d.gravityScale /= 2;
        }
    }


    // return value is whether there was a successful interaction with a totem
    bool DoTotem() {
        if (heldTotemContainer == null) {
            // reject if there is no totem in front of the player
            if (!totemcheck.IsColliding()) return false;

            Totem theTotem = totemcheck.GetCollider().GetComponent<Totem>();
            
            heldTotemContainer = theTotem?.parent;
            heldTotemContainer?.HoldTotem(totemHoldObject.transform);

            StartedGrab();
        }
        else {
            // totem held: release it
            heldTotemContainer.ReleaseTotem(totemThrowVel.WithX(totemThrowVel.x * Mathf.Sign(transform.localScale.x)));
            heldTotemContainer = null;

            StartedThrow();
        }

        return true;
    }

    public void Die() {
        OnDeath?.Invoke();
        Managers.LevelManager?.PlayerDied();

        GameObject.Instantiate(deathParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


}


[System.Serializable]
public struct MovementProfile {
    public float maxXSpeed;
    public float xAccel;
    public float jumpPower;
    [Range(0, 1)] public float shortJumpMultiplier;
    [Range(1, 2)] public int maxJumps;
    public float verticalJerk;
    public float maxFallSpeed;
    public float trampolineBounceMultiplier;
    public float trampolineBounceMultiplierWithJump;
}