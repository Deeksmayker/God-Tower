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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
        {
            _inertingSequence.Kill();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            InertToLeft();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            InertToRight();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            InertToBack();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            InertToForward();
        }

        
    }

    public void InertToRight() // x+
    {
        _inertingSequence.Kill();

        _inertingSequence.Append(cameraTransform.DOMoveX(cameraTransform.position.x + coordinatesForInerting, timeForInerting));
    }

    public void InertToLeft() //  x-
    {
        _inertingSequence.Kill();

        _inertingSequence.Append(cameraTransform.DOMoveX(cameraTransform.position.x - coordinatesForInerting, timeForInerting));
    }

    public void InertToBack() // z-
    {
        _inertingSequence.Kill();

        _inertingSequence.Append(cameraTransform.DOMoveZ
            (cameraTransform.position.z - coordinatesForInerting, timeForInerting));
    }

    public void InertToForward() // z+
    {
        _inertingSequence.Kill();

        _inertingSequence.Append(cameraTransform.DOMoveZ
            (cameraTransform.position.z + coordinatesForInerting, timeForInerting));
    }
}
