using System.Collections.Generic;

public class InventorySingletone 
{

    private static InventorySingletone _instance;

    public static InventorySingletone Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InventorySingletone();
            }
            return _instance;
        }
    }

    int _maxSlotCount = 5;
    List<Slot> _slots = new List<Slot>();
    
    int _currentSlot = 0;

    public void Initialize()
    {
        for (int i = 0; i < _maxSlotCount; i++)
        {
            _slots.Add(new Slot());
        }
    }

    public string ShowInventory()
    {
        string a = " ";

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].item != null)
                a += $"Slot[{i + 1}], name {_slots[i].item.itemName}, count {_slots[i].itemCount}//";
            else
                a += $"Slot[{i + 1}], name --, count --//";
        }
        return a;
    }

    public List<Slot> InventorySync()
    {
        List<Slot> slot = _slots;

        return slot;
    }

    public void AddItem(Item item, int itemCount)
    {
        int existingSlotIndex = FindExistingItemSlotIndex(item);

        if (existingSlotIndex != -1)
        {
            _slots[existingSlotIndex].itemCount += itemCount;
        }
        else
        {
            int freeSlotIndex = FindFreeSlotIndex();

            if (freeSlotIndex != -1)
            {
                _currentSlot = freeSlotIndex;

                _slots[freeSlotIndex].item = item;
                _slots[freeSlotIndex].itemCount = itemCount;
            }
            else
            {
                SwitchItem(_currentSlot, item, itemCount);
            }
        }
    }

    private int FindExistingItemSlotIndex(Item item)
    {
        if (item.itemType == Item.ItemType.weapon)
            return -1; 

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
                return i;
            }
        }
        return -1;
    }

    private void SwitchItem(int targetSlotIndex, Item item, int itemCount)
    {
        if (_slots[_currentSlot].item != null)
        {
            RemoveItem(_slots[_currentSlot].itemCount, _currentSlot);

            _slots[targetSlotIndex].item = item;
            _slots[targetSlotIndex].itemCount = itemCount;
        }
    }

    public void RemoveAllItem(int slotIndex)
    {
        RemoveItem(_slots[slotIndex].itemCount, slotIndex);
    }

    public void RemoveItem(int itemCount, int slotIndex)
    {
        if (_slots[slotIndex].item != null)
        {
            _slots[slotIndex].itemCount -= itemCount;

            if (_slots[slotIndex].itemCount < 1)
            {
                _slots[slotIndex].item = null;
                _slots[slotIndex].itemCount = 0;
                return;
            }
        }
    }
}
