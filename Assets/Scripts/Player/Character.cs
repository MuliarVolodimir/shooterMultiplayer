using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] int _health;
    [SerializeField] GameObject _healthBar;
    [SerializeField] Image _healthImage;

    private int _maxHealth;

    public TextMeshProUGUI _healthTxt;

    private void Start()
    {
        _maxHealth = _health;
        _healthTxt = _healthBar.GetComponentInChildren<TextMeshProUGUI>();

        UpdateUI(_health);
    }

    public void Takedamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
        UpdateUI(_health);
    }

    public void TakeHealth(int healAmount)
    {
        _health += healAmount;
        if (_health >= _maxHealth)
        {
            Debug.Log("Healing " + healAmount);
        }
        UpdateUI(_health);
    }

    private void Die()
    {
        //Not all functionality
        Destroy(gameObject);
    }

    private void UpdateUI(float health)
    {
        if (_healthBar != null)
        {
            _healthImage.fillAmount = health/_maxHealth;
            Debug.Log("bot " + health/_maxHealth);
            _healthTxt.text = _health.ToString();
        }
    }
}
