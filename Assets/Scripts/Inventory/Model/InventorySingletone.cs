using System;
using System.Collections.Generic;

public class InventorySingletone 
{
    private static InventorySingletone _instance;

    private const int MAX_SLOTS_COUNT = 5;
    private List<Slot> _slots = new List<Slot>();
    private int _currentSlot = 0;
    //private int _currentSelectedSlot = 0;

    //Actions :)
    public event Action<Item, int, int, int, bool> OnItemRemoved; // Remove // item, currentItemCount, removedItemCount, slotIndex
    public event Action<Item, int, int> OnItemAdded; // Add // item, itemCount, slotIndex
    public event Action<Item, Item, int, int, int> OnItemSwitched; // Switch // itemOld, itemNew, oldItemCount, newItemCount, slotIndex

    InventorySingletone()
    {
        for (int i = 0; i < MAX_SLOTS_COUNT; i++)
        {
            _slots.Add(new Slot());
        }
    } 

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

    public string GetInventoryInDebug()
    {
        string a = " ";

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item != null)
                a += $"Slot[{i + 1}], name {_slots[i].Item.itemName}, count {_slots[i].ItemCount}//";
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

    public void AddItem(Item item, int itemCount, int slotIndex)
    {
        _currentSlot = slotIndex;

        int existingSlotIndex = FindExistingItemSlotIndex(item);

        if (existingSlotIndex != -1)
        {
            _currentSlot = existingSlotIndex;
            _slots[existingSlotIndex].ItemCount += itemCount;
        }
        else
        {
            int freeSlotIndex = FindFreeSlotIndex();

            if (freeSlotIndex != -1)
            {
                _currentSlot = freeSlotIndex;

                _slots[freeSlotIndex].Item = item;
                _slots[freeSlotIndex].ItemCount = itemCount;
            }
            else
            {
                SwitchItem(_currentSlot, item, itemCount);
            }
        }
        OnItemAdded?.Invoke(_slots[_currentSlot].Item, _slots[_currentSlot].ItemCount, _currentSlot);

    }

    public int FindExistingItemSlotIndex(Item item)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item != null 
                && _slots[i].Item.ItemIndex == item.ItemIndex 
                && item.isStackable)
            {
                return i;
            }
        }

        return -1;
    }

    public int FindFreeSlotIndex()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item == null)
            {
                return i;
            }
        }
        return -1;
    }

    private void SwitchItem(int slotIndex, Item item, int itemCount)
    {
        if (_slots[_currentSlot].Item != null)
        {
            OnItemSwitched?.Invoke(_slots[slotIndex].Item, item, _slots[slotIndex].ItemCount, itemCount, slotIndex);

            RemoveAllItem(_currentSlot);
            _slots[slotIndex].Item = item;
            _slots[slotIndex].ItemCount = itemCount;
        }
    }

    public void RemoveAllItem(int slotIndex)
    {
        RemoveItem(_slots[slotIndex].Item, _slots[slotIndex].ItemCount, false);
    }

    public void RemoveItem(Item item, int removedItemCount, bool isUsed)
    {
        int slotIndex = FindItemToRemove(item);

        if (slotIndex != -1)
        {
            _slots[slotIndex].ItemCount -= removedItemCount;
            OnItemRemoved?.Invoke(_slots[slotIndex].Item, _slots[slotIndex].ItemCount, removedItemCount, slotIndex, isUsed);

            if (_slots[slotIndex].ItemCount < 1)
            {
                _slots[slotIndex].Item = null;
                _slots[slotIndex].ItemCount = 0;
            }
        }
    }

    private int FindItemToRemove(Item item)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item != null
                && _slots[i].Item.ItemIndex == item.ItemIndex)
            {
                return i;
            }
        }

        return -1;
    }
}
