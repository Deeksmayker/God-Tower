using System;
using DG.Tweening;
using UnityEngine;

namespace Code.Global.Animations
{
    [Serializable]
    public class ShakeAnimationPreset : AnimationPreset
    {
        public ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;
        public Vector3 strength;
        public float randomness = 90f;
        public int vibrato = 10;
        public bool snapping;
    }
}