﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleStuff : MonoBehaviour
{
    //A class to hold functions for title stuff
    //e.g. starting a game, loading a game, quitting

    public void StartGame(string sceneName)
    {
        /**
         * NOTE TO DESIGNERS:
         * you must put sceneName exactly how it's spelled in Unity.
         * Also, this function will not run unless the specified scene
         * has been added to the build.
         */
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit;
#endif

    }
}
