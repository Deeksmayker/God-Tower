using UnityEngine;

public static class MathUtils
{
    public static float CalculateLaunchAngle(float distanceToTarget, float launchVelocity, float heightDifference, float gravity)
    {
        float angleRad = Mathf.Atan((Mathf.Pow(launchVelocity, 2f) - Mathf.Sqrt(Mathf.Pow(launchVelocity, 4f) -
            gravity * (gravity * Mathf.Pow(distanceToTarget, 2f) +
                       2f * heightDifference * Mathf.Pow(launchVelocity, 2f)))) / (gravity * distanceToTarget));
        return angleRad * Mathf.Rad2Deg; // Convert to degrees before returning
    }
    
    public static float CalculateFlightTime(Vector3 position, Vector3 targetPosition, float launchAngleRad, float projectileVelocity, float gravity)
    {
        var distanceToTarget = Vector3.Distance(position, targetPosition);
        gravity = Mathf.Abs(gravity);
        float height = Mathf.Abs(targetPosition.y - position.y);
        
        float time = (distanceToTarget / (projectileVelocity * Mathf.Cos(launchAngleRad))) +
            Mathf.Sqrt(2f * height / gravity) - 1;

        return Mathf.Max(time, 0.1f);
    }
}
