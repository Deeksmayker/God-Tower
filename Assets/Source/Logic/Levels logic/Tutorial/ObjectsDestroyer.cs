using NTC.Global.Cache;
using System.Linq;
using UnityEngine;

public class ObjectsDestroyer : MonoCache
{
    [SerializeField] private LayerMask projectilesLayers;

    public void DestroyHomings()
    {
        var homings = Physics.OverlapSphere(transform.position, 1000, projectilesLayers);

        for (var i = 0; i < homings.Length; i++)
        {
            if (homings[i].TryGetComponent<BaseHomingObject>(out var homing))
            {
                Destroy(homing.gameObject);
            }
        }
    }
}