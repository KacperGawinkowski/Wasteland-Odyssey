using InventorySystem.Items;


[System.Serializable]
public class EquipmentSaveData
{
    public ItemSaveData[] items;
    public ItemSaveData[] equippedItems = new ItemSaveData[5];
}

[System.Serializable]
public class ItemSaveData
{
    public string itemId;
    public int count;
    public UniqueItemInstance uniqueItemInstance;
}