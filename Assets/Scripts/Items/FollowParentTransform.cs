using UnityEngine;

public class FollowParentTransform : MonoBehaviour
{
    [SerializeField] GameObject _target;

    void LateUpdate()
    {
        FollowParent();
    }

    public void SetFollowTarget(GameObject target)
    {
        _target = target;
    }

    private void FollowParent()
    {
        if (_target != null)
        {
            transform.position = _target.transform.position;
            transform.rotation = _target.transform.rotation;
        }
    }
}
