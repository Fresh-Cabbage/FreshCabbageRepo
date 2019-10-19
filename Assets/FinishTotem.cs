using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTotem : MonoBehaviour
{
    public GameObject[] finishScreen;

    // Start is called before the first frame update
    void Start()
    {
        finishScreen = GameObject.FindGameObjectsWithTag("FinishScreen");
        foreach (GameObject obj in finishScreen)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (GameObject obj in finishScreen)
            {
                obj.SetActive(true);
            }
        }
    }

}
