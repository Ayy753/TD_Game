using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

class MouseManager : MonoBehaviour
{
    private Vector3Int lastTileHovered = Vector3Int.zero;
    private bool wasLeftMouseDown = false;
    private bool wasRightMouseDown = false;
    private float MousePollingRate = 0.025f;

    public delegate void HoveredNewTile(Vector3Int tileCoords);
    public delegate void LeftMouseDown();
    public delegate void LeftMouseUp();
    public delegate void RightMouseDown();
    public delegate void RightMouseUp();

    public delegate void GameObjectClicked(GameObject gameObject);

    public static event HoveredNewTile OnHoveredNewTile;
    public static event LeftMouseDown OnLeftMouseDown;
    public static event LeftMouseUp OnLeftMouseUp;
    public static event RightMouseDown OnRightMouseDown;
    public static event RightMouseUp OnRightMouseUp;

    public static event GameObjectClicked OnGameObjectClicked;

    private void Start()
    {
        Debug.Log("starting mouse manager");
        StartCoroutine(MousePolling());
    }

    private void Update()
    {
        PollClickEvents();
    }

    /// <summary>
    /// Polls various mouse-related things
    /// and calls functions that fire events
    /// </summary>
    /// <returns></returns>
    private IEnumerator MousePolling()
    {
        while (true)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PollTileHoverEvents(pos);

            yield return new WaitForSecondsRealtime(MousePollingRate);
        }
    }

    /// <summary>
    /// Fires event when mouse goes down or up
    /// </summary>
    private void PollClickEvents()
    {
        //  if left mouse was not previously down and is now down
        if (wasLeftMouseDown == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                wasLeftMouseDown = true;
                if (OnLeftMouseDown != null)
                {
                    OnLeftMouseDown.Invoke();
                }
            }
        }
        //  Otherwise, check if left mouse was previously down and is now up
        else if (Input.GetMouseButtonUp(0))
        {
            wasLeftMouseDown = false;
            if (OnLeftMouseUp != null)
            {
                OnLeftMouseUp.Invoke();
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject objectAtPos = PerformGameObjectRaycast(pos);

            if (objectAtPos != null && OnGameObjectClicked != null) {
                OnGameObjectClicked.Invoke(objectAtPos);
            }
        }

        //  if right mouse was not previously down and is now down
        if (wasRightMouseDown == false) {
            if (Input.GetMouseButtonDown(1)) {
                wasRightMouseDown = true;
                if (OnRightMouseDown != null) {
                    OnRightMouseDown.Invoke();
                }
            }
        }
        //  Otherwise, check if right mouse was previously down and is now up
        else if (Input.GetMouseButtonUp(1)) {
            wasRightMouseDown = false;
            if (OnRightMouseUp != null) {
                OnRightMouseUp.Invoke();
            }
        }
    }

    /// <summary>
    /// Fires event if mouse is hovering over a new tile
    /// </summary>
    /// <param name="pos"></param>
    private void PollTileHoverEvents(Vector3 pos)
    {
        //  Check if mouse hovered over a new tile
        Vector3Int mousePos = Vector3Int.FloorToInt(pos);
        mousePos.z = 0;
        if (mousePos != lastTileHovered)
        {
            lastTileHovered = mousePos;
            if (OnHoveredNewTile != null)
            {
                OnHoveredNewTile.Invoke(mousePos);
            }
        }
    }

    private GameObject PerformGameObjectRaycast(Vector3 position) {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        // If it hits something...
        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }
}
