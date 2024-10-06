using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public CatModel Model => _model;
    public float GetStrength => _model.Strength;

    [SerializeField]
    private CatVisualData _visualData;

    [SerializeField]
    private Transform _targetHand;

    [SerializeField]
    private Transform _targetCircle;

    [SerializeField]
    private SpriteRenderer _visual;
    
    [SerializeField]
    private float _speed = 350f;

    private CatModel _model;

    private Sequence _normalizeSeq;
    private Sequence _deadSeq;
    
    private Rigidbody _rb;
    public bool isSelected = false;

    private void OnDestroy()
    {
        _deadSeq?.Kill();
        _normalizeSeq?.Kill();
    }

    public void NormalizePos()
    {
        _normalizeSeq = DOTween.Sequence();
        _normalizeSeq.AppendInterval(0.3f);
        _normalizeSeq.Append(transform.DOMoveY(1f, 0.3f));
        _normalizeSeq.Append(transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f));
    }

    public void SetModel(CatModel model)
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }
        _model = model;
        transform.localScale = model.Scale;
        _visual.color = model.Color;
    }

    public void Select()
    {
        isSelected = true;
        _visual.sprite = _visualData.AwakeSprite;
    }

    public void Deselect()
    {
        isSelected = false;
        _visual.sprite = _visualData.SleepSprite;
    }

    public IEnumerator Death()
    {
        _deadSeq = DOTween.Sequence();
        _deadSeq.Append(transform.DOMoveY(2f, 0.3f));
        _deadSeq.Append(transform.DOMoveY(-2f, 0.1f));
        yield return _deadSeq.WaitForCompletion();
        Destroy(gameObject);
    }

    public MovableObject TryInteractWithMovableObject()
    {
        var movableObject = FindNearbyMovableObject();
        return movableObject;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (isSelected)
        {
            var inputX = Input.GetAxis("Horizontal");
            var inputY = Input.GetAxis("Vertical");
            var dir = new Vector3(inputX, 0, inputY);
            var modelSpeed = dir * (Time.deltaTime * _speed);
            _rb.velocity = new Vector3(modelSpeed.x, _rb.velocity.y, modelSpeed.z);
        }
    }

    public void UpdateSelectCircle(float rad)
    {
        _targetCircle.transform.localScale = new Vector3(rad, rad, 0.1f);
    }

    private MovableObject FindNearbyMovableObject()
    {
        foreach (var col in MovableObject.MovableObjects)
        {
            if (Vector3.Distance(col.transform.position, transform.position) > (1f + col.transform.localScale.x / 2))
                continue;

            if (col.TryGetComponent(out MovableObject movableObject))
            {
                return movableObject;
            }
        }

        return null;
    }

    public void Grab(MovableObject movableObject, Vector3 pos)
    {
        movableObject.Grab(_targetHand);
        var vector3 = new Vector3(pos.x, pos.y + 0.5f, pos.z);
        _targetHand.DOMove(vector3, 0.3f);
    }

    public void AddForce()
    {
        var modelStrength = Vector3.up * (500f / _model.Strength);
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(modelStrength);
    }
}