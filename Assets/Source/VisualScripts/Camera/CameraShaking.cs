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

    public void StartShake(float amplitude, int frequency) // amplitude - размах по координатам, frequancy - насколько часто меняется направление (маленькое значение - плавно, большое - часто и резко)
    {
        _isShaking = true; 

        _amplitude = amplitude;
        _frequancy = frequency;

        cameraTransform.DOShakePosition(0.1f, amplitude, frequency, 90, false, false).OnComplete(() =>
        {
            if (_isShaking == true && _isShakingWithTime == false)
            {
                StartShake(amplitude, frequency);
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
