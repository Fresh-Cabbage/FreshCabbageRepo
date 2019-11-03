using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    public GameObject finishExplosion;
    public GameObject finishLight;
    
    public void FinishLevel() {
        GameObject.Instantiate(finishExplosion, transform.position, Quaternion.identity);
        // GameObject.Instantiate(finishLight, transform.position, Quaternion.identity);

        GameManager.Instance?.CompletedLevel();
    }
}
