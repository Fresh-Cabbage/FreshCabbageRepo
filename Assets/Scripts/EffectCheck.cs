using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectCheck : CollisionCheck
{
    public EffectRegionType regionType;


    protected override void Start() {
        base.Start();

        // overwrite target tags
        targetTags = new List<string>() {"EffectRegion"};
    }

    protected override bool DefaultValidCheck(Collider2D other) {
        return base.DefaultValidCheck(other) && (other.GetComponent<EffectRegion>()?.regionType ?? null) == regionType;
    }
    
    
}
