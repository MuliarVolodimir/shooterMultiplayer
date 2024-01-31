using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] int _health;
    private int _maxHealth;

    private void Start()
    {
        _maxHealth = _health;
    }

    public void Takedamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    public void TakeHealth(int healAmount)
    {
        _health += healAmount;
        if (_health >= _maxHealth)
        {
            Debug.Log("Healing " + healAmount);
        }
    }

    private void Die()
    {
        //Not all functionality
        Destroy(gameObject);
    }
}
