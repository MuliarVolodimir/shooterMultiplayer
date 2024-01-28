using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryInput : MonoBehaviour
{
    [SerializeField] List<SlotView> _slots;
    [SerializeField] TextMeshProUGUI _currentItemInfo;

    [SerializeField] GameObject _armPosition;
    [SerializeField] GameObject _currentEquipPrefab;

    private const int MAX_SLOTS_COUNT = 5;
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
        UsingItem();
        InventoryDebugCheck();
    }

    private void UsingItem()
    {
        if (Input.GetButton("Fire1"))
        {
            if (_currentEquipPrefab != null)
            {
                // for Weapon
                if (_currentEquipPrefab.GetComponent<Weapon>())
                {
                    var weapon = _currentEquipPrefab.GetComponent<Weapon>();
                    if (weapon.GetItemType() != null)
                    {
                        Item item = weapon.GetItemType();
                        if (InventorySingletone.Instance.FindExistingItemSlotIndex(item) != -1)
                        {
                            bool isAction = weapon.Action();
                            if (isAction)
                            {
                                InventorySingletone.Instance.RemoveItem(1, InventorySingletone.Instance.FindExistingItemSlotIndex(item));
                            }
                            UpdateGraphic(item, _inventory[InventorySingletone.Instance.FindExistingItemSlotIndex(item)].ItemCount, InventorySingletone.Instance.FindExistingItemSlotIndex(item), true);
                        }
                    }     
                }
                // for Healing Item
                if (_currentEquipPrefab.GetComponent<HealingItem>())
                {
                    var healingItem = _currentEquipPrefab.GetComponent<HealingItem>();
                    Item item = _currentEquipPrefab.GetComponent<ItemObject>().Item;

                    bool isAction = healingItem.Action();
                    if (isAction)
                    {
                        InventorySingletone.Instance.RemoveItem(1, InventorySingletone.Instance.FindExistingItemSlotIndex(item));
                    }
                    _slots[InventorySingletone.Instance.FindExistingItemSlotIndex(item)].UpdateSlotView(item, _inventory[InventorySingletone.Instance.FindExistingItemSlotIndex(item)].ItemCount);
                    SwitchSlot(_currentSelectedSlot);
                }
            }
        }
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
        for (int i = 0; i < MAX_SLOTS_COUNT; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
            {
                SwitchSlot(i);
                break;
            }
        }

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel < 0f) SwitchSlot((_currentSelectedSlot + 1) % MAX_SLOTS_COUNT);
        else if (scrollWheel > 0f) SwitchSlot((_currentSelectedSlot - 1 + MAX_SLOTS_COUNT) % MAX_SLOTS_COUNT);
    }

    private void SwitchSlot(int slotIndex)
    {
        _slots[_currentSelectedSlot].SwitchSelected(false);

        _currentSelectedSlot = slotIndex;
        _slots[_currentSelectedSlot].SwitchSelected(true);

        UpdateGraphic(_inventory[slotIndex].Item, _inventory[slotIndex].ItemCount, slotIndex, false);
    }

    private void UpdateGraphic(Item item, int itemCount, int slotIndex, bool isUsing)
    {
        if (item != null)
        {
            if (!isUsing)
            {
                if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);
                _currentEquipPrefab = Instantiate(item.itemPrefab, _armPosition.transform);
            }

            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"{item.itemName}:{itemCount}";
        }
        else
        {
            if (!isUsing)
                if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);

            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"Slot {_currentSelectedSlot + 1} empty";    
        }  
    }
}
