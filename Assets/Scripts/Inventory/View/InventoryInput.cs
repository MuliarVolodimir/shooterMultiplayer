using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

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
                //for Melee Weapon / Зброя ближнього бою
                if (_currentEquipPrefab.GetComponent<MeleeWeapon>())
                {
                    MeleeWeaponAction();
                    return;
                }
                // for FireArm Weapon / Вогнепальна зброя
                if (_currentEquipPrefab.GetComponent<FireArmWeapon>())
                {
                    FireArmWeaponAction();
                    return;
                }
                // for Healing Item / Лікувальні предмети
                if (_currentEquipPrefab.GetComponent<HealingItem>())
                {
                    HealingItemAction();
                    return;
                }
            }
        }
    }

    private void MeleeWeaponAction()
    {
        var meleeWeapon = _currentEquipPrefab.GetComponent<MeleeWeapon>();
        meleeWeapon.Action();
        return;
    }

    private void HealingItemAction()
    {
        var healingItem = _currentEquipPrefab.GetComponent<HealingItem>();
        bool isAction = healingItem.Action();

        if (isAction)
        {
            InventorySingletone.Instance.RemoveItem(1, _currentSelectedSlot);
            if (_inventory[_currentSelectedSlot].Item == null)
            {
                SwitchSlot(_currentSelectedSlot);
                return;
            }
            UpdateSlotView(_inventory[_currentSelectedSlot].Item, _inventory[_currentSelectedSlot].ItemCount, _currentSelectedSlot);
        }
        return;
    }

    private void FireArmWeaponAction()
    {
        var weapon = _currentEquipPrefab.GetComponent<FireArmWeapon>();
        Item bullet = weapon.GetBulletType();
        int existingItemIndex = InventorySingletone.Instance.FindExistingItemSlotIndex(bullet);

        if (existingItemIndex != -1)
        {
            bool isAction = weapon.Action();
            if (isAction)
            {
                InventorySingletone.Instance.RemoveItem(1, existingItemIndex);
            }

            UpdateSlotView(InventorySingletone.Instance.FindExistingItemSlotIndex(bullet) != -1 ? bullet : null
                , _inventory[existingItemIndex].ItemCount
                , existingItemIndex);
        }
        return;
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
        int freeSlotIndex = InventorySingletone.Instance.FindFreeSlotIndex();

        if (freeSlotIndex == -1)
        {
            RemoveAllItem();
        }

        InventorySingletone.Instance.AddItem(item, itemCount, _currentSelectedSlot);
        UpdateSlotView(_inventory[freeSlotIndex].Item, _inventory[freeSlotIndex].ItemCount, freeSlotIndex);
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
        if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);

        _currentSelectedSlot = slotIndex;
        _slots[_currentSelectedSlot].SwitchSelected(true);

        if (_inventory[slotIndex].Item != null)
        {
            if (_currentEquipPrefab != null) Destroy(_currentEquipPrefab);
            _currentEquipPrefab = Instantiate(_inventory[_currentSelectedSlot].Item.itemPrefab, _armPosition.transform);
        }

        UpdateSlotView(_inventory[slotIndex].Item, _inventory[slotIndex].ItemCount, slotIndex);
    }

    private void UpdateSlotView(Item item, int itemCount, int slotIndex)
    {
        if (item != null)
        {
            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"{item.itemName}:{itemCount}";
        }
        else
        {
            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"Slot {_currentSelectedSlot + 1} empty";
        }
    }
}
