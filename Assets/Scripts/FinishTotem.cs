using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is not yet a totem: we will fix this later
public class FinishTotem : MonoBehaviour
{
    public GameObject finishScreen;

    bool completed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (completed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // right now this is the win condition
            completed = true;

            GameManager.Instance?.CompletedLevel();

            finishScreen.SetActive(true);
        }
    }

}
