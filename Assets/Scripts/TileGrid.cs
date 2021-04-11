using UnityEngine;
using System.Collections.Generic;

public class TileGrid {
    public Dictionary<Vector3Int, GridObject> Tiles = new Dictionary<Vector3Int, GridObject>();

    public GridObject GetTile(Vector3Int position) {
        if (Tiles.ContainsKey(position)) {
            return Tiles[position];
        }
        return null;
    }

    public bool HasTile(Vector3Int position) => Tiles.ContainsKey(position);

    public void RemoveTile(Vector3Int position) {
        Tiles.Remove(position);
    }

    public void AddTile(Vector3Int position, GridObject tile) {
        if (!HasTile(position)) {
            Tiles.Add(position, tile);
        }
    }

    public void ReplaceTile(Vector3Int position, GridObject tile) {
        RemoveTile(position);
        AddTile(position, tile);
    }
}