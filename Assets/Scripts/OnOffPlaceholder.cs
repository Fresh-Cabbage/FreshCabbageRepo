using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffPlaceholder : MonoBehaviour
{
    SpriteRenderer sr;
    CollisionCheck effectCheck;
    public Collider2D surface;

    public bool defaultState;

    public Color offColor;
    public Color onColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        effectCheck = GetComponent<CollisionCheck>();
        surface = GetComponent<Collider2D>();
    }

    private void Update()
    {
        sr.color = effectCheck.IsColliding ? onColor : offColor;
        if(effectCheck.IsColliding ? !defaultState : defaultState)
        {
            surface.enabled = true;
        }
    }
}
