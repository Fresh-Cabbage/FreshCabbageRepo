using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParallax : MonoBehaviour
{
    GameObject camera;
    Vector3 cameraPrevPos;

    public float parallaxAmount;


    // Start is called before the first frame update
    void Start()
    {
        camera = GameManager.Instance?.MainCamera?.gameObject;
        cameraPrevPos = camera.transform.position;
    }

    void FixedUpdate()
    {
        if (camera == null)
            return;
        
        Vector3 diffPos = camera.transform.position - cameraPrevPos;
        transform.position += diffPos * parallaxAmount;
        cameraPrevPos = camera.transform.position;
    }
}
