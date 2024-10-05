using UnityEngine;

public class CatModel
{
    public float Strength { get; private set; }
    
    public float Speed { get; private set; }
    
    public Vector3 Scale { get; private set; }
    public Color Color { get; set; }
    
    public CatModel(float strength, float speed, Vector3 scale, Color color)
    {
        Strength = strength;
        Speed = speed;
        Scale = scale;
        Color = color;
    }

    public CatModel(CatModel newCatModel)
    {
        Strength = newCatModel.Strength;
        Speed = newCatModel.Speed;
        Scale = newCatModel.Scale;
        Color = newCatModel.Color;
    }
}