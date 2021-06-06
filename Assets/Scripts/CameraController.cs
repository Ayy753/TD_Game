using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to camera, used to pan and zoom camera
/// </summary>
public class CameraController : MonoBehaviour
{
    private const float panSpeed = 30f;
    private const float scrollSpeed = 5f;
    private const int minZoom = 5;
    private const int maxZoom = 75;

    private float horizontalPan;
    private float verticalPan;

    void Update()
    {
        horizontalPan = Input.GetAxisRaw("Horizontal");
        verticalPan = Input.GetAxisRaw("Vertical");

        if (horizontalPan < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, panSpeed * Time.unscaledDeltaTime);
        }
        else if (horizontalPan > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, panSpeed * Time.unscaledDeltaTime);
        }

        if (verticalPan < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, panSpeed * Time.unscaledDeltaTime);
        }
        else if (verticalPan > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, panSpeed * Time.unscaledDeltaTime);
        }

        //  Scroll wheel zoom
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + -Input.mouseScrollDelta.y * scrollSpeed, minZoom, maxZoom);
    }
}
