using System;
using UnityEngine;

public interface IWorkWithStackAbilities
{
    public event Action<Vector3> OnImpact;
}