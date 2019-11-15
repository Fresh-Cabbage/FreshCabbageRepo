using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendingFlower : MonoBehaviour
{
    Rigidbody2D rb2d;

    public float extendDistance;

    Vector3 initialPosition;
    Vector3 extendedPosition { get { return initialPosition + Vector3.up * extendDistance; }}

    public SpriteRenderer flowerSprite;
    public SpriteRenderer stalkSprite;

    float defaultStalkLength;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;
        defaultStalkLength = stalkSprite.size.x;
        Debug.Log("EXTENDINGFLOWER: size " + defaultStalkLength);
    }

    private void FixedUpdate() {
        // for now just extend by default
        Vector3 desiredPosition = Vector3.Lerp(transform.position, extendedPosition, 0.01f);
        rb2d.velocity = (desiredPosition - transform.position) / Time.fixedDeltaTime;
        Debug.Log("EXTENDINGFLOWER: " + (desiredPosition - transform.position));

        stalkSprite.size = stalkSprite.size.WithX(defaultStalkLength + (transform.position.y - initialPosition.y));
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * extendDistance);
    }
}
