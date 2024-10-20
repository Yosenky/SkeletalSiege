using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private int PlayerID;

    private GameObject currentTower;

    private Ray lastRay;
    private Vector3 lastHitPoint;
    private bool hasHit;
    private int currentTowerCost;

    // Start is called before the first frame update
    void Start()
    {
        currentTowerCost = 0;
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


            if (Input.GetMouseButtonDown(0))
            {
                AttackTowerController AttackTower = currentTower.GetComponent<AttackTowerController>();
                SpawnTowerController SpawnTower = currentTower.GetComponent<SpawnTowerController>();
                if (AttackTower != null)
                {
                    AttackTower.ActivateTower();
                }
                else if(SpawnTower != null)
                {
                    SpawnTower.ActivateTower();
                }
                else
                {
                    Debug.Log("No valid tower");
                    return;
                }
                currentTower = null;
            }
        }

        
    }

    public void SetCurrenTowerCost(int towerCost)
    {
        currentTowerCost = towerCost;
    }

    public void SetCurrentTower(GameObject tower)
    {
        if (tower == null)
        {
            Debug.LogError("Tower prefab is null.");
            return;
        }

        AttackTowerController AttackTower = tower.GetComponent<AttackTowerController>();
        SpawnTowerController SpawnTower = tower.GetComponent<SpawnTowerController>();

        

        if(currentTowerCost > 0 && ResourceController.instance.GetGold(RTSGameController.instance.team) >= currentTowerCost)
        {
            Quaternion towerRotation = tower.transform.rotation; // Capture the prefab's rotation
            currentTower = Instantiate(tower, Vector3.zero, towerRotation); // Spawn with prefab's rotation
            ResourceController.instance.SpendGold(RTSGameController.instance.team, currentTowerCost);
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
        
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