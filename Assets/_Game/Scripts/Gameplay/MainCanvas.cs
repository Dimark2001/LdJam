using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : Singleton<MainCanvas>
{
    [SerializeField]
    private TextMeshProUGUI _catsWeightText;

    [SerializeField]
    private TextMeshProUGUI _boxCount;

    [SerializeField]
    private TextMeshProUGUI _catsStrengthText;

    [SerializeField]
    private Button _restartButton;
    
    [SerializeField]
    private Button _menuButton;
    
    [SerializeField]
    private Button _pauseButton;
    
    [SerializeField]
    private Button _skipLevel;
    
    [SerializeField]
    private GameObject _pausePanel;

    private void Start()
    {
        CatManager.Instance.OnSelectedCatsChanged += UpdateVisual;
        _restartButton.onClick.AddListener(SceneController.ReloadScene);
        _pauseButton.onClick.AddListener(Pause);
        _menuButton.onClick.AddListener(Menu);
        _skipLevel.onClick.AddListener(SceneController.LoadNextScene);
    }

    private void Update()
    {
        if (_boxCount != null)
        {
            _boxCount.text = $"{BoxTriggerZone._countBox}/{MovableObject.MovableObjects.Count}";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pausePanel.activeSelf)
            {
                _pausePanel.SetActive(false);
                return;
            }
            Pause();
        }
    }

    private void Pause()
    {
        _pausePanel.SetActive(true);
    }
    
    public void Menu()
    {
        SceneController.Menu();
    }
    
    private void OnDestroy()
    {
        CatManager.Instance.OnSelectedCatsChanged -= UpdateVisual;
        _restartButton.onClick.RemoveListener(SceneController.ReloadScene);
    }

    public void UpdateVisual()
    {
        if (_catsStrengthText != null)
        {
            _catsStrengthText.text = $"{CatManager.Instance.GetSummaryStrength()}";
        }
    }

    public void ShowLose()
    {
        SceneController.ReloadScene();
    }

    public void ShowWinPanel()
    {
        SceneController.LoadNextScene();
    }
}