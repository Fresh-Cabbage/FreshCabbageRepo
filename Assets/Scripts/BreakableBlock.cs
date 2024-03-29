﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public CollisionCheck playerCheck;
    new Collider2D collider;

    public float freezeTime;
    public float breakDelay;

    bool destroyed;

    public GameObject breakParticles;

    public Material white;

    private void Start() {
        collider = GetComponent<Collider2D>();
    }

    private void Update() {
        if (!destroyed && playerCheck.IsColliding()) {
            StartCoroutine(Break(true));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Bullet b = other.GetComponent<Bullet>();
        if (b != null) {
            b.DestroyBullet();
            StartCoroutine(Break(false));
        }
    }


    private IEnumerator Break(bool shouldFreezeFrame) {
        destroyed = true;

        // disable this collision
        collider.enabled = false;

        yield return new WaitForSeconds(breakDelay);

        GetComponent<SpriteRenderer>().material = white;
        
        // freeze frame
        if (shouldFreezeFrame) {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(freezeTime);
            Time.timeScale = 1;
        } else {
            yield return 0;
        }

        // spawn particles
        GameObject.Instantiate(breakParticles, transform.position, Quaternion.identity);

        // finally, destroy this
        Destroy(gameObject);
    }
}
