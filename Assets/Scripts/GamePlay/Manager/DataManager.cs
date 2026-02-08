using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataManager
{
    public static DataManager Instance { get; } = new DataManager();

    public Dictionary<int, ItemData> ItemDataTable { get; private set; } = new Dictionary<int, ItemData>();

    private DataManager()
    {
        InitItemData();
    }

    //0: 힐템, 1:장비, 2, 기타
    private void InitItemData()
    {
        //일단은 클라이언트에서 처리
        ItemDataTable.Add(1, new ItemData { Id = 1, Name = "사과", Type = 0, MaxStack = 100, Grade = 0, HealAmount = 50 });
        ItemDataTable.Add(2, new ItemData { Id = 2, Name = "검", Type = 1, MaxStack = 1, Grade = 0 });
        ItemDataTable.Add(3, new ItemData { Id = 3, Name = "투구", Type = 1, MaxStack = 1, Grade = 0 });
        ItemDataTable.Add(4, new ItemData { Id = 4, Name = "나뭇잎", Type = 2, MaxStack = 100, Grade = 0 });
    }

    public ItemData GetItemData(int itemId)
    {
        if (ItemDataTable.TryGetValue(itemId, out var data))
            return data;
        return null;
    }
}