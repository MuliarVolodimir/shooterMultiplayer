using UnityEngine;

public class HealingItem : MonoBehaviour, IItem
{
    [SerializeField] int _healAmount;
    [SerializeField] float _useRate;
    private float _nextUse;

    public bool Action()
    {
        if (Time.time >= _nextUse)
        {
            _nextUse = Time.time + _useRate;
            Debug.Log("Player healed by " + _healAmount + " point`s");

            return true;
        }
        return false;
    }
    
}
