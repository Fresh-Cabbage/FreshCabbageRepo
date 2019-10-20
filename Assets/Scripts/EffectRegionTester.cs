using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRegionTester : MonoBehaviour
{
    public EffectRegion effectRegion;

    void Start()
    {
        effectRegion.Inflate();
    }
}
