using UnityEngine;

public static class MathUtils
{
    public static float CalculateLowLaunchAngle(float distanceToTarget, float launchVelocity, float heightDifference, float gravity)
    {
        //heightDifference = Mathf.Abs(heightDifference);
        //gravity = Mathf.Abs(gravity);
        
        float angleRad = Mathf.Atan((Mathf.Pow(launchVelocity, 2f) - Mathf.Sqrt(Mathf.Pow(launchVelocity, 4f) -
            gravity * (gravity * Mathf.Pow(distanceToTarget, 2f) +
                       2f * heightDifference * Mathf.Pow(launchVelocity, 2f)))) / (gravity * distanceToTarget));
        return angleRad * Mathf.Rad2Deg; // Convert to degrees before returning
    }
    
    public static float CalculateHighLaunchAngle(float distanceToTarget, float launchVelocity, float heightDifference, float gravity)
    {
        //heightDifference = Mathf.Abs(heightDifference);
        //gravity = Mathf.Abs(gravity);
        
        float angleRad = Mathf.Atan((Mathf.Pow(launchVelocity, 2f) + Mathf.Sqrt(Mathf.Pow(launchVelocity, 4f) -
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
    
    public static bool CompareNumsApproximately(float first, float second, float allowedDifference)
    {
        var d = first - second;

        return Mathf.Abs(d) < allowedDifference;
    }

    public static bool CompareVectors(Vector2 me, Vector2 other, float allowedDifference = 0.01f)
    {
        var dx = me.x - other.x;
        if (Mathf.Abs(dx) > allowedDifference)
            return false;

        var dy = me.y - other.y;
        if (Mathf.Abs(dy) > allowedDifference)
            return false;

        return true;
    }

    public static Vector3 GetHorizontalFromVector(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }
}
