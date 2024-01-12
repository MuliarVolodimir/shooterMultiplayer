using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int _maxSlotSize;
    [SerializeField] List<Item> _slots;

    public void AddItem(Item item, int slotIndex, int itemCount)
    {
        if (_slots.Count > slotIndex)
        {
            if (_slots[slotIndex] != null)
            {
                RemoveItem(slotIndex);
                _slots[slotIndex] = item;
                Debug.Log("Swich item " + item._name);
            }
            else
            {
                _slots[slotIndex] = item;
                Debug.Log($"Added item {item._name}");
            }
        }
        else
        {
            Debug.Log("Out Of Range Inventory");
        }
    }
    public void RemoveItem(int index)
    {
        if (_slots.Count > 0)
        {
            Debug.Log($"Removed {_slots[index]._name}");
            Instantiate(_slots[index]._itemPrefab, transform.position, transform.rotation);
            _slots[index] = null;
        }
        else
        {
            Debug.Log("Slot is CLEAR!!!");
        }
    }
    public void UseItem()
    {

    }

}
