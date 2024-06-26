using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InventoryInput : NetworkBehaviour
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
                gameObject.GetComponent<NetworkObject>().Spawn();
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
                var item = _currentEquipPrefab.GetComponent<IItem>();
                if (item != null)
                {
                    item.Action();
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
        if (_currentEquipPrefab != null)
        {
            _currentEquipPrefab.GetComponent<NetworkObject>().Despawn(true);
        }

        _currentSelectedSlot = slotIndex;
        _slots[_currentSelectedSlot].SwitchSelected(true);

        if (_inventory[slotIndex].Item != null)
        {
            _currentSelectedItem = _inventory[slotIndex].Item;
            CurrEquipPrefabChangeServerRpc();
        }

        UpdateSlotView(_inventory[slotIndex].Item, _inventory[slotIndex].ItemCount, slotIndex);
    }

    [ServerRpc]
    private void CurrEquipPrefabChangeServerRpc()
    {
        _currentEquipPrefab = Instantiate(_inventory[_currentSelectedSlot].Item.itemPrefab, _armPosition.transform);
        var netObject = _currentEquipPrefab.GetComponent<NetworkObject>();
        netObject.Spawn();

        var followTransform = _currentEquipPrefab.GetComponent<FollowParentTransform>();
        followTransform.SetFollowTarget(_armPosition);
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
