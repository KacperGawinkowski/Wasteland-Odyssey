using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace World.TownGeneration
{
    [Serializable]
    public class HouseData
    {
        // public int2 position;
        public int rotation;
        [FormerlySerializedAs("houseConfigIndex")] public int houseId;
        public int houseVariant;
        public int interiorVariant;

        public HouseData()
        {
        }
        
        

        // public HouseData(int2 position, int rotation)
        // {
        //     this.position = position;
        //     this.rotation = rotation;
        // }

        // public override bool Equals(object obj)
        // {
        //     return obj is HouseData house &&
        //            position.Equals(house.position);
        // }
        //
        // public override int GetHashCode()
        // {
        //     return HashCode.Combine(position);
        // }
    }
}