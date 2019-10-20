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

    protected override bool IsValid(Collider2D other) {
        return base.IsValid(other) && (other.GetComponent<EffectRegion>()?.regionType ?? null) == regionType;
    }
    
    
}
