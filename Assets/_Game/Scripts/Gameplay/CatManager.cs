using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    public event Action OnSelectedCatsChanged;

    [SerializeField]
    private CatController _catPrefab;

    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtual;

    private List<CatController> _catControllers;

    private List<CatController> SelectedCats;

    public CatController SelectedCat
    {
        get => _selectedCat;
        set
        {
            if (value == null)
            {
                _selectedCat = value;
                MainCanvas.Instance.ShowLose();
                return;
            }

            _selectedCat = value;
            OnSelectedCatsChanged?.Invoke();
            _cinemachineVirtual.Follow = value.transform;
            AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.SelectOther));
        }
    }

    private CatController _selectedCat;

    private MovableObject _movableObject;

    protected override void Awake()
    {
        base.Awake();
        var cat = FindObjectOfType<CatController>();

        _catControllers = new List<CatController> { cat };
        SelectedCats = new List<CatController> { cat };

        var catModel = new CatModel(5f, new Vector3(1, 0.2f, 1), Color.white);
        cat.SetModel(catModel);
        SelectOneCat(cat);
        MainCanvas.Instance.UpdateVisual();
        StartCoroutine(UpdateCats());
    }

    private IEnumerator UpdateCats()
    {
        var rad = 0f;
        while (true)
        {
            UpdateSelectedCats();
            TrySelectNextCat();
            TrySelectPreviousCat();
            TrySelectManyCats();
            yield return null;
        }

        void UpdateSelectedCats()
        {
            if (SelectedCats.Count == 0)
            {
                return;
            }

            for (var i = 0; i < SelectedCats.Count; i++)
            {
                var cat = SelectedCats[i];
                if (TryInteractWithMovableObject(cat))
                {
                    break;
                }
            }

            TryCatMitosis();
        }

        bool TryInteractWithMovableObject(CatController cat)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (_movableObject == null)
                {
                    var interactResult = cat.TryInteractWithMovableObject();
                    if (interactResult != null && interactResult.CanMoveObject(GetSummaryStrength()))
                    {
                        var pos = GetSummaryPos();
                        cat.Grab(interactResult, pos);
                        _movableObject = interactResult;
                        return true;
                    }
                }
                else
                {
                    _movableObject.Release();
                    _movableObject = null;
                    return true;
                }
            }

            return false;
        }

        void TryCatMitosis()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StartCoroutine(CatMitosis(SelectedCat));
            }
        }

        void TrySelectNextCat()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var index = _catControllers.IndexOf(SelectedCat);
                index++;

                if (index >= _catControllers.Count)
                {
                    index = 0;
                }

                SelectOneCat(_catControllers[index]);

                MainCanvas.Instance.UpdateVisual();
            }
        }

        void TrySelectPreviousCat()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var index = _catControllers.IndexOf(SelectedCat);
                index--;
                if (index < 0)
                {
                    index = _catControllers.Count - 1;
                }

                SelectOneCat(_catControllers[index]);

                MainCanvas.Instance.UpdateVisual();
            }
        }

        void TrySelectManyCats()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                AudioManager.PlayAudioClip(LoadFromResource.LoadAudioClip(LoadFromResource.SelectMany), true);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                rad += Time.deltaTime * 7f;
                SelectedCat.UpdateSelectCircle(rad);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                AudioManager.StopAudio();
                var newSelectedCats = new List<CatController>(_catControllers.FindAll(cat =>
                    Vector3.Distance(cat.transform.position, SelectedCat.transform.position) <= rad));
                foreach (var cat in SelectedCats)
                {
                    cat.Deselect();
                }

                SelectedCats = newSelectedCats;
                foreach (var cat in SelectedCats)
                {
                    cat.Select();
                }

                Debug.Log("rad - " + rad);
                rad = 0f;

                SelectedCat.UpdateSelectCircle(rad);
                OnSelectedCatsChanged?.Invoke();
            }
        }
    }

    public IEnumerator CatMitosis(CatController cat)
    {
        var newScale = cat.transform.localScale * 0.8f;
        if (newScale.x < 0.4f)
        {
            RemoveCat(cat);
            yield return cat.Death();
            SelectFirstCat();
            yield break;
        }
        var newCat = Instantiate(_catPrefab, cat.transform.parent);
        newCat.transform.position = cat.transform.position;
        var newRandomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        var newStrength = (float)Math.Round(cat.Model.Strength * 0.6f, 1);
        var newCatModel = new CatModel(newStrength, newScale, newRandomColor);

        newCat.SetModel(newCatModel);
        var oldCatModel = new CatModel(newCatModel)
        {
            Color = cat.Model.Color
        };
        cat.SetModel(oldCatModel);
        AddCat(newCat);
        newCat.NormalizePos();

        SelectOneCat(cat);
    }

    private void AddCat(CatController catController)
    {
        _catControllers.Add(catController);
    }

    private void RemoveCat(CatController catController)
    {
        _catControllers.Remove(catController);
        SelectedCats.Remove(catController);
    }

    private void SelectFirstCat()
    {
        if (_catControllers.Count == 0)
        {
            SelectedCat = null;
            return;
        }

        SelectOneCat(_catControllers[0]);
    }

    private void SelectOneCat(CatController cat)
    {
        foreach (var selectedCat in _catControllers)
        {
            selectedCat.Deselect();
        }

        SelectedCats.Clear();
        SelectedCats.Add(cat);
        SelectedCat = cat;
        SelectedCat.Select();

        if (_movableObject != null)
        {
            _movableObject.Release();
            _movableObject = null;
        }
    }

    public float GetSummaryStrength()
    {
        var summaryStrength = SelectedCats.Select(cat => cat.GetStrength).Sum();
        Debug.Log(summaryStrength);
        return summaryStrength;
    }
    
    private Vector3 GetSummaryPos()
    {
        var vector3s = SelectedCats.Select(cat => cat.transform.position);
        return vector3s.Aggregate(Vector3.zero, (current, vector3) => current + vector3) / SelectedCats.Count;
    }
    
}