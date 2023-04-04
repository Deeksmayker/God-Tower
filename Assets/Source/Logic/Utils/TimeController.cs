using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeController  : MonoBehaviour
{
    private List<TimeScaleTimer> _timers = new List<TimeScaleTimer>();
    private TimeScaleTimer _currentTimer;
    private bool _isPaused = false;

    public void SetTimeScale(float timeScale, float duration)
    {
        if (timeScale == 0)
        {
            SetPause(true);
            return;
        }

        TimeScaleTimer newTimer = new TimeScaleTimer(timeScale, duration);
        _timers.Add(newTimer);

        // Находим таймер с минимальным значением timeScale
        TimeScaleTimer timerWithMinTimeScale = _timers.OrderBy(t => t.timeScale).FirstOrDefault();

        if (_currentTimer == null || timerWithMinTimeScale.timeScale < _currentTimer.timeScale)
        {
            if (_currentTimer != null)
            {
                StopCoroutine(_currentTimer.coroutine);
            }

            _currentTimer = timerWithMinTimeScale;
            _currentTimer.coroutine = StartCoroutine(_currentTimer.StartTimer());
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
                _currentTimer = nextTimer;
                _currentTimer.coroutine = StartCoroutine(_currentTimer.StartTimer());
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    private TimeScaleTimer GetNextTimer()
    {
        TimeScaleTimer nextTimer = null;

        // Находим таймер с минимальным значением timeScale, который еще не закончился
        foreach (TimeScaleTimer timer in _timers.Where(t => !t.isFinished))
        {
            if (nextTimer == null || timer.timeScale < nextTimer.timeScale)
            {
                nextTimer = timer;
            }
        }

        // Удаляем из списка таймеров таймер, который закончился
        if (nextTimer != null && nextTimer.isFinished)
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
        public bool isFinished = false;

        public TimeScaleTimer(float timeScale, float duration)
        {
            this.timeScale = timeScale;
            this.duration = duration;
        }

        public IEnumerator StartTimer()
        {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(duration);
            isFinished = true;
        }
    }
}