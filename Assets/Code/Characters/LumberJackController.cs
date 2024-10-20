using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LumberJackController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navAgent;
    public GameObject selectionIndicator;

    public float health = 100f;

    // Configuration
    public float moveStoppingDistance = 0.5f; // Normal stopping distance for movement
    public int team = 1;  // Team 1 friendly, Team 2 Enemy
    public bool hasLumber = false;
    private SkinnedMeshRenderer hoodRenderer;


    // Methods
    public void SetTarget(GameObject target)
    {
        // Check if the target is not null
        if (target == null)
        {
            return;  // Exit if the target is null
        }
        SetDestination(target.transform.position);
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
        hoodRenderer = transform.Find("Rogue_Head_Hooded").GetComponent<SkinnedMeshRenderer>();
        ChangeAppearance(hasLumber);
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
        navAgent.stoppingDistance = moveStoppingDistance;  // Use normal stopping distance for movement
        navAgent.isStopped = false;  // Resume movement
        navAgent.destination = targetPosition;

        // Stop the attack animation by setting the bool to false
        animator.SetBool("attack", false);
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

    public void CollectLumber()
    {
        hasLumber = true;
        ChangeAppearance(hasLumber);
    }

    public void DepositLumber()
    {
        hasLumber = false;
        // give money to player
        ChangeAppearance(hasLumber);
    }

    void ChangeAppearance(bool hasLumber)
    {
        if (hasLumber)
        {
            
            if (hoodRenderer != null)
            {
                // Change the material color to red (or any other color)
                hoodRenderer.material.color = Color.red;
            }
        }
        else 
        { 
            // Check if the renderer is found
            if (hoodRenderer != null)
            {
                // Change the material color to red (or any other color)
                hoodRenderer.material.color = Color.blue;
            }
        }
    } 

}
