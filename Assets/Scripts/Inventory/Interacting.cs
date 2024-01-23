using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

public class Interacting : MonoBehaviour 
{
    [SerializeField] InventoryInput _inventoryInput;
    [SerializeField] Camera _camera;
    [SerializeField] float _interactDistance;
    [SerializeField] LayerMask _interactLayer;

    private Outline _lastInteractObj;
    void Start()
    {
        _inventoryInput = GetComponent<InventoryInput>();
    }

    void Update()
    {
        PickUpingAndDroping();

    }

    private void PickUpingAndDroping()
    {
        Vector3 rayStart = _camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        ItemInfo(rayStart, out hit);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(rayStart, _camera.transform.forward, out hit, _interactDistance, _interactLayer))
            {
                var hitObj = hit.transform.gameObject.GetComponent<ItemObject>();
                if (hitObj)
                {
                    _inventoryInput.AddItem(hitObj.item, hitObj.count);

                    Destroy(hit.transform.gameObject, 0.1f);
                }
            }
            Debug.DrawRay(rayStart, _camera.transform.forward, Color.yellow);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                _inventoryInput.RemoveAllItem();

                return;
            }
            _inventoryInput.RemoveItem();
        }
    }

    private void ItemInfo(Vector3 rayStart, out RaycastHit hit)
    {
        if (Physics.Raycast(rayStart, _camera.transform.forward, out hit, _interactDistance, _interactLayer))
        {
            if (_lastInteractObj != null) _lastInteractObj.enabled = false;

            _lastInteractObj = hit.transform.gameObject.GetComponent<Outline>();
            _lastInteractObj.enabled = true;

        }
        else if (_lastInteractObj != null)
        {
            _lastInteractObj.enabled = false;
            _lastInteractObj = null;
        }
    }
}
