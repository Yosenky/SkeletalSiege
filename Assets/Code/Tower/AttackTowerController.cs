using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerController : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    private GameObject currentTarget;
    private float lastShootTime;
    private bool isActive = false;

    void Start()
    {
        lastShootTime = -shootInterval; // To allow immediate shooting on game start if needed
    }

    void Update()
    {
        if (!isActive) return;
        FindTarget();
        if (currentTarget != null)
        {
            TurnTowardsTarget();
            if (Time.time >= lastShootTime + shootInterval)
            {
                ShootProjectile();
                lastShootTime = Time.time; // Update the last shoot time
            }
        }
    }

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        currentTarget = null;
        float closestDistance = detectionRange;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                currentTarget = enemy;
            }
        }
    }

    private void TurnTowardsTarget()
    {
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        directionToTarget.y = 0; // Maintain direction only on the X-Z plane

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

            // Set the target for the projectile
            BulletController projectileScript = projectile.GetComponent<BulletController>();
            if (projectileScript != null)
            {
                projectileScript.target = currentTarget;
            }
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Shoot Point not set");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!isActive) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void ActivateTower()
    {
        isActive = true;
    }
}