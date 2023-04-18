using DG.Tweening;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    public static CameraShaking Instance;

    [SerializeField] private Transform cameraTransform;

    private bool _isShaking;
    private bool _isShakingWithTime;

    private float _amplitude;
    private int _frequancy;

    private void Awake()
    {
        Instance = this;
    }

    public void StartShake(float amplitude, int frequancy) // amplitude - ������ �� �����������, frequancy - ��������� ����� �������� ����������� (��������� �������� - ������, ������� - ����� � �����)
    {
        _isShaking = true; 

        _amplitude = amplitude;
        _frequancy = frequancy;

        cameraTransform.DOShakePosition(0.1f, amplitude, frequancy, 90, false, false).OnComplete(() =>
        {
            if (_isShaking == true && _isShakingWithTime == false)
            {
                StartShake(amplitude, frequancy);
            }
        });
    }

    public void StopShake()
    {
        _isShaking = false;
    }

    public void StartShakeWithTime(int shakingTime, float amplitude, int frequancy)
    {
        _isShakingWithTime = true;
        cameraTransform.DOShakePosition(shakingTime, amplitude, frequancy, 90, false, false).OnComplete(() =>
        {
            _isShakingWithTime = false;

            if (_isShaking == true)
                StartShake(_amplitude, _frequancy);
        });
    }
}
