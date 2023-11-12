using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector3 _openedPosition;
    [SerializeField] private float _openDuration;

    public void Open()
    {
        transform.DOMove(_openedPosition, _openDuration);
    }
}
