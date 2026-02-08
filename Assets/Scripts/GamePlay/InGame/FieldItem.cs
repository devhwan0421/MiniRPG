using UnityEngine;

public class FieldItem : MonoBehaviour
{
    private int _inventoryId = -1;
    private int _itemId = -1;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _col;
    private ItemInteraction _child;

    private bool _isPickedUp = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
        _child = GetComponentInChildren<ItemInteraction>();
    }

    public void Init(int inventoryId, int itemId)
    {
        Debug.Log($"invenId: {inventoryId}");
        _inventoryId = inventoryId;
        _itemId = itemId;
        
        _spriteRenderer.sprite = ResourceManager.Instance.GetItemSprite(itemId);
        _spriteRenderer.sortingOrder = 10;

        _col.size = _spriteRenderer.sprite.bounds.size;

        _child.setColliderSize(_spriteRenderer.sprite.bounds.size);
    }

    public void PickUpItem()
    {
        if (_isPickedUp) return;

        Debug.Log($"서버에 아이템 줍기 요청 {_itemId}");
        _isPickedUp = true;

        //int mapId = 1;
        int mapId = Managers.Map.MapId;

        var pickUpPacket = PacketMaker.Instance.PickUpItem(mapId, _inventoryId);
        Managers.Network.SendPacket(pickUpPacket);

        Destroy(gameObject);
    }
}