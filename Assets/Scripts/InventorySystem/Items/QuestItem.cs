using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(QuestItem), menuName = "InventorySystem/Items/" + nameof(QuestItem))]
public class QuestItem : StackableItem
{
   public QuestLocation[] location;
}
