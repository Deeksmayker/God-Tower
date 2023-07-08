using NTC.Global.Cache;
using UnityEngine;

public class AudioPlaybackSpeedController : MonoCache
{
    private float _basePitch;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    protected override void OnEnabled()
    {
        _basePitch = _source.pitch;
    }

    private void Update()
    {
        if (TimeController.Instance.IsPaused)
        {
            _source.pitch = 0;
            return;
        }

        if (Time.timeScale.Equals(1) || Time.timeScale.Equals(0))
        {
            if (!_source.pitch.Equals(_basePitch))
                _source.pitch = _basePitch;
            return;
        }

        _source.pitch = Mathf.Lerp(0.05f, _basePitch, Time.timeScale);
    }
}