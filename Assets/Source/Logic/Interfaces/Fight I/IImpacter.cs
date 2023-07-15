using System;
using UnityEngine;

public interface IImpacter
{
    public event Action<Vector3> OnImpact;
}