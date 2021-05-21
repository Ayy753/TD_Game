using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

class MouseManager : MonoBehaviour
{
    private Vector3Int lastTileHovered = Vector3Int.zero;
    private bool wasMouseDown = false;
    private float MousePollingRate = 0.025f;

    public delegate void HoveredNewTile(Vector3Int tileCoords);
    public delegate void MouseDown();
    public delegate void MouseUp();

    public static event HoveredNewTile OnHoveredNewTile;
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
}
