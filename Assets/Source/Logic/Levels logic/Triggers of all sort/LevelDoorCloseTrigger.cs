using NTC.Global.Cache;
using UnityEngine;

public class LevelDoorCloseTrigger : MonoCache
{
    [SerializeField] private GameObject[] objectsToDisable;

    private RoomDoor _door;

    private void Awake()
    {
        _door = GetComponentInParent<RoomDoor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out var playerUnit))
        {
            _door.Close();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out var playerUnit))
        {
            for (var i = 0; i < objectsToDisable.Length; i++)
            {
                if (objectsToDisable[i])
                {
                    objectsToDisable[i].SetActive(false);
                }
            }
        }
    }
}