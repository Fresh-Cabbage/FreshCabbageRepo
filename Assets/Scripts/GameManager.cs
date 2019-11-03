using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;

    PlayerController player;
    WorldLight worldLight;

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
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player == null)
            Debug.LogWarning("No player in this scene!");

        worldLight = GameObject.FindGameObjectWithTag("WorldLight")?.GetComponent<WorldLight>();
        if (worldLight == null) 
            Debug.LogWarning("No world light in this scene!");
        
        worldLight?.FadeIn(0.5f);
    }

    IEnumerator LoadScene(string name, float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(name);
    }
    IEnumerator LoadScene(int buildIndex, float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(buildIndex);
    }

    IEnumerator FadeOut(float delay, float time) {
        yield return new WaitForSeconds(delay);
        worldLight?.FadeOut(time);
    }

    public void PlayerDied() {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, 1.0f));
        StartCoroutine(FadeOut(0.5f, 0.5f));
    }

    public void CompletedLevel() {
        if (player != null)
            player.inCutscene = true;
        if (worldLight != null)
            worldLight.Flash(2, 1);
        
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1, 2.0f));
        StartCoroutine(FadeOut(1.5f, 0.5f));
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, 0.0f));
        }
    }
}
