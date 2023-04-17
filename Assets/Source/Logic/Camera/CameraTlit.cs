using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTlit : MonoBehaviour
{


    private void Awake()
    {
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


    public float tiltSpeed = 1.0f; // скорость изменения угла наклона камеры
    public float maxTiltAngle = 30.0f; // максимальный угол наклона камеры
    public float minTiltAngle = -30.0f; // минимальный угол наклона камеры


    private float currentTiltAngle = 0.0f;

    void Update()
    {
        float verticalMovement = Input.GetAxis("Vertical");
        currentTiltAngle = Mathf.Clamp(currentTiltAngle - verticalMovement * tiltSpeed, minTiltAngle, maxTiltAngle);
    }
}
