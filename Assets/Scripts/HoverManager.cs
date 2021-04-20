using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEditor;
using UnityEngine;
namespace Assets.Scripts
{
    class HoverManager :  MonoBehaviour
    {
        GameManager gameManager;
        MapManager mapManager;
        GUIController guiController;
        Vector3Int lastTileHovered = Vector3Int.zero;

        public delegate void HoveredNewTile(Vector3Int tileCoords);
        public static event HoveredNewTile OnHoveredNewTile;

        private void Start()
        {
            gameManager = GameManager.Instance;
            guiController = gameManager.GUIController;
            mapManager = gameManager.MapManager;

            StartCoroutine(HoverPolling());
        }

        /// <summary>
        /// Polls mouse position at a reasonable frequency
        /// Raises event when the mouse hovers over a new tile
        /// </summary>
        /// <returns></returns>
        private IEnumerator HoverPolling()
        {
            while (true)
            {
                Vector3Int mousePos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = 0; 

                if (mousePos != lastTileHovered)
                {
                    lastTileHovered = mousePos;
                    OnHoveredNewTile.Invoke(mousePos);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
