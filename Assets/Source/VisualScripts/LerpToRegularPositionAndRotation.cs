using UnityEngine;


public class LerpToRegularPositionAndRotation : MonoBehaviour
{
    [SerializeField, Min(0f)] private float positionLerpSpeed = 8f;
    [SerializeField, Min(0f)] private float rotationLerpSpeed = 8f;

    private Transform _cachedTransform;

    private Vector3 _regularCameraLocalPosition;
    private Quaternion _regularCameraLocalRotation;

    private void Awake()
    {
        _cachedTransform = transform;
        _regularCameraLocalRotation = _cachedTransform.localRotation;
        _regularCameraLocalPosition = _cachedTransform.localPosition;
    }

    private void LateUpdate()
    {
        var position = _cachedTransform.localPosition;
        var rotation = _cachedTransform.localRotation;

        rotation = Quaternion.Lerp(rotation, _regularCameraLocalRotation, Time.deltaTime * rotationLerpSpeed);
        position = Vector3.Lerp(position, _regularCameraLocalPosition, Time.deltaTime * positionLerpSpeed);

        _cachedTransform.localRotation = rotation;
        _cachedTransform.localPosition = position;
    }
}
