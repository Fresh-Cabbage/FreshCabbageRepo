using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static GameObject player;
    public GameObject[] totems;
    private static GameObject[] totemList;
    private static SortedDictionary<GameObject, Vector3> totemPos;
    private static Vector3 playerPos;
    public static bool checkpointsActive;
    private Collider2D thisCollider;
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        checkpointsActive = false;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player==null)
        {
            Debug.LogWarning("oh no");
        }
        totemList = totems;
        for (int i = 0; i < totemList.Length; i++)
        {
            totemPos.Add(totemList[i], totemList[i].transform.position);
        }
    }

    public static void ResetFromCheckpoint()
    {
        player.transform.position = playerPos;
        for (int i = 0; i < totemList.Length; i++)
        {
            totemList[i].transform.position = totemPos[totemList[i]];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateTotem();
    }

    public void ActivateTotem()
    {
        checkpointsActive = true;
        if (!isActive)
        {
            Debug.Log("activated");
            isActive = true;
            playerPos = player.transform.position;
            for (int i = 0; i < totemList.Length; i++)
            {
                totemPos[totemList[i]] = totemList[i].transform.position;
            }
        }
    }
}
