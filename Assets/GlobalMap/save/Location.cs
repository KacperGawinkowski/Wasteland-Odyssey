using System;
using UnityEngine;

[Serializable]
public abstract class Location
{
    // to chyba w ogóle nie jest potrzebne nawet sus
    public int positionX;
    public int positionY;

    protected Location()
    {
    }

    protected Location(Vector2Int mapCoordinates)
    {
        positionX = mapCoordinates.x;
        positionY = mapCoordinates.y;
    }
}