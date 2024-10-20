using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target; // The target the projectile will fly towards
    public float speed = 2f; // Speed at which the projectile will move

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            // Move the projectile towards the target
            transform.position += directionToTarget * speed * Time.deltaTime;

            // Rotate the projectile to face the target
            transform.LookAt(target.transform);
        }
        else
        {
            // Move the projectile forward
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (target != null && other.gameObject == target)
        {
            Debug.Log("Bullet hit: " + other.gameObject.name);
            // Handle hitting the target, e.g., apply damage
            // For now, just destroy both
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
  
}
