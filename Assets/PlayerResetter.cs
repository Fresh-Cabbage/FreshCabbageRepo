using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResetter : MonoBehaviour
{
    public float resetHoldTime;
    float resetTimer;

    public float resetShakeIntensity;

    Vector3 previousShakeOffset;

    public PlayerController player;

    private void Start() {
        resetTimer = resetHoldTime;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Reset") > 0) {
            resetTimer = Helpers.Timer(resetTimer);
        } else {
            resetTimer = Mathf.Min(resetHoldTime, resetTimer + Time.deltaTime * 4);
        }

        transform.position -= previousShakeOffset;

        if (resetTimer != resetHoldTime) {
            Vector3 newShakeOffset = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector3.right * resetShakeIntensity * (1 - resetTimer / resetHoldTime);
            transform.position += newShakeOffset;
            previousShakeOffset = newShakeOffset;
        
            if (resetTimer == 0) {
                player.Die();
            }
        } else {
            previousShakeOffset = Vector3.zero;
        }
    }
}
