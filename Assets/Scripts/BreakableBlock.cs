using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public CollisionCheck playerCheck;

    private void Update() {
        GameObject player = playerCheck.GetCollider()?.gameObject;
        if (player != null && player.layer == LayerMask.NameToLayer("PlayerRoll") && Mathf.Abs(player.transform.position.y - transform.position.y) <= 0.25f)
            Break();
    }


    private void Break() {
        // have a brief delay before actually breaking so that it looks as if the player goes slightly into the block before destroying it
        Destroy(gameObject, 0.02f);
    }
}
