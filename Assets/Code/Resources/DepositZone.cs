using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        LumberJackController lumberJack = other.gameObject.GetComponent<LumberJackController>();

        if (lumberJack != null && lumberJack.hasLumber)
        {
            lumberJack.DepositLumber();
            ResourceController.instance.AddGold(10);
        }
    }
}
