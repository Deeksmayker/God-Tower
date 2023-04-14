using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    public static CameraShaking Instance;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _basicMultiChannelPerlin;
    private bool _isShaking;
    private int _amplitude;
    private int _frequancy;

    private void Awake()
    {
        Instance = this;
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _basicMultiChannelPerlin = _virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    public void StartShake(int amplitude, int frequancy)
    {
        _isShaking = true;
        _amplitude = amplitude;
        _frequancy = frequancy;
        SetAmplitudeAndFrequency(_amplitude, _frequancy);
    }

    public void StopShake()
    {
        _isShaking = false;
        SetAmplitudeAndFrequency(0, 0);
    }

    public void StartShakeWithTime(int amplitude, int frequancy, int shakingTime)
    {
        StartShakeWithTime_UniTask(amplitude, frequancy, shakingTime);
    }

    private async UniTask StartShakeWithTime_UniTask(int amplitude, int frequancy, int shakingTime)
    {
        SetAmplitudeAndFrequency(amplitude, frequancy);
        await UniTask.Delay(TimeSpan.FromSeconds(shakingTime));
        StopShakeWithTime();
    }

    private void StopShakeWithTime()
    {
        if (_isShaking)
            SetAmplitudeAndFrequency(_amplitude, _frequancy);
        else
            StopShake();
    }

    private void SetAmplitudeAndFrequency(int amplitude, int frequancy)
    {
        _basicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        _basicMultiChannelPerlin.m_FrequencyGain = frequancy;
    }
}
