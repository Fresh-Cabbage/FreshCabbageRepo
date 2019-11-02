using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    protected List<Collider2D> colliders;

    public List<string> targetTags;


    public Func<Collider2D, bool> validCheck;


    protected virtual void Start() {
        Reset();
    }

    protected virtual void OnDisable() {
        Reset();
    }

    protected void Reset() {
        colliders = new List<Collider2D>();
    }


    protected virtual bool DefaultValidCheck(Collider2D other) {
        return other != null && other.enabled && targetTags.Any(tag => other.CompareTag(tag));
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (DefaultValidCheck(other)) {
            // found a collider, add it to the list
            colliders.Add(other);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other) {
        // remove collider if preset
        colliders.Remove(other);
    }

    public void CheckColliders() {
        for (int c = colliders.Count - 1; c >= 0; c--) {
            // remove any colliders that are no longer valid collisions
            if (!DefaultValidCheck(colliders[c]) || (validCheck != null && !validCheck(colliders[c]))) {
                colliders.RemoveAt(c);
            }
        }
    }

    public bool IsColliding() {
        CheckColliders();
        return colliders.Count > 0;
    }

    public Collider2D GetCollider() {
        return IsColliding() ? colliders[colliders.Count - 1] : null;
    }
}
