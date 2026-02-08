using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryUi _inventoryUi;

    private int _inventoryId = -1;
    private int _itemId = -1;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _countText;

    public void SetItem(int inventoryId, int itemId, int count)
    {
        _inventoryId = inventoryId;
        _itemId = itemId;

        Debug.Log($"아이템 슬롯 설정: InventoryId={inventoryId} ItemId={itemId}, Count={count}");
        //아이템 정보
        DataManager.Instance.ItemDataTable.TryGetValue(itemId, out var data);

        if (data != null)
        {
            Debug.Log($"아이템 데이터: 이름={data.Name}, 타입={data.Type}, 최대개수={data.MaxStack}, 등급={data.Grade}");
        }
        else
        {
            Debug.LogWarning($"아이템 데이터 없음: ItemId={itemId}");
        }
        //아이템 아이콘 설정
        //_iconImage.sprite = Managers.Resource.GetItemSprite(itemId);
        _iconImage.sprite = ResourceManager.Instance.GetItemSprite(itemId);
        if (_iconImage.sprite == null)
        {
            Debug.LogWarning($"아이템 아이콘 스프라이트 없음: ItemId={itemId}");
        }
        else
        {
            Debug.Log($"아이템 아이콘 설정됨: ItemId={itemId} Sprite={_iconImage.sprite.name}");
        }
        _iconImage.gameObject.SetActive(true);

        //아이템 수량 설정
        _countText.text = count > 1 ? count.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount == 1)
        {
            _inventoryUi.SetSelectedItem(_inventoryId, _itemId, _iconImage.sprite);
        }
        else if(eventData.clickCount == 2)
        {
            OnDoubleClickSlot();
        }
    }

    private void OnDoubleClickSlot()
    {
        _inventoryUi.UseItem(_inventoryId);
    }

    /*public void OnBeginDrag(PointerEventData eventData)
    {
        _iconImage.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _iconImage.transform.localPosition = Vector3.zero;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("아이템 버리기");
        }
    }*/
}