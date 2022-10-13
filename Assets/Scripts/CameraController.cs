using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera Panning
    public float panSpeed = 30f;
    public Vector2 panLimit;

    // Camera Zooming
    public float zoomSpeed = 0.5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public void Update()
    {
        Vector3 pos = transform.position;

        // Drag Camera with middle mouse drag
        if (Input.GetMouseButton(2))
        {
            pos -= transform.right * Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            pos -= transform.up * Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
        }

        // Pan Camera with arrow keys
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            pos += transform.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            pos -= transform.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            pos += transform.right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            pos -= transform.right * panSpeed * Time.deltaTime;
        }

        // move camera
        transform.position = pos;
        

        // Camera Zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        var zoom = Camera.main.orthographicSize;
        zoom -= scroll * 1000 * zoomSpeed * Time.deltaTime;

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        Camera.main.orthographicSize = zoom;
    }


}
