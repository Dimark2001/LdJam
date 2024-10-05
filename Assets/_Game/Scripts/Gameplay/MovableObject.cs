using TMPro;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [SerializeField]
    private float _weight = 2f;

    [SerializeField]
    private TextMeshPro _weightText;

    private Transform _target;
    private Collider _col;
    private bool _isGrab;

    private void Start()
    {
        _col = GetComponent<Collider>();
        _weightText.text = $"{_weight}";
    }

    private void LateUpdate()
    {
        if (_isGrab)
        {
            transform.position = _target.position;
        }
    }

    public void Grab(Transform target)
    {
        _col.isTrigger = true;
        target.position = transform.position;
        _isGrab = true;
        _target = target;
    }

    public void Release()
    {
        _col.isTrigger = false;
        _isGrab = false;
        _target = null;
    }

    public bool CanMoveObject(float strength)
    {
        if (_weight > strength)
            return false;
        return true;
    }
}