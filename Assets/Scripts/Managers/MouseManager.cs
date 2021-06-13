using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

class MouseManager : MonoBehaviour {
    private Vector3Int lastTileHovered = Vector3Int.zero;
    private bool wasLeftMouseDown = false;
    private bool wasRightMouseDown = false;
    private const float MousePollingRate = 0.025f;
    private IDisplayable lastHoveredDisplayable;

    public delegate void HoveredNewTile(Vector3Int tileCoords);
    public delegate void LeftMouseDown();
    public delegate void LeftMouseUp();
    public delegate void RightMouseDown();
    public delegate void RightMouseUp();

    public delegate void GameObjectClicked(GameObject gameObject);
    public delegate void HoveredNewTooltipable(IDisplayable displayable);
    public delegate void UnHoveredTooltipable();

    public static event HoveredNewTile OnHoveredNewTile;
    public static event LeftMouseDown OnLeftMouseDown;
    public static event LeftMouseUp OnLeftMouseUp;
    public static event RightMouseDown OnRightMouseDown;
    public static event RightMouseUp OnRightMouseUp;

    public static event GameObjectClicked OnGameObjectClicked;
    public static event HoveredNewTooltipable OnHoveredNewTooltipable;
    public static event UnHoveredTooltipable OnUnhoveredTooltipable;

    private void Start() {
        Debug.Log("starting mouse manager");
        StartCoroutine(MousePolling());
    }

    private void Update() {
        PollClickEvents();
    }

    /// <summary>
    /// Polls various mouse-related things
    /// and calls functions that fire events
    /// </summary>
    /// <returns></returns>
    private IEnumerator MousePolling() {
        while (true) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PollTileHoverEvents(pos);

            PollToolTip(pos);

            yield return new WaitForSecondsRealtime(MousePollingRate);
        }
    }

    /// <summary>
    /// Fires event when mouse goes down or up
    /// </summary>
    private void PollClickEvents() {
        //  if left mouse was not previously down and is now down
        if (wasLeftMouseDown == false) {
            if (Input.GetMouseButtonDown(0)) {
                wasLeftMouseDown = true;
                if (OnLeftMouseDown != null) {
                    OnLeftMouseDown.Invoke();
                }
            }
        }
        //  Otherwise, check if left mouse was previously down and is now up
        else if (Input.GetMouseButtonUp(0)) {
            wasLeftMouseDown = false;
            if (OnLeftMouseUp != null) {
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
    private void PollTileHoverEvents(Vector3 pos) {
        //  Check if mouse hovered over a new tile
        Vector3Int mousePos = Vector3Int.FloorToInt(pos);
        mousePos.z = 0;
        if (mousePos != lastTileHovered) {
            lastTileHovered = mousePos;
            if (OnHoveredNewTile != null) {
                OnHoveredNewTile.Invoke(mousePos);
            }
        }
    }

    /// <summary>
    /// Performs physics raycast on gameobjects in scene at mouse pos
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private GameObject PerformGameObjectRaycast(Vector3 position) {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        // If it hits something...
        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Performs GUI raycast at cursor position
    /// </summary>
    /// <returns></returns>
    private List<RaycastResult> PerformUIRaycast() {
        //  Check if cursor is over a UI element that implements IDisplayable
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        pointerEvent.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, result);

        return result;
    }

    /// <summary>
    /// Performs GUI raycast at pos and displays tooltip if it hits idisplayable
    /// </summary>
    /// <param name="pos"></param>
    private void PollToolTip(Vector3 pos) {
        List<RaycastResult> hits = PerformUIRaycast();
        IDisplayable displayable = null;
        foreach (RaycastResult hit in hits) {
            displayable = hit.gameObject.GetComponent<IDisplayable>();
            if (displayable != null) {
                if (lastHoveredDisplayable != displayable) {
                    lastHoveredDisplayable = displayable;
                    if (OnHoveredNewTooltipable != null) {
                        OnHoveredNewTooltipable.Invoke(displayable);
                    }
                }
                return;
            }
        }

        //  TODO: implement tooltip listening for non-GUI gameobjects (using gameobject raycast)
        if (displayable == null && lastHoveredDisplayable != null) {
            lastHoveredDisplayable = null;
            if (OnUnhoveredTooltipable != null) {
                OnUnhoveredTooltipable.Invoke();
            }
        }
    }
}
