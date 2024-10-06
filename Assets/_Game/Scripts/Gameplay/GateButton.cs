using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GateButton : MonoBehaviour
{
    [SerializeField]
    private float _pressDifficult = 2f;
    [SerializeField]
    private TextMeshPro _difficultText;
    
    [SerializeField]
    private Transform _pressTransform;
    
    [SerializeField]
    private Transform _gateTransform;
    
    [SerializeField]
    private Transform _gateTarget;
    
    private Vector3 _startPos;
    private Vector3 _startGatePos;
    private Sequence _pressSeq;
    private Sequence _gateSeq;
    
    private void Start()
    {
        _startPos = _pressTransform.position;
        _startGatePos = _gateTransform.position;
        _difficultText.text = $"{_pressDifficult}";
    }

    private void OnDestroy()
    {
        _pressSeq?.Kill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CatController catController))
        {
            if (catController.Model.Strength >= _pressDifficult)
            {
                PressSeq(_startPos - new Vector3(0f, 0.5f, 0f), () =>
                {
                    OpenGate();
                });
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out CatController catController))
        {
            if (catController.Model.Strength < _pressDifficult)
            {
                PressSeq(_startPos);
                CloseGate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CatController catController))
        {
            if (catController.Model.Strength >= _pressDifficult)
            {
                PressSeq(_startPos);
                CloseGate();
            }
        }
    }

    private void PressSeq(Vector3 pos, Action action = null)
    {
        _pressSeq?.Kill();
        _pressSeq = DOTween.Sequence();
        _pressSeq.Append(_pressTransform.DOMove(pos, 0.3f));
        _pressSeq.AppendCallback(() =>
        {
            action?.Invoke();
        });
    }

    private void OpenGate()
    {
        _gateSeq = DOTween.Sequence();
        _gateSeq.Append(_gateTransform.DOMove(_gateTarget.position, 2f));
        _gateSeq.AppendCallback(() =>
        {
            _gateSeq = null;
        });
    }
    
    private void CloseGate()
    {
        if (_gateSeq != null)
        {
            _gateSeq.PlayBackwards();
        }
        else
        {
            _gateSeq = DOTween.Sequence();
            _gateSeq.Append(_gateTransform.DOMove(_startGatePos, 2f));
        }
    }
}