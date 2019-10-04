using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField] private bool isColliding;
    public bool IsColliding { get { return isColliding; }}

    public List<string> targetTags;

    
    private void OnTriggerEnter2D(Collider2D other) {
        if (targetTags.Any(tag => other.CompareTag(tag))) {
            isColliding = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other) => OnTriggerEnter2D(other);

    private void OnTriggerExit2D(Collider2D other) {
        if (targetTags.Any(tag => other.CompareTag(tag))) {
            isColliding = false;
        }
    }
}
