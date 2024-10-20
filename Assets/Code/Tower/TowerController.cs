using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    // Public or Serialized Fields for GameObject to spawn and the spawn interval
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnDistance = 3f;

    private void Start()
    {
        // Start the Spawn Coroutine
        StartCoroutine(SpawnObjectRoutine());
    }

    // Coroutine to spawn objects at set intervals
    private IEnumerator SpawnObjectRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval

            if (objectToSpawn != null)
            {
                // Instantiate the object at the tower's position (+ a slight offset if necessary)
                Instantiate(objectToSpawn, transform.position + Vector3.forward*spawnDistance, Quaternion.identity);
            }
        }
    }
}
