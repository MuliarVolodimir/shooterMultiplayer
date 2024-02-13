using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Character : NetworkBehaviour, ICharacter
{
    [SerializeField] int _health;
    [SerializeField] GameObject _healthBar;
    [SerializeField] Image _healthImage;

    private int _maxHealth;

    [SerializeField] TextMeshProUGUI _healthTxt;

    private void Start()
    {
        _maxHealth = _health;
        _healthTxt = _healthBar.GetComponentInChildren<TextMeshProUGUI>();
        UpdateUI(_health);
    }

    private void Update()
    {
        if (transform.position.y <= -10)
        {
            DieServerRpc();
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            DieServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        //Not all functionality

        //Despawn function
        SpawnerSystem.Instance.Despawn(gameObject);
        _health = _maxHealth;
    }

    private void UpdateUI(float health)
    {
        if (_healthBar != null)
        {
            _healthImage.fillAmount = health / _maxHealth;
            _healthTxt.text = _health.ToString();
        }
    }
}
