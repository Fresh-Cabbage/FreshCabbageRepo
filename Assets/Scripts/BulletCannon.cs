using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCannon : MonoBehaviour
{
    public bool defaultOn;

    public float fireInterval;
    public float fireSpeed;
    float fireTimer;

    public Transform axis;
    public Transform tip;

    public GameObject bullet;

    public EffectCheck electricCheck;
    
    Animator anim;


    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if (defaultOn != electricCheck.IsColliding()) {
            fireTimer = Helpers.Timer(fireTimer);
            
            if (fireTimer == 0) {
                Fire();
            }
        }
    }

    private void Fire() {
        GameObject b = GameObject.Instantiate(bullet, tip.position, Quaternion.identity);
        b.GetComponent<Bullet>()?.SetDirection(axis.rotation, fireSpeed);
        
        fireTimer = fireInterval;
    
        anim.SetTrigger("Fired");
    }

}
