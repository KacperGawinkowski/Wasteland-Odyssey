public enum InventoryEntryArrowDirection : byte
{
    Sell,
    Buy,
    None
}

public enum InventoryInterfaceType : byte
{
    OnlyPlayerInventory,
    Looting,
    Trading
}

public enum InventoryEntryLocation : byte
{
    PlayerInventory,
    PlayerNewItems,
    TradeInventory,
    TradeNewItems
}