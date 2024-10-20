using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSUnitController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navAgent;
    public GameObject selectionIndicator;
    public GameObject attackTarget;
    public GameObject projectilePrefab;

    public float health = 100f;

    // Configuration
    public float meleeAttackDistance;   // Melee units attack when within this distance
    public float rangedAttackDistance;  // Ranged units stop and attack at this distance
    public float attackDelay;           // Delay between attacks
    public float projectileSpeed = 10f; // For ranged units
    public float moveStoppingDistance = 0.5f; // Normal stopping distance for movement
    public float meleeDamage = 30f;     // Melee attack damage

    public int team = 1;  // Team 1 friendly, Team 2 Enemy

    // State Tracking
    float _timeSinceLastAttack;

    // Methods
    public void SetTarget(GameObject target)
    {
        // Check if the target is not null
        if (target == null)
        {
            return;  // Exit if the target is null
        }

        // Check if the target has an RTSUnitController component
        RTSUnitController targetUnit = target.GetComponent<RTSUnitController>();
        if (targetUnit != null && targetUnit.team != this.team)
        {
            // Set the attack target if it's from a different team
            attackTarget = target;
        }
        else
        {
            SetDestination(target.transform.position);
        }
    }

    public void Select()
    {
        selectionIndicator.SetActive(true);
    }

    public void Deselect()
    {
        selectionIndicator.SetActive(false);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = moveStoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            Die();
            return; // Stop further logic if dead
        }

        // Check if character is close to its destination and stop moving
        if (!navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                // Ensure velocity is low before stopping movement animation
                if (navAgent.velocity.sqrMagnitude <= 0.01f)
                {
                    animator.SetFloat("velocity", 0f); // Idle animation
                }
            }
            else
            {
                // Character is still moving, play running animation
                animator.SetFloat("velocity", navAgent.velocity.magnitude); // Running animation
            }
        }

        // Increment attack timer
        _timeSinceLastAttack += Time.deltaTime;

        // Prepare to get in range of target
        if (attackTarget)
        {
            float distanceToTarget = Vector3.Distance(attackTarget.transform.position, transform.position);

            // Logic for melee units
            if (CompareTag("Melee"))
            {
                HandleMeleeAttack(distanceToTarget);
            }
            // Logic for ranged units
            else if (CompareTag("Range"))
            {
                HandleRangedAttack(distanceToTarget);
            }
        }
    }

    // Logic for melee unit attacks
    void HandleMeleeAttack(float distanceToTarget)
    {
        // Move toward target, keep moving until within melee attack distance
        if (distanceToTarget > meleeAttackDistance)
        {
            navAgent.isStopped = false;  // Keep moving if not close enough
            navAgent.stoppingDistance = moveStoppingDistance; // Normal movement stopping distance
            SetDestination(attackTarget.transform.position);
            animator.SetBool("attack", false);  // Not attacking while moving
        }
        else
        {
            // Stop and attack when within melee attack distance
            navAgent.isStopped = true;  // Stop moving to start attacking
            animator.SetBool("attack", true);  // Start attack animation

            // Perform melee attack if the delay has passed
            if (_timeSinceLastAttack >= attackDelay)
            {
                _timeSinceLastAttack = 0;  // Reset attack timer
                
                RTSUnitController enemyUnit = attackTarget.GetComponent<RTSUnitController>();
                if (enemyUnit != null && enemyUnit.team != this.team)  // Only deal damage to enemy units
                {
                    enemyUnit.TakeDamage(meleeDamage);  // Deal melee damage to the enemy
                }
            }
        }
    }

    // Logic for ranged unit attacks
    void HandleRangedAttack(float distanceToTarget)
    {
        // Move toward target, but only stop at the ranged attack distance
        if (distanceToTarget > rangedAttackDistance)
        {
            navAgent.isStopped = false;  // Keep moving if not close enough
            navAgent.stoppingDistance = moveStoppingDistance; // Normal movement stopping distance
            SetDestination(attackTarget.transform.position);
            animator.SetBool("attack", false);  // Not attacking while moving
        }
        else
        {
            // Stop and attack when within ranged attack distance
            navAgent.isStopped = true;  // Stop moving to start attacking
            animator.SetBool("attack", true);  // Start attack animation

            // Perform ranged attack if the delay has passed
            if (_timeSinceLastAttack >= attackDelay)
            {
                _timeSinceLastAttack = 0;  // Reset attack timer
                FireProjectile();  // Fire the projectile for ranged units
            }
        }
    }

    // Take damage when attacked
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Current health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    public void SetDestination(Vector3 targetPosition)
    {
        attackTarget = null;  // Clear the attack target since we're moving
        navAgent.stoppingDistance = moveStoppingDistance;  // Use normal stopping distance for movement
        navAgent.isStopped = false;  // Resume movement
        navAgent.destination = targetPosition;

        // Stop the attack animation by setting the bool to false
        animator.SetBool("attack", false);
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && attackTarget != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);

            UnitProjectiles projectileScript = projectile.GetComponent<UnitProjectiles>();
            projectileScript.team = this.team;  // Set the projectile's team to the firing unit's team

            Vector3 directionToTarget = (attackTarget.transform.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody>().velocity = directionToTarget * projectileSpeed;
            Destroy(projectile, 5f);
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        // Play death animation, disable components, etc.
        animator.SetTrigger("die");
        navAgent.isStopped = true;
        // Disable the unit or destroy it after some time
        Destroy(gameObject, 2f); // Optional: destroy after 2 seconds
    }
}
