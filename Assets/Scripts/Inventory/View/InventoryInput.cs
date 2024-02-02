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
    private Item _currentSelectedItem;

    private void Awake()
    {
        InventorySingletone.Instance.OnItemRemoved += OnitemRemoved;
        InventorySingletone.Instance.OnItemAdded += OnItemAdded;
        InventorySingletone.Instance.OnItemSwitched += OnItemSwitched;
    }

    private void OnItemAdded(Item item, int itemCount, int slotIndex)
    {
        UpdateSlotView(item, itemCount, slotIndex);

        Debug.Log($"+ {itemCount} {item.itemName}");
    }

    private void OnitemRemoved(Item item, int currentItemCount ,int removedItemCount, int slotIndex, bool isUsed)
    {
        UpdateSlotView(item, currentItemCount, slotIndex);

        if (item != null)
        {
            if (!isUsed)
            {
                GameObject gameObject = Instantiate(item.itemPrefab, transform.position, transform.rotation);
                gameObject.GetComponent<ItemObject>().Count = removedItemCount;
            }

            Debug.Log($"- {removedItemCount} {item.itemName} {item}");
        }
    }

    private void OnItemSwitched(Item oldItem, Item newItem, int oldItemCount, int newItemCount, int slotIndex)
    {
        UpdateSlotView(newItem, newItemCount, slotIndex);
        Debug.Log($"{oldItemCount} {oldItem} ~ {newItemCount} {newItem}");
    }

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
    }

    private void HealingItemAction()
    {
        var healingItem = _currentEquipPrefab.GetComponent<HealingItem>();
        healingItem.Action();
    }

    private void FireArmWeaponAction()
    {
        var weapon = _currentEquipPrefab.GetComponent<FireArmWeapon>();
        weapon.Action();
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
        InventorySingletone.Instance.AddItem(item, itemCount, _currentSelectedSlot);

        SwitchSlot(_currentSelectedSlot);
    }

    public void RemoveAllItem()
    {
        InventorySingletone.Instance.RemoveAllItem(_currentSelectedSlot);
        SwitchSlot(_currentSelectedSlot);
    }

    public void RemoveItem()
    {
        InventorySingletone.Instance.RemoveItem(_currentSelectedItem, 1, false);

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
            _currentSelectedItem = _inventory[slotIndex].Item;

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
            _currentItemInfo.text = $"{itemCount}";
        }
        else
        {
            _slots[slotIndex].UpdateSlotView(item, itemCount);
            _currentItemInfo.text = $"--";
        }
    }
}
