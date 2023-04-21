using System;

namespace Code.Global.Animations
{
    [Serializable]
    public class JumpAnimationPreset : AnimationPreset
    {
        public float jumpPower = 1f;
        public int jumpCount = 1;
    }
}