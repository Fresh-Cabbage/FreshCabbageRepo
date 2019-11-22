using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Checkpoint currentCheckpoint;
    
    
    private void Start() {
        int checkpointCounter = 0;

        foreach (Transform t in transform) {
            Checkpoint c = t.GetComponent<Checkpoint>();
            if (c != null) {
                c.checkpointManager = this;
                c.checkpointNumber = checkpointCounter;

                if (c.checkpointNumber == Managers.LevelManager?.previousCheckpoint) {
                    Managers.LevelManager.SpawnPlayerAtPosition(c.respawnPosition);
                    currentCheckpoint = c;
                    c.SetActivated(true);
                } else {
                    c.SetActivated(false);
                }

                checkpointCounter++;
            }
        }
    }


    public void TryActivateCheckpoint(Checkpoint checkpoint) {
        if (currentCheckpoint == checkpoint) return;

        // de-activate current checkpoint
        currentCheckpoint?.SetActivated(false);

        // activate new checkpoint
        currentCheckpoint = checkpoint;
        currentCheckpoint.SetActivated(true);

        if (Managers.LevelManager != null)
            Managers.LevelManager.previousCheckpoint = currentCheckpoint.checkpointNumber;
    }
}
