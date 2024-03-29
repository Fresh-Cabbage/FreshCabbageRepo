﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderSwitch : MonoBehaviour
{
    SpriteRenderer sr;
    EffectCheck effectCheck;
    public GameObject door;

    public bool defaultState;

    public Color offColor;
    public Color onColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        effectCheck = GetComponent<EffectCheck>();
    }

    private void Update() {
        sr.color = effectCheck.IsColliding() ? onColor : offColor;
        door.SetActive(effectCheck.IsColliding() ? !defaultState : defaultState);
    }
}
