using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public float panSpeed = 20f;  // Camera movement speed
    public float panBorderThickness = 10f;  // Thickness of screen edge detection
    public Vector2 panLimit;  // Limit to how far the camera can move (x and z)

    public float scrollSpeed = 20f;  // Zoom in/out speed
    public float minY = 10f;  // Min height for camera zoom
    public float maxY = 50f;  // Max height for camera zoom

    void Update()
    {
        Vector3 pos = transform.position;

        // Move camera based on mouse position on screen edges
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // Zoom in/out with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        // Clamp camera movement to within the map limits
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
