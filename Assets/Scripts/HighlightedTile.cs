using UnityEngine;

public class HighlightedTile
{
    public Vector3Int Position { get; private set; }
    public IMapManager.Layer Layer { get; private set; }
    public Color Color { get; private set; }
    public Color PreviousColor { get; private set; }

    /// <summary>
    /// Keeps track of tile that has been highlighted so it can later be reverted to the previous color or unhighlighted
    /// </summary>
    /// <param name="position">Position on TileMap</param>
    /// <param name="layer">Layer on TileMap</param>
    /// <param name="color">Color to highlight</param>
    /// <param name="previousColor">Previous color for reverting highlight (defaults to white)</param>
    public HighlightedTile(Vector3Int position, IMapManager.Layer layer, Color color, Color? previousColor = null)
    {
        Position = position;
        Layer = layer;
        Color = color;
        PreviousColor = previousColor.GetValueOrDefault(Color.white);
    }
}