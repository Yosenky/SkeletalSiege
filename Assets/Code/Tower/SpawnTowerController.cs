using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTowerController : MonoBehaviour
{
    // Public or Serialized Fields for GameObject to spawn and the spawn interval
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int player;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnDistance = 3f;

    private bool isActive = false;
    private void Start()
    {

    }

    // Coroutine to spawn objects at set intervals
    private IEnumerator SpawnObjectRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval

            Vector3 spawnpoint = transform.position + (player==1 ? Vector3.right * spawnDistance : Vector3.left * spawnDistance);
            if (objectToSpawn != null)
            {
                // Instantiate the object at the tower's position (+ a slight offset if necessary)
                Instantiate(objectToSpawn, spawnpoint, Quaternion.identity);
            }
        }
    }

    public void ActivateTower()
    {
        isActive = true;
        StartCoroutine(SpawnObjectRoutine());
    }
}
