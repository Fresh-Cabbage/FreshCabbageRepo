﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;

    private void Awake() {
        // enforce singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() {
        // subscribe GameManager to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        // unsubscribed GameManager to the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {

    }

    IEnumerator LoadScene(string name, float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(name);
    }

    public void PlayerDied() {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, 1.0f));
    }

    public void CompletedLevel() {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, 2.0f));
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, 0.0f));
        }
    }
}
