using UnityEngine;

public class PickUpuble : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _checkRadius;

    [SerializeField] GameObject _object;

    void Start()
    {
        
    }
    void Update()
    {
        PickUpWeapon();
    }

    private void PickUpWeapon()
    {
        if (Physics.CheckSphere(transform.position, _checkRadius, _layerMask) && Input.GetKeyDown(KeyCode.F))
        {
            // Ваша логіка підбору
        }
    }
}
