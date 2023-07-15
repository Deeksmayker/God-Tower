using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class LevelEnter : MonoCache
{
    [SerializeField] private bool teleportPlayerOnPosition = true;
    [SerializeField] private Transform playerSpawnPoint;

    [Inject] private PlayerUnit _player;

    private void Awake()
    {
        if (teleportPlayerOnPosition)
            _player.TeleportPlayer(playerSpawnPoint.position);
    }

    /*public void StartLevel()
    {
        _player.HandleLevelStarted();
    }*/
}