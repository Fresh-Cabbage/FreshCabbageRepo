using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffPlaceholder : MonoBehaviour
{
    SpriteRenderer sr;
    public EffectCheck effectCheck;
    public Collider2D surface;

    public bool defaultState;

    public Color offColor;
    public Color onColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        surface = GetComponent<Collider2D>();
    }

    private void Update()
    {
        sr.color = effectCheck.IsColliding() ? onColor : offColor;
        surface.enabled = effectCheck.IsColliding() ? !defaultState : defaultState;
    }
}
