using System;
using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject playerUnit;
    [SerializeField] private GameObject arm;

    private Vector3 _originalPosition = Vector3.zero;
    private float _landingOffset = -0.5f;

    bool _isJump;
    bool _isLand;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(NewJumpEffect());
    }

    private IEnumerator NewJumpEffect()
    {
        Vector3 targetPosition = _originalPosition - new Vector3(0f, _landingOffset, 0f);

        float duration1 = 0.1f;
        float duration2 = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration1)
        {
            transform.localPosition = -Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration1);
            //arm.transform.localPosition = -Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        targetPosition = Vector3.zero;
        _originalPosition = transform.localPosition;

        while (elapsedTime < duration2)
        {
            transform.localPosition = Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration2);
            //arm.transform.localPosition = Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        //arm.transform.localPosition = Vector3.zero;
        _originalPosition = Vector3.zero;

        _isJump = false;

        while (Math.Abs(playerUnit.transform.localPosition.y - 13.77f) > 0.3f)
            yield return new WaitForSeconds(0.1f);

        StartCoroutine(LandEffect());
    }

    private IEnumerator LandEffect()
    {
        if (_isLand == false)
        {
            _isLand = true;

            Vector3 targetPosition = _originalPosition - new Vector3(0f, _landingOffset, 0f);

            float duration1 = 0.2f;
            float duration2 = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration1)
            {
                transform.localPosition = -Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration1);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            targetPosition = Vector3.zero;
            _originalPosition = transform.localPosition;

            while (elapsedTime < duration2 && _isJump == false)
            {
                transform.localPosition = Vector3.Lerp(_originalPosition, targetPosition, elapsedTime / duration2);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (!_isJump)
            {
                transform.localPosition = Vector3.zero;
                _originalPosition = Vector3.zero;
            }

            _isLand = false;
        }
    }

    /*public IEnumerator CheckDelay()
    {
        float a;
        float b;
        yield return new WaitForSeconds(1f);
        do
        {
            a = transform.localPosition.y;
            yield return new WaitForSeconds(0.001f);
            b = transform.localPosition.y;

        } while (b != 0f && a <= b);

        if (_isJump == false)
            Land();
    }*/


    /*
[SerializeField] private float shakeFrequency = 1f;
[SerializeField] private float shakeAmplitude = 0.1f; 
[SerializeField] private float shakeRotationAmplitude = 10f; 

private Vector3 originalPosition;
private Quaternion originalRotation;

private float currentShakeFrequency = 1f;
private float currentShakeAmplitude = 1f;
private float currentShakeRotation = 1f;

private void Start()
{
    originalPosition = transform.localPosition;
    originalRotation = transform.localRotation;
}

private void Update()
{
    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A))
    {
        Vector3 shakePosition = originalPosition + Random.insideUnitSphere * shakeAmplitude * currentShakeAmplitude;
        Quaternion shakeRotation = Quaternion.Euler(Random.insideUnitSphere * currentShakeRotation);

        transform.localPosition = Vector3.Lerp(transform.localPosition, shakePosition, shakeFrequency * currentShakeFrequency * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation * shakeRotation, shakeFrequency * currentShakeFrequency * Time.deltaTime);

    }
}
*/

}
