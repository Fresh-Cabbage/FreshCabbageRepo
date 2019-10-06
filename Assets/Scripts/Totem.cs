﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public delegate void TotemAction();
    public static TotemAction OnLand;


    [HideInInspector] public TotemContainer parent;
    [HideInInspector] public bool isHeld;

    [HideInInspector] public Rigidbody2D rb2d;


    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (Vector2.Angle(Vector2.up, collision.contacts[0].normal) < 10f) {
            OnLand();

            if (rb2d != null) {
                rb2d.velocity = Vector2.zero;
            }
        }
    }
}
