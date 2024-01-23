using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryInput : MonoBehaviour
{
    [SerializeField] List<Slot> _inventory;
    [SerializeField] TextMeshProUGUI _itemInfo;

    [SerializeField] GameObject _armPosition;
    [SerializeField] GameObject _currentEquipPrefab;

    private int _currentSelectedSlot = 0;


    private void Start()
    {
        InventorySingletone.Instance.Initialize();

        //_inventory = InventorySingletone.Instance.InventorySync();
        Debug.Log(InventorySingletone.Instance.ShowInventory());
    }

    private void Update()
    {
        SlotsNavigation();
        InventoryCheck();
    }

    void InventoryCheck()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory = InventorySingletone.Instance.InventorySync();
            Debug.Log(InventorySingletone.Instance.ShowInventory());
        }
    }

    public void AddItem(Item item, int itemCount)
    {
        InventorySingletone.Instance.AddItem(item, itemCount);
        _inventory = InventorySingletone.Instance.InventorySync();
        SwitchSlot(_currentSelectedSlot);
    }

    public void RemoveAllItem()
    {
        InventorySingletone.Instance.RemoveAllItem(_currentSelectedSlot);
        _inventory = InventorySingletone.Instance.InventorySync();
    }

    public void RemoveItem()
    {
        InventorySingletone.Instance.RemoveItem(1, _currentSelectedSlot);
        _inventory = InventorySingletone.Instance.InventorySync();
    }

    private void SlotsNavigation()
    {
        for (int i = 0; i < 5; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
            {
                SwitchSlot(i);
                break;
            }
        }

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel < 0f) SwitchSlot((_currentSelectedSlot + 1) % 5);
        else if (scrollWheel > 0f) SwitchSlot((_currentSelectedSlot - 1 + 5) % 5);
    }

    void SwitchSlot(int slotIndex)
    {
        _currentSelectedSlot = slotIndex;

        if (_inventory[slotIndex].item != null)
            UpdateGraphic(_inventory[slotIndex].item, _inventory[slotIndex].itemCount);
        else
            UpdateGraphic(null, 0);
    }

    void UpdateGraphic(Item item, int itemCount)
    {
        if (item != null)
            _itemInfo.text = $"{item.itemName}:{itemCount}";
        else
            _itemInfo.text = $"Slot {_currentSelectedSlot + 1} empty" ;
    }
}
