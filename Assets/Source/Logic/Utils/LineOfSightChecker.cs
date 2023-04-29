using UnityEngine;

public static class LineOfSightChecker
{
    public static bool CanSeeTarget(Vector3 startPoint, Vector3 target, LayerMask environmentLayers)
    {
        return !Physics.Raycast(startPoint, target - startPoint,
            Vector3.Distance(target, startPoint), environmentLayers);
    }
}