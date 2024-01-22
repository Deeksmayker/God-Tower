using System;
using System.Collections;
using UnityEngine;

public static class Timer
{
    public static IEnumerator TakeActionAfterTime(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}
