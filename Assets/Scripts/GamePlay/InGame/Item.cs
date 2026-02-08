public class Item //인벤토리 내 아이템 정보
{
    public int InventoryId { get; set; }
    public int OwnerId { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
    public bool IsEquipped { get; set; }
    public int Enhancement { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public ItemData Data => DataManager.Instance.GetItemData(ItemId);

    public Item(ItemInfo itemInfo)
    {
        InventoryId = itemInfo.InventoryId;
        OwnerId = itemInfo.OwnerId;
        ItemId = itemInfo.ItemId;
        Count = itemInfo.Count;
        IsEquipped = itemInfo.IsEquipped;
        Enhancement = itemInfo.Enhancement;
        PosX = itemInfo.PosX;
        PosY = itemInfo.PosY;
        PosZ = itemInfo.PosZ;
    }
}