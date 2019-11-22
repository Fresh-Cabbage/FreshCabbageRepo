using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBlock : MonoBehaviour
{
    public SpriteRenderer sr;
    public EffectCheck effectCheck;
    public Collider2D surface;
    Animator anim;

    public bool defaultState;

    private void Start()
    {
        surface = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        bool active = effectCheck.IsColliding() ? !defaultState : defaultState;

        surface.enabled = active;
        anim.SetBool("Active", active);
    }
}
