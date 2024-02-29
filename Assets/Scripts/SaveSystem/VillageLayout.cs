using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using World.TownGeneration;

[System.Serializable]
public class VillageLayout
{
    // public HouseData[] houses;
    public Dictionary<int2, HouseData> houses;
    public int2[] roads;
    
    public int left;
    public int bottom;
    public int right;
    public int top;
}
