using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberMill : MonoBehaviour
{

    // On trigger, give unit lumber
    private void OnTriggerEnter(Collider other)
    {
        print("TRIGGER");
        LumberJackController lumberJack = other.gameObject.GetComponent<LumberJackController>();

        if (lumberJack != null)
        {
            lumberJack.CollectLumber();
        }
    }

}
