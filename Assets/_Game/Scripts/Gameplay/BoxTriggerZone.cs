using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoxTriggerZone : MonoBehaviour
{
    [SerializeField]
    private float _waitToWin = 3f;
    
    [SerializeField]
    private Image _loadImage;
    
    public static int _countBox = 0;
    private bool _isWin = false;
    private Coroutine _winCoroutine;
    private Sequence _winSeq;

    private void Awake()
    {
        _countBox = 0;
    }

    private void Update()
    {
        _loadImage.transform.position = transform.position + new Vector3(0f, 0.01f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MovableObject movableObject))
        {
            _countBox++;
            AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.Collect));
            if (_countBox == MovableObject.MovableObjects.Count)
            {
                _isWin = true;
                _winCoroutine = StartCoroutine(TryWin());
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MovableObject movableObject))
        {
            _countBox--;

            if (_winCoroutine != null)
            {
                _isWin = false;
                _winSeq.Kill();
                _loadImage.gameObject.SetActive(false);
                StopCoroutine(_winCoroutine);
            }
        }
    }

    private IEnumerator TryWin()
    {
        _loadImage.gameObject.SetActive(true);
        _winSeq = DOTween.Sequence();
        _loadImage.fillAmount = 0f;
        _winSeq.Append(_loadImage.DOFillAmount(1, _waitToWin));
        
        yield return new WaitForSeconds(_waitToWin);

        if (_isWin)
        {
            MainCanvas.Instance.ShowWinPanel();
        }
    }
}