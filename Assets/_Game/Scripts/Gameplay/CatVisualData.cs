using UnityEngine;

[CreateAssetMenu(menuName = "Create CatVisualData", fileName = "CatVisualData", order = 0)]
public class CatVisualData : ScriptableObject
{
    public Sprite AwakeSprite;
    public Sprite SleepSprite;
    public Sprite DeadSprite;
}