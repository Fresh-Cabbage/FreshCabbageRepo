using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public delegate void TotemAction();
    public static TotemAction OnLand;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (Vector2.Angle(Vector2.up, collision.contacts[0].normal) < 10f) {
            OnLand();
        }
    }
}
