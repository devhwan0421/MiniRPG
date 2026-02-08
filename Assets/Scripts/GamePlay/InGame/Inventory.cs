using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class Inventory
{
    public int slotIndex { get; set; }
    //public List<Item> ItemList { get; set; }
    public Dictionary<int, Item> ItemList { get; set; } = new Dictionary<int, Item>();
    private readonly object _lock = new object();

    public event Action OnInventoryChanged;

    public void InitInventory(List<ItemInfo> items)
    {
        lock (_lock)
        {
            ItemList = items.Select(item => new Item(item)).ToDictionary(item => item.InventoryId);
            /*ItemList = items.Select(item => new Item
            {
                InventoryId = item.InventoryId,
                OwnerId = item.OwnerId,
                ItemId = item.ItemId,
                Count = item.Count,
                IsEquipped = item.IsEquipped,
                Enhancement = item.Enhancement
            }).ToDictionary(item => item.InventoryId);*/
        }
    }

    public Dictionary<int, Item> GetMyInventory()
    {
        lock (_lock)
        {
            return new Dictionary<int, Item>(ItemList);
        }
    }

    public void ShowInventory()
    {
        lock (_lock)
        {
            Console.WriteLine("Slot Index: " + slotIndex);
            Console.WriteLine("Items:");
            foreach (var item in ItemList)
            {
                var data = item.Value.Data;
                Console.WriteLine($"- ID: {item.Value.InventoryId} ItemId: {item.Value.ItemId}, Name: {data.Name}, Type: {data.Type}, Count: {item.Value.Count}");
            }
        }
    }

    public void AddItem(Item newItem)
    {
        lock (_lock)
        {
            ItemList.Add(newItem.InventoryId, newItem);
        }
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(int inventoryId)
    {
        lock (_lock)
        {
            ItemList.Remove(inventoryId);
        }
        OnInventoryChanged?.Invoke();
    }

    public void UpdateItem(Item updatedItem)
    {
        lock (_lock)
        {
            if (ItemList.TryGetValue(updatedItem.InventoryId, out var item))
            {
                item.Count = updatedItem.Count;
                item.IsEquipped = updatedItem.IsEquipped;
                item.Enhancement = updatedItem.Enhancement;
            }
        }
        OnInventoryChanged?.Invoke();
    }

    public void UseItem(int inventoryId)
    {
        //서버에 아이템 사용 요청(await 필요 없을 듯) 아닌가?
        //1. 요청
        //2. 서버에서 응답 오면 인벤토리에서 아이템 개수 차감
        //3. 중복요청시 어떻게?
        //4. 회복 아이템은 즉시 사용?
        //PacketSender.Instance.UseItemRequest(inventoryId);

        //Console.WriteLine("Item : " + inventoryId + " 사용 요청");

        var useItemRequestBuff = PacketMaker.Instance.MakeUseItemBuffer(inventoryId);
        NetworkManager.Instance.SendPacket(useItemRequestBuff);
    }

    public void OnUseItemResponse(UseItemResponse res)
    {
        Item item = null;

        if (!res.Success) return;
        lock (_lock)
        {
            if (ItemList.TryGetValue(res.InventoryId, out item))
            {
                item.Count -= 1;
                if (item.Count <= 0)
                {
                    ItemList.Remove(res.InventoryId);
                }
                else
                {
                    // 나중에
                }
            }
            else
            {
                return;
            }
        }

        if(res.Type == 0) //힐템일 경우
            //Managers.Object.MyPlayer.OnHpHeal(res.HealAmount);
            Managers.Object.MyPlayer.PlayerController.OnHpHeal(res.HealAmount);

        OnInventoryChanged?.Invoke();
    }

    public void DropItem(int inventoryId)
    {
        int mapId = Managers.Map.MapId;
        //int mapId = ObjectManager.Instance.Map.MapId;
        //int mapId = ObjectManager.Instance.MyPlayer.Map;
        //PacketSender.Instance.DropItemRequest(inventoryId, mapId);
        //Console.WriteLine("Item : " + inventoryId + " 버리기 요청");
        var dropItemRequestBuff = PacketMaker.Instance.MakeDropItemBuffer(inventoryId, mapId, Managers.Object.MyPlayer.PlayerInfo.PosX, Managers.Object.MyPlayer.PlayerInfo.PosY, Managers.Object.MyPlayer.PlayerInfo.PosZ);
        NetworkManager.Instance.SendPacket(dropItemRequestBuff);
    }

    public void OnDropItemResponse(DropItemResponse res)
    {
        if (!res.Success)
        {
            Console.WriteLine("아이템 버리기 실패");
            return;
        }

        int newInventoryId = res.Item.InventoryId; // newInventoryId
        int newItemId = res.Item.ItemId;

        lock (_lock)
        {
            if (ItemList.TryGetValue(res.OldInventoryId, out var item))
            {
                item.Count -= 1;
                if (item.Count <= 0)
                {
                    ItemList.Remove(res.OldInventoryId);
                    Console.WriteLine($"아이템 {item.InventoryId} 버리기 완료. 인벤토리에서 제거됨.");
                }
                else
                {
                    Console.WriteLine($"아이템 {item.InventoryId} 버리기 완료. 남은 개수: {item.Count}");
                }
                //맵 버려진 아이템 리스트에 추가
                //서버에서 브로드캐스트
                //inventoryId 행의 오너 컬럼을 -1로 변경하여 주인이 없는 상태로 만들고
                //맵에 일정 시간 동안 존재하고 아무도 줍지 않으면 서버에서 delete처리
                //이러면 서버는 월드 매니저로 맵 데이터를 관리해야할 것 같은데 흠

                //서버에서 맵 드랍 아이템 브로드캐스트할 것임. 그냥 인벤토리에서 드랍 아이템만 제거
                /*Item dropItem = new Item
                {
                    InventoryId = dropInventoryId,
                    OwnerId = -1,
                    ItemId = item.ItemId,
                    Count = 1,
                    IsEquipped = false,
                    Enhancement = item.Enhancement
                };
                ObjectManager.Instance.Map.AddDropItem(dropItem);*/

                //10개 묶음이었던 아이템은 서버에서 따로 행을 insert 후 inventoryId를 새로 발급해줘야 할 듯
                //드랍템은 db에서 행을 새로 생성하는게 맞을 듯, 그리고 서버에 월드 매니저가 있어야하는게 맞음(각 맵 상태 관리)
                //어느 맵에 버린 것인지도 서버로 전송해야 함(플레이어 현재 맵id)
                //지금은 1개씩 버리는 것으로 가정하고 진행
                //int dropinventoryid로 받지만 나중엔 객체로 버릴 아이템 정보를 서버로부터 받고 객체로 처리해야할 듯
            }
            else
            {
                Console.WriteLine("버그");
            }
        }
        OnInventoryChanged?.Invoke();
        //Managers.Object.DropItemField(itemId, res.PosX, res.PosY, res.PosZ);
        Managers.Map.DropItemField(newInventoryId, newItemId, res.PosX, res.PosY, res.PosZ);
    }

    public void OnPickUpItemResponse(PickUpItemResponse res)
    {
        Item item = new Item(res.Item);

        AddItem(item);
    }
}