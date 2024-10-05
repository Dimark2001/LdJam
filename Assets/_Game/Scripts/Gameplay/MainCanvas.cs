using TMPro;
using UnityEngine;

public class MainCanvas : Singleton<MainCanvas>
{
    [SerializeField]
    private TextMeshProUGUI _catsStrengthText;

    private void Start()
    {
        CatManager.Instance.OnSelectedCatsChanged += UpdateVisual;
    }

    public void UpdateVisual()
    {
        _catsStrengthText.text = $"{CatManager.Instance.GetSummaryStrength()}";
    }

    public void ShowLose()
    {
        SceneController.ReloadScene();
    }
}