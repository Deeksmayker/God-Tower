using NTC.Global.Cache;
using UnityEngine;

public class TeleporterOnTrigger : MonoCache
{
    [SerializeField] private Transform target;

    private void OnTriggerEnter(Collider other)
    {
        var offset = other.transform.position - transform.position;
        other.GetComponent<CharacterController>().enabled = false;
        other.transform.position = target.position + offset;
        other.GetComponent<CharacterController>().enabled = true;
    }
}