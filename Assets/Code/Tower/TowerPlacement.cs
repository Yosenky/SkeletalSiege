using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private LayerMask placementLayerMask;
    private GameObject currentTower;

    private Ray lastRay;
    private Vector3 lastHitPoint;
    private bool hasHit;

    // Start is called before the first frame update
    void Start()
    {
        // Optional: initialize any startup logic here.
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTower != null)
        {
            Ray camRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            lastRay = camRay; // Store the ray for visualization

            if (Physics.Raycast(camRay, out RaycastHit hitInfo, 100f, placementLayerMask))
            {
                lastHitPoint = hitInfo.point; // Store the hit point for visualization
                hasHit = true;
                currentTower.transform.position = hitInfo.point;
            }
            else
            {
                hasHit = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            currentTower = null;
        }
    }

    public void SetCurrentTower(GameObject tower)
    {
        currentTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        // Visualizing the ray
        if (lastRay.origin != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastRay.origin, lastRay.origin + lastRay.direction * 100f); // Draw ray

            // Visualizing the hit point
            if (hasHit)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(lastHitPoint, 0.5f); // Draw hit point
            }
        }
    }
}