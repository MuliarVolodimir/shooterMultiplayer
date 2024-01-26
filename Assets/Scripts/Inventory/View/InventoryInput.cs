using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInput : MonoBehaviour
{
    [SerializeField] List<SlotView> _slots;
    [SerializeField] TextMeshProUGUI _currentItemInfo;

    [SerializeField] GameObject _armPosition;
    [SerializeField] GameObject _currentEquipPrefab;

    private int _currentSelectedSlot = 0;
    private List<Slot> _inventory;

    private void Start()
    {
        _inventory = InventorySingletone.Instance.InventorySync();
        SwitchSlot(_currentSelectedSlot);
        Debug.Log(InventorySingletone.Instance.GetInventoryInDebug());
    }

    private void Update()
    {
        SlotsNavigation();
        InventoryDebugCheck();
    }

    private void InventoryDebugCheck()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(InventorySingletone.Instance.GetInventoryInDebug());
        }
    }

    public void AddItem(Item item, int itemCount)
    {
        if (InventorySingletone.Instance.FindFreeSlotIndex() == -1)
        {
            RemoveAllItem();
        }

        InventorySingletone.Instance.AddItem(item, itemCount, _currentSelectedSlot);
        SwitchSlot(_currentSelectedSlot);
    }

    public void RemoveAllItem()
    {
        if (_currentEquipPrefab != null)
        {
            GameObject gameObject = Instantiate(_inventory[_currentSelectedSlot].Item.itemPrefab, transform.position, transform.rotation);
            gameObject.GetComponent<ItemObject>().Count = _inventory[_currentSelectedSlot].ItemCount;
        }

        InventorySingletone.Instance.RemoveAllItem(_currentSelectedSlot);
        SwitchSlot(_currentSelectedSlot);
    }

    public void RemoveItem()
    {
        if (_inventory[_currentSelectedSlot].Item != null)
        {
            Instantiate(_inventory[_currentSelectedSlot].Item.itemPrefab, transform.position, transform.rotation);
        }

        InventorySingletone.Instance.RemoveItem(1, _currentSelectedSlot);
        SwitchSlot(_currentSelectedSlot);
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

    private void SwitchSlot(int slotIndex)
    {
        _slots[_currentSelectedSlot].SwitchSelected(false);

        _currentSelectedSlot = slotIndex;
        _slots[_currentSelectedSlot].SwitchSelected(true);

        UpdateGraphic(_inventory[slotIndex].Item, _inventory[slotIndex].ItemCount, slotIndex);
    }

    private void UpdateGraphic(Item item, int itemCount, int slotIndex)
    {
        if (item != null)
        {
            if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);

            _currentEquipPrefab = Instantiate(item.itemPrefab, _armPosition.transform);

            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"{item.itemName}:{itemCount}";
        }
        else
        {
            if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);

            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"Slot {_currentSelectedSlot + 1} empty";    
        }  
    }
}
