using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalMap
{
    public Dictionary<Vector2Int, GlobalMapTileData> tiles = new();
    public Dictionary<Vector2Int, Location> locations = new();

    public Vector2 perlin;

    #region locations

    public void AddLocation(Vector2Int position, Location location)
    {
        locations.Add(position, location);
    }

    public void AddLocation(int x, int y, Location location)
    {
        AddLocation(new Vector2Int(x, y), location);
    }

    public void RemoveLocation(Vector2Int position)
    {
        locations.Remove(position);
    }

    public void RemoveLocation(int x, int y)
    {
        RemoveLocation(new Vector2Int(x, y));
    }

    public Location GetLocation(Vector2Int position)
    {
        return locations.TryGetValue(position, out var location) ? location : default;
    }
    
    public Location GetLocation(int x, int y)
    {
        return GetLocation(new Vector2Int(x, y));
    }

    #endregion

    public void AddTileData(Vector2Int position, GlobalMapTileData data)
    {
        tiles.Add(position, data);
    }

    public void AddTileData(int x, int y, GlobalMapTileData data)
    {
        if (data != default)
        {
            AddTileData(new Vector2Int(x, y), data);
        }
    }

    public GlobalMapTileData GetTileData(Vector2Int position)
    {
        return tiles.TryGetValue(position, out GlobalMapTileData data) ? data : default;
    }

    public GlobalMapTileData GetTileData(int x, int y)
    {
        return GetTileData(new Vector2Int(x, y));
    }
}