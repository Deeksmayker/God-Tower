using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeController : MonoCache
{
    public static TimeController Instance;

    private List<TimeScaleTimer> _timers = new List<TimeScaleTimer>();
    [SerializeField] private TimeScaleTimer _currentTimer;
    [FormerlySerializedAs("_isPaused")] public bool IsPaused = false;

    private float _currentTimeScale = 1;
    private float _timeStopTimer;
    private float _maxTimeStop;
    private float _restoreTimeScaleT;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    protected override void Run()
    {
        if (IsPaused || _maxTimeStop <= 0)
            return;

        if (_timeStopTimer > 0)
        {
            Time.timeScale = 0;
            _currentTimeScale = Time.timeScale;
            _timeStopTimer -= Time.unscaledDeltaTime;
        }

        if (_timeStopTimer <= 0)
        {
            _restoreTimeScaleT += Time.unscaledDeltaTime / (_maxTimeStop * 5);
            Time.timeScale = Mathf.Lerp(0, 1, Mathf.Pow(_restoreTimeScaleT, 2));
            _currentTimeScale = Time.timeScale;

            if (_restoreTimeScaleT >= 1)
            {
                _restoreTimeScaleT = 0;
                _maxTimeStop = 0;
                _timeStopTimer = 0;
                Time.timeScale = 1;
            }
        }
    }

    public async UniTask SetTimeScale(float timeScale, float duration)
    {
        Time.timeScale = timeScale;

        await UniTask.Delay(TimeSpan.FromSeconds(duration), DelayType.UnscaledDeltaTime);

        if (IsPaused)
            return;

        Time.timeScale = _currentTimeScale;

        /*TimeScaleTimer newTimer = new TimeScaleTimer(timeScale, duration);
        _timers.Add(newTimer);

        TimeScaleTimer timerWithMinTimeScale = _timers.OrderBy(t => t.timeScale).FirstOrDefault();

        if (_currentTimer == null || timerWithMinTimeScale.timeScale < _currentTimer.timeScale)
        {
            _currentTimer = timerWithMinTimeScale;
            _currentTimer.coroutine = StartCoroutine(_currentTimer.StartTimer());
        }*/
    }

    public void AddTimeStopDuration(float addedDuration)
    {
        _timeStopTimer += addedDuration;
        _maxTimeStop += addedDuration;
    }

    public void SetPause(bool isPaused)
    {
        IsPaused = isPaused;

        Time.timeScale = isPaused ? 0 : 1;
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