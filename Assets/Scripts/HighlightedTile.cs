namespace DefaultNamespace.TilemapSystem {

    using UnityEngine;

    public class HighlightedTile {
        public Vector3Int Position { get; private set; }
        public MapLayer Layer { get; private set; }
        public Color Color { get; private set; }

        /// <summary>
        /// Keeps track of tile that has been highlighted 
        /// </summary>
        public HighlightedTile(Vector3Int position, MapLayer layer, Color color) {
            Position = position;
            Layer = layer;
            Color = color;
        }
    }
}
