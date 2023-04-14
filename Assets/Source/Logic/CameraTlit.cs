using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTlit : MonoBehaviour
{
    public CinemachineVirtualCamera _virtualCamera;
    public CinemachineComposer _composer;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _composer = _virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    }

    public void MakeTiltLeft(float levelOfTilte) // Z
    {
    }

    public void MakeTiltRight(float levelOfTilte) // Z
    {

    }

    public void MakeTiltBack(float levelOfTilte) // X
    {

    }


    public float tiltSpeed = 1.0f; // �������� ��������� ���� ������� ������
    public float maxTiltAngle = 30.0f; // ������������ ���� ������� ������
    public float minTiltAngle = -30.0f; // ����������� ���� ������� ������


    private float currentTiltAngle = 0.0f;

    void Update()
    {
        float verticalMovement = Input.GetAxis("Vertical");
        currentTiltAngle = Mathf.Clamp(currentTiltAngle - verticalMovement * tiltSpeed, minTiltAngle, maxTiltAngle);
        _composer.m_TrackedObjectOffset.y = currentTiltAngle;
    }
}
