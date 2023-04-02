using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float _minTimeScale = 1f; // Минимальный timeScale
    private bool _isPaused = false; // Флаг, указывающий, находится ли игра на паузе
    private List<TimeScaleTimer> _timers = new List<TimeScaleTimer>(); // Список таймеров

    // Метод для установки timeScale на заданный промежуток времени
    public void SetTimeScale(float timeScale, float duration)
    {
        // Если timeScale равен нулю, игра ставится на паузу
        if (timeScale == 0)
        {
            SetPause(true);
            return;
        }

        // Добавляем новый таймер в список
        TimeScaleTimer newTimer = new TimeScaleTimer(timeScale, duration);
        _timers.Add(newTimer);

        // Если новый таймер имеет минимальный timeScale, запускаем его сразу
        if (timeScale < _minTimeScale)
        {
            _minTimeScale = timeScale;
            StartCoroutine(newTimer.StartTimer());
        }
    }

    // Метод для установки игры на паузу или снятия ее с паузы
    public void SetPause(bool isPaused)
    {
        _isPaused = isPaused;

        // Если игра снимается с паузы, запускаем следующий таймер
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

    // Метод для получения следующего таймера
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

    // Класс для хранения информации о таймере
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

        // Метод для запуска таймера
        public IEnumerator StartTimer()
        {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}