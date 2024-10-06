using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public static List<MovableObject> MovableObjects = new();
    
    [SerializeField]
    private float _weight = 2f;

    [SerializeField]
    private TextMeshPro _weightText;

    [SerializeField]
    private Collider _col;
    
    private Transform _target;
    private bool _isGrab;

    private void Start()
    {
        MovableObjects.RemoveAll(m => m == null);
        MovableObjects.Add(this);
        _weightText.text = $"{_weight}";
    }

    private void LateUpdate()
    {
        if (_isGrab)
        {
            transform.position = _target.position;
        }
        _weightText.transform.position = transform.position + new Vector3(transform.localScale.x, transform.localScale.y + 1f, transform.localScale.z)/2;
        _weightText.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public void Grab(Transform hand)
    {
        _col.isTrigger = true;
        hand.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.x, transform.position.z);
        _isGrab = true;
        _target = hand;
        AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.Grab));
    }

    public void Release()
    {
        _col.isTrigger = false;
        _isGrab = false;
        _target = null;
        AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.Grab));
    }

    public bool CanMoveObject(float strength)
    {
        if (_weight > strength)
            return false;
        return true;
    }

    public void AddForce()
    {
        var modelStrength = Vector3.up * (500f / _weight);
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(modelStrength);
    }
}