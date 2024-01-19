using UnityEngine;

public class HealingItem : MonoBehaviour, IItem
{
    [SerializeField] int _healAmount;
    [SerializeField] float _useRate = 6f;
    private float _nextUse;

    public bool Action() //(Character character)
    {
        //character.Heal(_healAmount);
        if (Time.time >= _nextUse)
        {
            _nextUse = Time.time + _useRate;

            if (Input.GetButton("Fire1"))
            {
                Debug.Log("Player healed by " + _healAmount + " point`s");
                return true;
            }
        }
        return false;
    }
}
