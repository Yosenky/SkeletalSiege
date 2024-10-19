using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSBarbarianController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navAgent;
    public GameObject selectionIndicator;
    public GameObject attackTarget;
    public GameObject projectilePrefab;

    // Configuration
    public float attackDistance;
    public float attackDelay;

    // State Tracking
    float _timeSinceLastAttack;

    // Methods
    public void SetTarget(GameObject target)
    {
        attackTarget = target;
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
    }

    // Update is called once per frame
    void Update()
    {
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
            // Prepare to attack
            SetDestination(attackTarget.transform.position);
            
            if (Vector3.Distance(attackTarget.transform.position, transform.position) <= attackDistance)
            {
                navAgent.isStopped = true;

                // Perform Attack
                if (_timeSinceLastAttack >= attackDelay)
                {
                    // Reset attack timer
                    _timeSinceLastAttack = 0;

                    animator.SetTrigger("Attack");
                }
            }
        }
    }

    public void SetDestination(Vector3 targetPosition)
    {
        navAgent.destination = targetPosition;
    }
}
