using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float _minTimeScale = 1f;
    private bool _isPaused = false;
    private List<TimeScaleTimer> _timers = new List<TimeScaleTimer>();

    public void SetTimeScale(float timeScale, float duration)
    {
        if (timeScale == 0)
        {
            SetPause(true);
            return;
        }

        TimeScaleTimer newTimer = new TimeScaleTimer(timeScale, duration);
        _timers.Add(newTimer);

        if (timeScale < _minTimeScale)
        {
            _minTimeScale = timeScale;
            StartCoroutine(newTimer.StartTimer());
        }
    }

    public void SetPause(bool isPaused)
    {
        _isPaused = isPaused;

        if (!_isPaused && _timers.Count > 0)
        {
            TimeScaleTimer nextTimer = GetNextTimer();
            if (nextTimer != null)
            {
                _minTimeScale = nextTimer.timeScale;
                StartCoroutine(nextTimer.StartTimer());
            }
        }
    }

    private TimeScaleTimer GetNextTimer()
    {
        TimeScaleTimer nextTimer = null;
        foreach (TimeScaleTimer timer in _timers)
        {
            if (nextTimer == null || timer.timeScale < nextTimer.timeScale)
            {
                nextTimer = timer;
            }
        }
        if (nextTimer != null)
        {
            _timers.Remove(nextTimer);
        }
        return nextTimer;
    }

    private class TimeScaleTimer
    {
        public float timeScale;
        public float duration;
        public Coroutine coroutine;

        public TimeScaleTimer(float timeScale, float duration)
        {
            this.timeScale = timeScale;
            this.duration = duration;
        }

        public IEnumerator StartTimer()
        {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}