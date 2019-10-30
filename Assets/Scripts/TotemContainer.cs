using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemContainer : MonoBehaviour
{
    public Totem held;
    public Totem unheld;
    public EffectRegion effectRegion;
    public List<GameObject> objectsUnderTotem;

    public bool isHeld;


    private void Start() {
        held.isHeld = true;
        unheld.isHeld = false;
        held.parent = this;
        unheld.parent = this;

        SetParents();
        SetEffectRegionState();
    }


    private void SetParents() {
        foreach (GameObject g in objectsUnderTotem) {
            g.transform.SetParent(isHeld ? held.transform : unheld.transform);
            g.transform.localPosition = Vector3.zero;
        }
    }

    private void SetEffectRegionState() {
        if (isHeld) {
            effectRegion.Inflate();
        } else {
            effectRegion.Deflate();
        }
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

        SetParents();
        SetEffectRegionState();

        if (effectRegion.regionType == EffectRegionType.WIN) {
            // the player wins by grabbing this totem!
            LevelFinisher finish = GetComponent<LevelFinisher>();

            if (finish == null)
                Debug.LogError("This is a win totem but there is no LevelFinisher attached!");

            finish?.FinishLevel();
        }
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

        SetParents();
        SetEffectRegionState();
    }
}