using UnityEngine;

[CreateAssetMenu(menuName = "SO/ShakePreset", fileName = "Shake preset")]
public class ShakePreset : ScriptableObject
{
    public ShakeSettings positionShake;
    public ShakeSettings rotationShake;
}
