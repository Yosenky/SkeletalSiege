using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    private GameObject currentTower;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTower != null){
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(camray, out RaycastHit hitInfo, 100f)){
                currentTower.transform.position = hitInfo.point;
            }
        }

        if(Input.GetMouseButtonDown(0)){
            currentTower = null;
        }
    }

    public void SetCurrentTower(GameObject tower){
        currentTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
    }
}
