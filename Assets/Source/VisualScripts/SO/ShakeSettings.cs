using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class ShakeSettings
{
    public bool isOn = true;
    public float duration = 1f;
    public ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;
    public Ease easeMode = Ease.Linear;
    public Vector3 strength;
    public float randomness = 90f;
    public int vibrato = 10;
    public bool snapping;
    public bool fadeOut = true;
}
