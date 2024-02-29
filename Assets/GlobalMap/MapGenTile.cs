using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = nameof(MapGenTile), menuName = "GlobalMap/" + nameof(MapGenTile))]
public class MapGenTile : ScriptableObject
{
    public GlobalMapTileModifier mapTileModifier;
    public int weight = 1;
    public float moveCost = 0;
    public TileBase tile;
    public MapGenTile[] topTiles;
    public MapGenTile[] bottomTiles;
    public MapGenTile[] leftTiles;
    public MapGenTile[] rightTiles;
}