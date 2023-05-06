using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;

public class TimeController : MonoCache
{
    public static TimeController Instance;

    private List<TimeScaleTimer> _timers = new List<TimeScaleTimer>();
    [SerializeField] private TimeScaleTimer _currentTimer;
    private bool _isPaused = false;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public async UniTask SetTimeScale(float timeScale, float duration)
    {
        Time.timeScale = timeScale;

        await UniTask.Delay(TimeSpan.FromSeconds(duration), DelayType.UnscaledDeltaTime);

        Time.timeScale = 1;

        /*TimeScaleTimer newTimer = new TimeScaleTimer(timeScale, duration);
        _timers.Add(newTimer);

        TimeScaleTimer timerWithMinTimeScale = _timers.OrderBy(t => t.timeScale).FirstOrDefault();

        if (_currentTimer == null || timerWithMinTimeScale.timeScale < _currentTimer.timeScale)
        {
            _currentTimer = timerWithMinTimeScale;
            _currentTimer.coroutine = StartCoroutine(_currentTimer.StartTimer());
        }*/
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

        foreach (TimeScaleTimer timer in _timers.Where(t => !t.isFinished))
        {
            if (nextTimer == null || timer.timeScale < nextTimer.timeScale)
            {
                nextTimer = timer;
            }
        }

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
        public float timer;

        private bool _isPaused;

        public TimeScaleTimer(float timeScale, float duration)
        {
            this.timeScale = timeScale;
            this.duration = duration;
        }

        public IEnumerator StartTimer()
        {
            timer = 0;
            while (timer < duration)
            {
                if (!_isPaused)
                    timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}