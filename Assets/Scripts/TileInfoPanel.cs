namespace DefaultNamespace.GUI {

    using DefaultNamespace;
    using UnityEngine;
    using TMPro;
    using Zenject;
    using System;
    using DefaultNamespace.TilemapSystem;

    public class TileInfoPanel : IInitializable, IDisposable{
        readonly IMapManager mapManager;
        readonly TMP_Text txtName, txtDescription;

        public TileInfoPanel(IMapManager mapManager) {
            this.mapManager = mapManager;
            txtName = GameObject.Find("pnlTileInfo/txtName").GetComponent<TMP_Text>();
            txtDescription = GameObject.Find("pnlTileInfo/txtTileDescription").GetComponent<TMP_Text>();
        }

        public void Initialize() {
            MouseManager.OnHoveredNewTile += MouseManager_OnHoveredNewTile;
        }

        public void Dispose() {
            MouseManager.OnHoveredNewTile -= MouseManager_OnHoveredNewTile;
        }

        private void MouseManager_OnHoveredNewTile(Vector3Int tileCoords) {
            TileData tile = mapManager.GetTopLayerTileData(tileCoords, true);

            if (tile != null) {
                txtName.text = tile.Name;
                txtDescription.text = tile.Description;
            }
            else {
                txtName.text = "The void";
                txtDescription.text = "Devoid of matter";
            }
        }
    }
}