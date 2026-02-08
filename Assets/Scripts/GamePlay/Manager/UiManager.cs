using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    private static UiManager _instance;

    public InventoryUi _inventoryUi;
    public DialogueUi _dialogueUi;
    public SystemUi _systemUi;

    public static UiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UiManager>();
            }
            return _instance;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventoryUi.isInventoryOpen = !_inventoryUi.isInventoryOpen;
            _inventoryUi._inventoryPanel.SetActive(_inventoryUi.isInventoryOpen);
        }
    }
}