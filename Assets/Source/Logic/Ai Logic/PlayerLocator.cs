using NTC.Global.Cache;
using UnityEngine;

public class PlayerLocator : MonoCache
{
    [SerializeField] private float locatePlayerRange;

    private PlayerTargetPoint _targetPoint;

    private void Awake()
    {
        _targetPoint = FindObjectOfType<PlayerTargetPoint>();
    }

    public bool IsPlayerVisible()
    {
        if (Physics.Raycast(transform.position, GetDirectionToPlayerNorm(), out var hit, locatePlayerRange, Layers.PlayerBase | Layers.Environment))
        {
            Log("see something but is it player - " + (hit.transform.GetComponent<PlayerUnit>() != null));
            return hit.transform.GetComponent<PlayerUnit>();
        }
        return false;
    }

    public Vector3 GetDirectionToPlayerNorm()
    {
        var direction = (_targetPoint.transform.position - transform.position).normalized;

        DrawLine(transform.position, transform.position + direction * locatePlayerRange, 2);

        return direction;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, locatePlayerRange);
    }
}