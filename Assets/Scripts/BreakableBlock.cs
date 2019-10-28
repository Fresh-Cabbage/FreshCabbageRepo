using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public CollisionCheck playerCheck;
    new Collider2D collider;

    public float freezeTime;
    public float breakDelay;

    bool destroyed;

    public Material white;

    private void Start() {
        collider = GetComponent<Collider2D>();
    }

    private void Update() {
        if (!destroyed && playerCheck.IsColliding()) {
            StartCoroutine(Break());
        }
    }


    private IEnumerator Break() {
        destroyed = true;

        // disable this collision
        collider.enabled = false;

        yield return new WaitForSeconds(breakDelay);

        GetComponent<SpriteRenderer>().material = white;
        
        // freeze frame
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);
        Time.timeScale = 1;

        // finally, destroy this
        Destroy(gameObject);
    }
}
