using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemContainer : MonoBehaviour
{
    public Totem held;
    public Totem unheld;
    public EffectRegion effectRegion;

    public bool isHeld;


    private void Start() {
        held.isHeld = true;
        unheld.isHeld = false;
        held.parent = this;
        unheld.parent = this;
    }

    private void FixedUpdate() {
        MoveEffectRegion();
    }

    void MoveEffectRegion() {
        // the effect region does not have physics so we will manually set it to the right spot
        effectRegion.transform.position = (isHeld ? held : unheld).transform.position;
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

        MoveEffectRegion(); // avoid tearing
        effectRegion.Inflate();
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

        MoveEffectRegion(); // avoid tearing
        effectRegion.Deflate();
    }
}