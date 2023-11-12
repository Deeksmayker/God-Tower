using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;

public class WaveController : MonoCache
{
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private UnityEvent _wavesEnded;

    private int _currentWaveNumber;
    private bool _wavesIsStarted;

    private void StartWaves()
    {
        _waves[0]?.StartWave();

        for (int i = 0; i < _waves.Count; i++)
        {
            _waves[i].Ended += () =>
            {
                _currentWaveNumber += 1;

                if (_waves.Count <= _currentWaveNumber)
                {
                    _wavesEnded?.Invoke();
                    return;
                }

                _waves[_currentWaveNumber].StartWave();
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerUnit>() && !_wavesIsStarted)
        {
            StartWaves();
            _wavesIsStarted = true;
        }
    }
}