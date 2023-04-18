using DG.Tweening;
using UnityEngine;
using Zenject;

public class CameraRotating : MonoBehaviour
{
    /*public float _cameraRotationSpeed = 0.1f;
    [SerializeField] private Transform cameraTransform;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            tiltCamera(true); // Replace parameter with bool
        }
        else if (Input.GetKey(KeyCode.D))
        {
            tiltCamera(false); // Replace parameter with bool
        }
    }

    void tiltCamera(bool rotateRight)
    {
        float angle = cameraTransform.eulerAngles.y;
        
        if (rotateRight)
            angle += _cameraRotationSpeed;
        else
            angle -= _cameraRotationSpeed;

        angle = Mathf.Clamp(angle, 20, 70);

        cameraTransform.eulerAngles = new Vector3(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, angle);
    }*/





    public static CameraRotating Instance;

    [Inject] private PlayerUnit _player;
    private IMover _mover;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int angleOfRotationForZ = 20;
    [SerializeField] private int angleOfRotationForX = 40;
    [SerializeField] private Ease easeForRotation;
    [SerializeField] private Ease easeForReturnToStraightPosition;
    [SerializeField] private float timeForRotation = 1;
    [SerializeField] private float timeForReturnToStraightPosition = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mover = _player.Get<IMover>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateToLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RotateToRight();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RotateToBack();
        }

        if(Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.S))
        {
            ReturnToStraightPosition();
        }
    }

    [ContextMenu("Test RotateToRight")]
    public void RotateToRight()// z-=
    {
        Vector3 necessaryRotation = new Vector3(cameraTransform.rotation.x,
            cameraTransform.rotation.y, cameraTransform.rotation.z - angleOfRotationForZ);

        cameraTransform.DORotate(necessaryRotation, timeForRotation).SetEase(easeForRotation).OnComplete(() =>
        {
            /*cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x - angleOfRotationForX,
                cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);*/
        });


    }

    [ContextMenu("Test RotateToLeft")]
    public void RotateToLeft() // z+=
    {
        Vector3 necessaryRotation = new Vector3(cameraTransform.rotation.x,
            cameraTransform.rotation.y, cameraTransform.rotation.z + angleOfRotationForZ);

        cameraTransform.DORotate(necessaryRotation, timeForRotation).SetEase(easeForRotation).OnComplete(() =>
        {
            /*cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x - angleOfRotationForX,
                cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);*/
        });


    }

    [ContextMenu("Test RotateToBack")]
    public void RotateToBack() // x-=
    {
        Vector3 necessaryRotation = new Vector3(cameraTransform.rotation.x - angleOfRotationForX,
            cameraTransform.rotation.y, cameraTransform.rotation.z);

        cameraTransform.DORotate(necessaryRotation, timeForReturnToStraightPosition).SetEase(easeForRotation).OnComplete(() =>
        {
            /*cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x - angleOfRotationForX,
                cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);*/
        });


    }

    [ContextMenu("Test ReturnToStraightPosition")]
    public void ReturnToStraightPosition()
    {
        Vector3 necessaryRotation = Vector3.zero;
        cameraTransform.DORotate(necessaryRotation, timeForReturnToStraightPosition).SetEase(easeForReturnToStraightPosition).
            OnComplete(() =>
            {
                /*cameraTransform.rotation = new Quaternion(cameraTransform.rotation.x - angleOfRotationForX,
                    cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);*/
            });
        ;
    }
}
