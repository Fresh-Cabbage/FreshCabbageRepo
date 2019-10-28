using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script is outdated!
public class BreakableBlockContainer : MonoBehaviour
{
    PlatformEffector2D effector;


    private void Start() {
        effector = GetComponent<PlatformEffector2D>();
    }

    private void OnEnable() {
        PlayerController.OnStartRoll += EnablePlayerBreaking;
        PlayerController.OnStopRoll += DisablePlayerBreaking;
    }
    private void OnDisable() {
        PlayerController.OnStartRoll -= EnablePlayerBreaking;
        PlayerController.OnStopRoll -= DisablePlayerBreaking;
    }


    // I expect to encounter a bug with this system later.
    // If the player rolls at the same moment another object attempts to collide with a breakable block on the side,
    // the object will go through the breakable block because the effector will be on.
    // There needs to be some way for the platform effector to ONLY work on the player, but as far as i could find there was not anything like that.
    void EnablePlayerBreaking() {
        effector.useOneWay = true;
    }
    void DisablePlayerBreaking() {
        effector.useOneWay = false;
    }
}
