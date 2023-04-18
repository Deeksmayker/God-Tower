using UnityEngine;
using DG.Tweening;
using Zenject;

public class CameraInerting : MonoBehaviour
{
    public static CameraInerting Instance;

    [Inject] private PlayerUnit _player;
    private IMover _mover;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float timeForInerting = 1;
    [SerializeField] private float coordinatesForInerting = 10;
    private Sequence _inertingSequence;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _inertingSequence = DOTween.Sequence();
    }

    private void Update()
    {
     
    }

    [ContextMenu("Test InertToRight")]
    public void InertToRight() // x+
    {
        _inertingSequence.Kill();
        float startedX = cameraTransform.position.x;

        _inertingSequence.Append(cameraTransform.DOMoveX(cameraTransform.position.x + coordinatesForInerting, timeForInerting))
            .Append(cameraTransform.DOMoveX(startedX, timeForInerting));
    }

    [ContextMenu("Test InertToLeft")]
    public void InertToLeft() //  x-
    {
        _inertingSequence.Kill();
        float startedX = cameraTransform.position.x;

        _inertingSequence.Append(cameraTransform.DOMoveX(cameraTransform.position.x - coordinatesForInerting, timeForInerting))
            .Append(cameraTransform.DOMoveX(startedX, timeForInerting));
    }

    [ContextMenu("Test InertToBack")]
    public void InertToBack() // z-
    {
        _inertingSequence.Kill();
        float startedZ = cameraTransform.position.z;

        _inertingSequence.Append(cameraTransform.DOMoveZ
            (cameraTransform.position.x - coordinatesForInerting, timeForInerting))
            .Append(cameraTransform.DOMoveZ(startedZ, timeForInerting));
    }

    [ContextMenu("Test InertToForward")]
    public void InertToForward() // z+
    {
        _inertingSequence.Kill();
        float startedZ = cameraTransform.position.z;

        _inertingSequence.Append(cameraTransform.DOMoveZ
            (cameraTransform.position.x + coordinatesForInerting, timeForInerting))
            .Append(cameraTransform.DOMoveZ(startedZ, timeForInerting));
    }
}
