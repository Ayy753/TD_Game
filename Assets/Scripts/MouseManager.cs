using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

class MouseManager : MonoBehaviour
{
    private Vector3Int lastTileHovered = Vector3Int.zero;
    private GameObject lastGameObjectHovered;
    private bool wasMouseDown = false;
    private float MousePollingRate = 0.025f;

    public delegate void HoveredNewTile(Vector3Int tileCoords);
    public delegate void HoveredNewGameObject(GameObject gameObject);
    public delegate void UnhoveredGameObject();
    public delegate void MouseDown();
    public delegate void MouseUp();

    public static event HoveredNewTile OnHoveredNewTile;
    public static event HoveredNewGameObject OnHoveredNewGameObject;
    public static event UnhoveredGameObject OnUnhoveredGameObject;
    public static event MouseDown OnMouseDown;
    public static event MouseUp OnMouseUp;

    private void Start()
    {
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
        //  Repeats forever every X seconds
        while (true)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PollDisplayableHover(pos);
            PollTileHoverEvents(pos);

            yield return new WaitForSeconds(MousePollingRate);
        }
    }

    /// <summary>
    /// Fires event when mouse goes down or up
    /// </summary>
    private void PollClickEvents()
    {
        //  if was not previously down and is now down
        if (wasMouseDown == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                wasMouseDown = true;
                if (OnMouseDown != null)
                {
                    OnMouseDown.Invoke();
                }
            }
        }
        //  Otherwise, check if mouse was previously down and is now up
        else if (Input.GetMouseButtonUp(0))
        {
            wasMouseDown = false;
            if (OnMouseUp != null)
            {
                OnMouseUp.Invoke();
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

    /// <summary>
    /// Checks if the mouse is hovering over either a game 
    /// object or UI that implements IDisplayable
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private void PollDisplayableHover(Vector3 pos)
    {
        IDisplayable displayable = null;

        //  Check if cursor is over a game object using raycasting
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity);
        if (hit.collider != null)
        {
            displayable = hit.collider.gameObject.GetComponent<IDisplayable>();
            if (displayable != null && hit.collider.gameObject != lastGameObjectHovered)
            {
                lastGameObjectHovered = hit.collider.gameObject;

                if (OnHoveredNewGameObject != null)
                {
                    OnHoveredNewGameObject.Invoke(hit.collider.gameObject);
                }
            }
        }
        else
        {
            //  Check if cursor is over a UI element that implements IDisplayable
            PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
            pointerEvent.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEvent, result);

            //  Check each GUI object hit by ray for an IDisplayable object
            foreach (var item in result)
            {
                displayable = item.gameObject.GetComponent<IDisplayable>();
                if (displayable != null && item.gameObject != lastGameObjectHovered)
                {
                    lastGameObjectHovered = item.gameObject;

                    if (OnHoveredNewGameObject != null)
                    {
                        OnHoveredNewGameObject.Invoke(item.gameObject);
                    }
                }
                break;
            }
        }

        //  If a game object was previously hovered over
        //  but nothing is under cursor now
        if (displayable == null && lastGameObjectHovered != null)
        {
            lastGameObjectHovered = null;

            //  Fire event notifying anything interested that previous 
            //  hovered gameobject is no longer being hovered over
            if (OnUnhoveredGameObject != null)
            {
                OnUnhoveredGameObject.Invoke();
            }
        }
    }

}
