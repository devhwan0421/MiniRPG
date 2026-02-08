using UnityEditor;
using UnityEngine;

public class ItemData //아이템 실제 정보
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public int MaxStack { get; set; }
    public int Grade { get; set; }
    public int HealAmount { get; set; }
}