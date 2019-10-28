using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    bool completed;
    
    public void FinishLevel() {
        // right now this method is very minimal
        // but we expect a lot more work to be done when the level is finished in the future
        completed = true;
        
        GameManager.Instance?.CompletedLevel();
    }
}
