using UnityEngine;


public static class RandomUtils
{
    public static Vector3 GetRandomNormalizedVector()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
}
