using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class WaterGridTeleporter : MonoCache
{
    [SerializeField] private float gridSize = 50;

    [Inject] private PlayerUnit _player;

    protected override void Run()
    {
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(_player.transform.position.x)) >= gridSize)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(Mathf.Abs(transform.position.z) - Mathf.Abs(_player.transform.position.z)) >= gridSize)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _player.transform.position.z);
        }
    }
}