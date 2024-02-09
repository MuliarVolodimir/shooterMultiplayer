using UnityEngine;

public class HealingItem : MonoBehaviour, IItem
{
    [SerializeField] int _healAmount;
    [SerializeField] float _useRate;
    [SerializeField] private Item _item;

    private float _nextUse;

    private void Start()
    {
        _item = GetComponent<ItemObject>().Item;
    }

    public void Action()
    {
        if (Time.time >= _nextUse)
        {
            _nextUse = Time.time + _useRate;
            InventorySingletone.Instance.RemoveItem(_item, 1, true);

            Debug.Log("Player healed by " + _healAmount + " point`s");
        }
    }
}
