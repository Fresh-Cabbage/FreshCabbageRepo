using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemContainer : MonoBehaviour
{
    public Totem held;
    public Totem unheld;

    public bool isHeld;


    private void Start() {
        held.isHeld = true;
        unheld.isHeld = false;
        held.parent = this;
        unheld.parent = this;
    }

    public void HoldTotem(Transform newParent, Vector3 holdOffset) {
        if (isHeld) {
            Debug.LogError("Can't hold totem that is already being held");
            return;
        }

        isHeld = true;

        held.gameObject.SetActive(true);
        unheld.gameObject.SetActive(false);

        transform.position = newParent.position + holdOffset;
        transform.SetParent(newParent);
    }

    public void ReleaseTotem(Vector2 throwDirection) {
        if (!isHeld) {
            Debug.LogError("Can't release totem that is not being held");
            return;
        }

        isHeld = false;

        held.gameObject.SetActive(false);
        unheld.gameObject.SetActive(true);

        unheld.transform.position = held.transform.position;
        transform.SetParent(null);
        unheld.Throw(throwDirection);
    }
}