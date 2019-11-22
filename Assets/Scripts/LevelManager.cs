using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Managers))]
public class LevelManager : MonoBehaviour
{
    PlayerController player;
    WorldLight worldLight;
    public Camera MainCamera { get; private set; }

    public int previousCheckpoint;

    private void OnEnable() {
        // subscribe GameManager to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        // unsubscribed GameManager to the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start() {
        previousCheckpoint = -1;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player == null)
            Debug.LogWarning("No player in this scene!");

        worldLight = GameObject.FindGameObjectWithTag("WorldLight")?.GetComponent<WorldLight>();
        if (worldLight == null) 
            Debug.LogWarning("No world light in this scene!");

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
        if (MainCamera == null)
            Debug.LogWarning("No camera in this scene!");
        
        worldLight?.FadeIn(0.5f);
    }

    public void SpawnPlayerAtPosition(Vector2 position) {
        Debug.Log("GAMEMANAGER: spawnplayer happens " + player);
        player.transform.position = position;
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

        previousCheckpoint = -1;
        
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1, 2.0f));
        StartCoroutine(FadeOut(1.5f, 0.5f));
    }
}