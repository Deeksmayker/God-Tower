using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

public class PlayerData
{
    private float _healthPoint;

    public PlayerData() { }

    public void SetHealthPoint(float healthPoint)
    {
        _healthPoint = healthPoint;
    }

    public float GetHealthPoint()
    {
        return _healthPoint;
    }
}
