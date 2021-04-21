using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class HoverManager : MonoBehaviour
    {
        Vector3Int lastTileHovered = Vector3Int.zero;
        GameObject lastGameObjectHovered;

        public delegate void HoveredNewTile(Vector3Int tileCoords);
        public delegate void HoveredNewGameObject(GameObject gameObject);
        public delegate void UnhoveredGameObject();

        public static event HoveredNewTile OnHoveredNewTile;
        public static event HoveredNewGameObject OnHoveredNewGameObject;
        public static event UnhoveredGameObject OnUnhoveredGameObject;

        private void Start()
        {
            StartCoroutine(HoverPolling());
        }

        /// <summary>
        /// Polls mouse position at a reasonable frequency
        /// Raises event when the mouse hovers over a new tile
        /// Or when mouse hovers over a new game object
        /// or UI that implements IDisplayable
        /// </summary>
        /// <returns></returns>
        private IEnumerator HoverPolling()
        {
            //  Repeats forever every X seconds
            while (true)
            {
                bool raycastHit = false;

                //  Raycast on gameobjects
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity);
                if (hit.collider != null)
                {
                    raycastHit = true;
                    if ( hit.collider.gameObject != lastGameObjectHovered)
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
                    //  Raycast on UI
                    PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
                    pointerEvent.position = Input.mousePosition;

                    List<RaycastResult> result = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerEvent, result);

                    //  Check each GUI object hit by ray for an IDisplayable object
                    foreach (var item in result)
                    {
                        if (item.gameObject.GetComponent<IDisplayable>() != null)
                        {
                            raycastHit = true;
                            if (item.gameObject != lastGameObjectHovered)
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
                }

                if (raycastHit == false && lastGameObjectHovered != null)
                {
                    lastGameObjectHovered = null;

                    if (OnUnhoveredGameObject != null)
                    {
                        OnUnhoveredGameObject.Invoke();
                    }
                }

                //  Tile hover logic
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
                yield return new WaitForSeconds(0.025f);
            }
        }



    }
}
