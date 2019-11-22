using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour {

    public static Managers Instance { get; private set; }
    public static LevelManager LevelManager { get; private set; }


    private void Awake() {
        // enforce singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        LevelManager = GetComponent<LevelManager>();
    }

}