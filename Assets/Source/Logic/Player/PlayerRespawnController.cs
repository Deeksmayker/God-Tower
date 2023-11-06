using NTC.Global.Cache;
using UnityEngine;

public class PlayerRespawnController : MonoCache
{
    private Transform _currenRespawnTransform;

    public void Respawn()
    {
        Get<CharacterController>().enabled = false;
        transform.position = (_currenRespawnTransform.position);
        Get<CharacterController>().enabled = true;
        Get<IMover>().SetVelocity(Vector3.zero);
    }

    public void SetRespawnPoint(Transform transform)
    {
        _currenRespawnTransform = transform;
    }
}