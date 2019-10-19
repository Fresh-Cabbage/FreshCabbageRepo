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

        SetEffectRegionState();
    }

    private void SetEffectRegionState() {
        if (isHeld) {
            effectRegion.Inflate();
            effectRegion.transform.SetParent(held.transform);
        } else {
            effectRegion.Deflate();
            effectRegion.transform.SetParent(unheld.transform);
        }

        effectRegion.transform.localPosition = Vector3.zero;
    }

    public void HoldTotem(Transform newParent) {
        if (isHeld) {
            Debug.LogError("Can't hold totem that is already being held");
            return;
        }

        isHeld = true;

        held.gameObject.SetActive(true);
        unheld.gameObject.SetActive(false);

        transform.position = newParent.position;
        transform.SetParent(newParent);

        SetEffectRegionState();
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

        SetEffectRegionState();
    }
}