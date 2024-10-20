using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectiles : MonoBehaviour
{
    public float damage = 20f;  // Damage dealt by the projectile
    public int team;

    private void OnCollisionEnter(Collision collision)
    {
        RTSUnitController hitUnit = collision.gameObject.GetComponent<RTSUnitController>();

        if (hitUnit != null && hitUnit.team != team)  // Only deal damage to enemy units
        {
            // Deal damage to the enemy unit
            hitUnit.TakeDamage(damage);
            Debug.Log("Projectile hit " + collision.gameObject.name + " and dealt " + damage + " damage!");

            // Destroy the projectile on impact
            Destroy(gameObject);
        }
    }
}
