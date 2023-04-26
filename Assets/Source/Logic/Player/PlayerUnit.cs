using System;
using NTC.Global.Cache;
using UnityEngine;

public class PlayerUnit : MonoCache
{
    private void Awake()
    {
        Application.targetFrameRate = 200;
    }
}
