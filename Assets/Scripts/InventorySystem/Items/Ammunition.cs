using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = nameof(Ammunition), menuName = "InventorySystem/Items/" + nameof(Ammunition))]
    public class Ammunition : StackableItem
    {
        public AmmunitionType ammunitionType;
    }

    public enum AmmunitionType
    {
        PISTOL,
        RIFLE,
        SHOTGUN
    }
}
