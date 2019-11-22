using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendingFlower : MonoBehaviour
{
    Rigidbody2D rb2d;

    public float extendDistance;
    public float growTime;
    float growTimer;

    bool isBlooming;

    Vector3 initialPosition;
    Vector3 extendedPosition { get { return initialPosition + Vector3.up * extendDistance; }}

    Vector3 currentStartingPosition;
    Vector3 currentTargetPosition;

    public SpriteRenderer flowerSprite;
    public SpriteRenderer stalkSprite;
    public EffectCheck effectCheck;

    public Color flowerBloomColor;
    Color flowerDefaultColor;

    float defaultStalkLength;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;
        defaultStalkLength = stalkSprite.size.x;

        currentStartingPosition = initialPosition;
        currentTargetPosition = initialPosition;

        flowerDefaultColor = flowerSprite.color;
    }

    private void Update() {
        flowerSprite.color = isBlooming ? flowerBloomColor : flowerDefaultColor;
        stalkSprite.size = stalkSprite.size.WithX(defaultStalkLength + (transform.position.y - initialPosition.y));
    }

    private void FixedUpdate() {
        if (effectCheck.IsColliding() != isBlooming) {
            isBlooming = !isBlooming;
            StartGrowth(isBlooming);
        }

        Vector3 desiredPosition = Vector3.Lerp(currentTargetPosition, currentStartingPosition, Mathf.Pow(growTimer / growTime, 3));
        growTimer = Helpers.Timer(growTimer);

        rb2d.velocity = (desiredPosition - transform.position) / Time.fixedDeltaTime;
    }

    private void StartGrowth(bool shouldBloom) {
        currentStartingPosition = transform.position;
        currentTargetPosition = shouldBloom ? extendedPosition : initialPosition;
        growTimer = growTime;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * extendDistance);
    }
}
