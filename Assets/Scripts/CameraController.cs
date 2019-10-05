using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // camera will be connected to the player another way when scene flow starts being implemented
    
    public float lerpSpeed;

    Vector3 targetPosition;

    private void FixedUpdate() {
        if (player != null)
            targetPosition = player.transform.position.WithZ(transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
    }
}
