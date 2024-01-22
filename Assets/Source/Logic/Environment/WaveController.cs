using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;

public class WaveController : MonoCache
{
    [SerializeField] private bool loopWaves;
    [SerializeField] private Wave[] _waves;
    public UnityEvent WavesEnded;
    public UnityEvent WavesStarted;

    private int _currentWaveNumber;
    private bool _wavesIsStarted;

    private void StartWaves()
    {
		if (_waves.Length <= 0) return;

        _waves[0]?.StartWave();

        for (int i = 0; i < _waves.Length; i++)
        {
            _waves[i].OnEnded += HandleWaveEnd; 
        }
    }

	protected override void OnDisabled(){
		for (var i = 0; i < _waves.Length; i++){
			_waves[i].OnEnded -= HandleWaveEnd;
		}
	}

	private void HandleWaveEnd(){
		Log("Wave " + _currentWaveNumber + " Ended");
		_currentWaveNumber += 1;

		if (_waves.Length <= _currentWaveNumber)
		{
			Log("All waves ended");
			WavesEnded?.Invoke();
			if (loopWaves){
			    _currentWaveNumber = 0;
			    _waves[0].StartWave();
			}
			return;
		}

		_waves[_currentWaveNumber].StartWave();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerUnit>() && !_wavesIsStarted)
        {
            StartWaves();
            _wavesIsStarted = true;
			WavesStarted.Invoke();
        }
    }
}
