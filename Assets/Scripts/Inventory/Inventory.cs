using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int _maxSlotSize;
    [SerializeField] List<Slot> _slots = new List<Slot>();
    [SerializeField] TextMeshProUGUI _itemInfo;

    [SerializeField] GameObject _armPosition;
    [SerializeField] GameObject _currentEquipPrefab;

    private int _currentSelectedSlot = 0;

    private void Start()
    {
        SwitchSlot(0);
    }

    private void Update()
    {
        SlotsNavigation();
        UsingItem();
    }

    void UsingItem()
    {
        if (Input.GetKey(KeyCode.Mouse0) && _slots[_currentSelectedSlot].isSelected && _currentEquipPrefab != null)
        {
            Item currentItem = _slots[_currentSelectedSlot].item;

            if (currentItem != null && currentItem.itemPrefab.GetComponent<HealingItem>())
            {
                bool isUsed = currentItem.itemPrefab.GetComponent<HealingItem>().Action();

                if (isUsed) RemoveItem(1, true);
            }

            if (currentItem != null && currentItem.itemPrefab.GetComponent<Weapon>())
            {
                bool isUsed = currentItem.itemPrefab.GetComponent<Weapon>().Action();
                
            }
        }
    }

    public void AddItem(Item item, int itemCount)
    {
        int existingSlotIndex = FindExistingItemSlotIndex(item);

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].isSelected)
            {
                _currentSelectedSlot = i;
            }
        }

        if (existingSlotIndex != -1)
        {
            _slots[existingSlotIndex].itemCount += itemCount;
            _slots[existingSlotIndex].UpdateSlot(item, _slots[existingSlotIndex].itemCount);
            Debug.Log($"Added: {itemCount} {item.itemName}");
        }
        else
        {
            int freeSlotIndex = FindFreeSlotIndex();

            if (freeSlotIndex != -1)
            {
                _slots[freeSlotIndex].item = item;
                _slots[freeSlotIndex].itemCount = itemCount;
                _slots[freeSlotIndex].UpdateSlot(item, _slots[freeSlotIndex].itemCount);
                Debug.Log($"Added: new {item.itemName}");
            }
            else
            {
                SwitchItem(item, _currentSelectedSlot, itemCount);
                Debug.Log("slotSwithed");
            }
        }
        SwitchSlot(_currentSelectedSlot);
    }

    private int FindExistingItemSlotIndex(Item item)
    {
        if (item.itemType == Item.ItemType.weapon)
            return -1; // Не знаходимо місце для іншого типу предмету

        return _slots.FindIndex(slot =>
            slot.item != null && slot.item.itemType == item.itemType &&
            (item.itemType != Item.ItemType.ammo || (item.itemType == Item.ItemType.ammo 
            && slot.item.ammoType == item.ammoType)));
    }

    private int FindFreeSlotIndex()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].item == null)
            {
                Debug.Log($"Free slot found at index {i}");
                return i;
            }
        }
        Debug.Log("No free slots found");
        return -1;
    }

    private void SwitchItem(Item item, int targetSlotIndex, int itemCount)
    {
        if (_slots[_currentSelectedSlot].item != null)
        {
            RemoveItem(_slots[_currentSelectedSlot].itemCount, false);

            _slots[targetSlotIndex].UpdateSlot(item, itemCount);
            
            SwitchSlot(targetSlotIndex);
            Debug.Log($"Switched item");
        }
    }

    public void RemoveAllItem()
    {
        RemoveItem(_slots[_currentSelectedSlot].itemCount, false);
    }

    public void RemoveItem(int itemCount, bool isUsed)
    {
        if (_slots[_currentSelectedSlot].item != null)
        {
            _slots[_currentSelectedSlot].itemCount -= itemCount;

            if ((_slots[_currentSelectedSlot].itemCount) > 0)
            {
                _slots[_currentSelectedSlot].UpdateSlot(_slots[_currentSelectedSlot].item, _slots[_currentSelectedSlot].itemCount);

                if (isUsed)
                {
                    Debug.Log("Used " + _slots[_currentSelectedSlot].item.itemName);
                    return;
                }

                Instantiate(_currentEquipPrefab, transform.position, transform.rotation);
                Debug.Log("Droped " + itemCount + " " + _slots[_currentSelectedSlot].item.itemName);
                UpdateGraphic(_slots[_currentSelectedSlot].item, _slots[_currentSelectedSlot].itemCount);
                return;
            }

            if (!isUsed)
            {
                GameObject gm = Instantiate(_currentEquipPrefab, transform.position, transform.rotation);
                gm.GetComponent<ItemObject>().count = itemCount;
            }

            _slots[_currentSelectedSlot].UpdateSlot(null, 0);
            UpdateGraphic(_slots[_currentSelectedSlot].item, _slots[_currentSelectedSlot].itemCount);

            Debug.Log(_currentSelectedSlot + " - Slot is Clear");

            Destroy(_currentEquipPrefab);
        }
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
        _slots[_currentSelectedSlot].SetSelected(false);
        Destroy(_currentEquipPrefab);

        _currentSelectedSlot = slotIndex;

        _slots[_currentSelectedSlot].SetSelected(true);

        if (_slots[slotIndex].item != null)
        {
            _currentEquipPrefab = Instantiate(_slots[_currentSelectedSlot].item.itemPrefab, _armPosition.transform);
            UpdateGraphic(_slots[_currentSelectedSlot].item, _slots[_currentSelectedSlot].itemCount);
        }
        else UpdateGraphic(null, 0);
    }

    void UpdateGraphic(Item item, int itemCount)
    {
        if (item != null)
            _itemInfo.text = $"{item.itemName}:{itemCount}";
        else
            _itemInfo.text = "";
    }
}