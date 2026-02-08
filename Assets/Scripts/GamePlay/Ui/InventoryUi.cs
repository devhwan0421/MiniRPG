using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    public GameObject _inventoryPanel;
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Image _followIcon;

    private List<ItemSlot> _slots = new List<ItemSlot>();

    public bool isInventoryOpen = false;

    private int _selectedInventoryId = -1;
    private int _selectedItemId = -1;

    /*private void Awake()
    {
        //플레이어 월드 입장시 등록으로 변경할 것
        Managers.Object.MyPlayer.Inventory.OnInventoryChanged += Refresh;
    }*/

    public void Init()
    {
        Debug.Log("InventoryUi Init()");
        Managers.Object.MyPlayer.Inventory.OnInventoryChanged += Refresh;
    }

    private void OnDestroy()
    {
        Managers.Object.MyPlayer.Inventory.OnInventoryChanged -= Refresh;
    }

    private void Start()
    {
        //Refresh();
        _inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            _inventoryPanel.SetActive(isInventoryOpen);
        }*/

        if(_selectedItemId != -1)
        {
            _followIcon.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    DropItem();
                }
                else
                {
                    CancelSelection();
                }
            }
        }
    }

    public void Refresh()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
        _slots.Clear();

        var inventory = Managers.Object.MyPlayer.Inventory.GetMyInventory();

        foreach (var itemInfo in inventory)
        {
            var slotObj = Instantiate(_slotPrefab, _content);
            var itemSlot = slotObj.GetComponent<ItemSlot>();
            itemSlot._inventoryUi = this;

            itemSlot.SetItem(itemInfo.Key, itemInfo.Value.ItemId, itemInfo.Value.Count);
            _slots.Add(itemSlot);
        }
    }

    public void SetSelectedItem(int inventoryId, int itemId, Sprite iconSprite)
    {
        _selectedInventoryId = inventoryId;
        _selectedItemId = itemId;
        _followIcon.sprite = iconSprite;
        _followIcon.gameObject.SetActive(true);
    }

    private void DropItem()
    {
        Managers.Object.MyPlayer.Inventory.DropItem(_selectedInventoryId);

        _selectedInventoryId = -1;
        _selectedItemId = -1;
        _followIcon.gameObject.SetActive(false);
    }

    private void CancelSelection()
    {
        _selectedInventoryId = -1;
        _selectedItemId = -1;
        _followIcon.gameObject.SetActive(false);
    }

    public void UseItem(int inventoryId)
    {
        Managers.Object.MyPlayer.Inventory.UseItem(inventoryId);
    }
}