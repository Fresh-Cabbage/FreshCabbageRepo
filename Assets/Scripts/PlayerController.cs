using System.Collections;
using System.Collections.Generic;
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
    bool isGrounded { get { return groundcheck.IsColliding(); }}

    public GameObject rollHitbox;


    TotemContainer heldTotemContainer;
    public GameObject totemHoldObject;
    public Vector2 totemThrowVel;

    public bool physicsFrozen;
    public bool inCutscene;
    Vector2 previousVelocity;

    float xInput;
    bool jInput;
    bool rInput;
    bool tInput;
    bool oldJInput;
    bool oldRInput;
    bool oldTInput;
    bool dInput;

    bool tPressed { get { return tInput && !oldTInput; }}

    
    // events!
    public delegate void PlayerAction();
    public static PlayerAction OnStartRoll;
    public static PlayerAction OnStopRoll;
    public static PlayerAction OnDeath;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        baseGravity = rb2d.gravityScale;
    }

    private void Update() {
        if (hazardcheck.IsColliding())
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

        // first, handle general movement state stuff
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

        if (rollTimer > 0) {
            // player should have roll velocity
            vel.x = transform.localScale.x * rollSpeed;
            
            rollTimer = Helpers.Timer(rollTimer);
            if (rollTimer == 0) {
                StoppedRoll();
            }
        }
        else if (moveState != MovementState.DUCK && xInput != 0) {
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
        if (jumpBufferTimer > 0 && coyoteTimer > 0 && moveState.CanGoToJump() && numJumpsUsed == 0) {
            vel.y = mov.jumpPower;
            StartedJump(1);
        }
        else if (mov.maxJumps == 2 && numJumpsUsed != 2 && coyoteTimer == 0 && jumpBufferTimer > 0 && moveState.CanGoToJump()) {
            vel.y = mov.jumpPower;
            StartedJump(2);
        }

        if (minimumJumpTimer == 0 && moveState == MovementState.JUMP && (!jInput || rb2d.velocity.y < 0.2f)) {
            // player's jump has terminated (either by releasing the jump button or by starting to move down)
            moveState = MovementState.FALL;
            vel.y *= mov.shortJumpMultiplier;
        }

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

        // roll check
        if (rInput && !oldRInput && moveState.CanGoToRoll() && heldTotemContainer == null) {
            StartedRoll();
        }

        rb2d.velocity = vel;
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
    }

    public void BouncedOnTrampoline(float bounceVel) {
        rb2d.velocity = new Vector2(rb2d.velocity.x, bounceVel * CurrentMovement.trampolineBounceMultiplier);
        StartedJump(1);
    }

    void StartedRoll() {
        moveState = MovementState.ROLL;
        rollTimer = rollTime;
        anim.SetBool("Rolling", true);

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

        Debug.Log("PLAYER: termination of grab animation");
    }

    void StartedThrow() {
        anim.SetTrigger("Thrown");
    }

    void StoppedThrow() {
        Debug.Log("PLAYER: termination of throw animation");
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

    private void Die() {
        OnDeath?.Invoke();
        GameManager.Instance?.PlayerDied();

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
}