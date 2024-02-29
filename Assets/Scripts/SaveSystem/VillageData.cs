using NPC.Friendly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.TownGeneration;

[System.Serializable]
public class VillageData : Location
{
    public string villageName;
    public VillageLayout layout;

    public FriendlyNpcData[] importantNpc;
    public FriendlyNpcData[] fillerNpc;

    public VillageData()
    {
    }
    
    public VillageData(string villageName, VillageLayout layout, Vector2Int mapCoordinates) : base(mapCoordinates)
    {
        this.villageName = villageName;
        this.layout = layout;
    }
}