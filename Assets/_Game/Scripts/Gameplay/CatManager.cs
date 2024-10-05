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
        }
    }

    private CatController _selectedCat;

    protected override void Awake()
    {
        base.Awake();
        var cat = FindObjectOfType<CatController>();

        _catControllers = new List<CatController> { cat };
        SelectedCats = new List<CatController> { cat };

        var catModel = new CatModel(2f, 5f, Vector3.one, Color.white);
        cat.SetModel(catModel);
        SelectOneCat(cat);

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
        
        yield break;

        void UpdateSelectedCats()
        {
            if (SelectedCats.Count == 0)
            {
                return;
            }
            for (var i = 0; i < SelectedCats.Count; i++)
            {
                var cat = SelectedCats[i];
                cat.Move();
                switch (cat.TryInteractWithMovableObject())
                {
                    case InteractType.None:
                        continue;
                    case InteractType.Grab:
                        break;
                    case InteractType.Release:
                        break;
                }
            }
            TryCatMitosis();
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
                rad += Time.deltaTime;
                SelectedCat.UpdateSelectCircle(rad);
            }
            
            if (Input.GetKeyUp(KeyCode.Space))
            {
                var newSelectedCats = new List<CatController>(_catControllers.FindAll(cat => Vector3.Distance(cat.transform.position, SelectedCat.transform.position) <= rad));
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
            }
        }
    }

    public IEnumerator CatMitosis(CatController cat)
    {
        var newCat = Instantiate(_catPrefab, cat.transform.parent);
        newCat.transform.position = cat.transform.position;
        var newScale = cat.transform.localScale * 0.8f;
        var newRandomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        var newCatModel = new CatModel(cat.Model.Strength * 0.8f, 5, newScale, newRandomColor);
        if (newScale.x < 0.4f)
        {
            RemoveCat(cat);
            StartCoroutine(newCat.Death());
            yield return cat.Death();
            SelectFirstCat();
            yield break;
        }

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

    public void TryMoveObject(MovableObject movableObject)
    {
        var strength = GetSummaryStrength();
        movableObject.CanMoveObject(strength);
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
    }
    
    public float GetSummaryStrength()
    {
        return SelectedCats.Select(cat => cat.GetStrength).Sum();
    }
}