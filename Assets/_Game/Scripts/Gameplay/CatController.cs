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
    
    private MovableObject _movableObject;
    private CatModel _model;

    private bool _isGrab;
    
    private Sequence _normalizeSeq;
    private Sequence _deadSeq;
    
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
        _model = model;
        transform.localScale = model.Scale;
        _visual.color = model.Color;
    }

    public void Select()
    {
        _visual.sprite = _visualData.AwakeSprite;
    }
    
    public void Deselect()
    {
        _visual.sprite = _visualData.SleepSprite;
    }
    
    public IEnumerator Death()
    {
        _deadSeq = DOTween.Sequence();
        _deadSeq.AppendInterval(1f);
        _deadSeq.Append(transform.DOMoveY(2f, 0.3f));
        _deadSeq.Append(transform.DOMoveY(-2f, 0.1f));
        yield return _deadSeq.WaitForCompletion();
        Destroy(gameObject);
    }

    public InteractType TryInteractWithMovableObject()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!_isGrab)
            {
                TryGrab();
                return InteractType.Grab;
            }
            
            if(_movableObject != null)
            {
                _isGrab = false;
                _movableObject.Release();
                _movableObject = null;
                return InteractType.Release;
            }
        }
        
        return InteractType.None;
    }
    
    public void Move()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");
        var dir = new Vector3(inputX, 0, inputY);
        transform.position += dir * (_model.Speed * Time.deltaTime);
    }

    public void UpdateSelectCircle(float rad)
    {
        _targetCircle.transform.localScale = new Vector3(rad, rad, 0.1f);
    }

    private void TryGrab()
    {
        var colliders = Physics.OverlapSphere(transform.position, 0.6f);
        foreach (var col in colliders)
        {
            if(col.TryGetComponent(out MovableObject movableObject))
            {
                _movableObject = movableObject;
                _isGrab = true;
                if (_movableObject.CanMoveObject(Model.Strength))
                {
                    _movableObject.Grab(_targetHand);
                }
            }
        }
    }
}

public enum InteractType
{
    None,
    Grab,
    Release
}