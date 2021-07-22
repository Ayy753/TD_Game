using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Attach to camera, used to pan and zoom camera
/// </summary>
public class CameraController : MonoBehaviour {
    [Inject] TargetManager targetManager;

    private const float panSpeed = 30f;
    private const float scrollSpeed = 5f;
    private const int minZoom = 5;
    private const int maxZoom = 75;

    private float horizontalPan;
    private float verticalPan;

    private Transform focusedTarget;

    private void OnEnable() {
        InputHandler.OnCommandEntered += HandleCommandEntered;
        MouseManager.OnRightMouseUp += HandleRightButtonUp;
    }

    private void OnDisable() {
        InputHandler.OnCommandEntered -= HandleCommandEntered;
        MouseManager.OnRightMouseUp -= HandleRightButtonUp;
    }

    void Update() {
        horizontalPan = Input.GetAxisRaw("Horizontal");
        verticalPan = Input.GetAxisRaw("Vertical");

        //  If user is holding down a pan key
        if (horizontalPan != 0 || verticalPan != 0) {
            if (horizontalPan < 0) {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, panSpeed * Time.unscaledDeltaTime);
            }
            else if (horizontalPan > 0) {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, panSpeed * Time.unscaledDeltaTime);
            }

            if (verticalPan < 0) {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, panSpeed * Time.unscaledDeltaTime);
            }
            else if (verticalPan > 0) {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, panSpeed * Time.unscaledDeltaTime);
            }

            //  Clear focus target
            focusedTarget = null;
        }
        //  Otherwise if user is focusing on a target
        else if (focusedTarget != null) {
            //  If focused target becomes inactive, clear it to prevent reused enemies from being focused when respawned
            if (focusedTarget.gameObject.activeInHierarchy == false) {
                focusedTarget = null;
            }
            else {
                Vector3 position = focusedTarget.position;
                transform.position = new Vector3(focusedTarget.position.x, focusedTarget.position.y, transform.position.z);
            }
        }

        //  Scroll wheel zoom
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + -Input.mouseScrollDelta.y * scrollSpeed, minZoom, maxZoom);
    }

    /// <summary>
    /// Follows target if one exists
    /// </summary>
    /// <param name="command"></param>
    private void HandleCommandEntered(InputHandler.Command command) {
        if (command == InputHandler.Command.FollowTarget) {
            Itargetable target = targetManager.GetTarget();

            if (target != null) {
                focusedTarget = target.GetTransform();
            }
        }
    }

    /// <summary>
    /// Clears focused target
    /// </summary>
    private void HandleRightButtonUp() {
        focusedTarget = null;
    }
}
