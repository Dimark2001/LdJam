using System;
using UnityEngine;

public class CatModel
{
    public float Weight { get; private set; }
    public float Strength { get; private set; }
    
    public Vector3 Scale { get; private set; }
    public Color Color { get; set; }
    
    public CatModel(float strength, Vector3 scale, Color color)
    {
        Strength = strength;
        Scale = scale;
        Color = color;
    }

    public CatModel(CatModel newCatModel)
    {
        Strength = newCatModel.Strength;
        Scale = newCatModel.Scale;
        Color = newCatModel.Color;
    }
}