using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector2 respawnOffset;
    [HideInInspector] public int checkpointNumber;

    public Vector3 respawnPosition { get { return transform.position + respawnOffset.ToVector3(); }}

    public CheckpointManager checkpointManager;

    SpriteRenderer sr;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            // checkpoint triggered
            checkpointManager.TryActivateCheckpoint(this);
        }
    }

    public void SetActivated(bool activated) {
        if (activated)
            sr.color = Color.green.WithAlpha(sr.color.a);
        else
            sr.color = Color.gray.WithAlpha(sr.color.a);
    }


    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0.5f, 0.3f);
        Gizmos.DrawRay(transform.position, respawnOffset);
        Gizmos.DrawWireSphere(respawnPosition, 0.5f);
    }
}
