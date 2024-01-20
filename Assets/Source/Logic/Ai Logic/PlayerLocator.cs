using NTC.Global.Cache;
using UnityEngine;

public class PlayerLocator : MonoCache
{
    public static PlayerLocator Instance;

    [SerializeField] private float locatePlayerRange;

    private PlayerTargetPoint _targetPoint;

    private void Awake()
    {
        if (!Instance){
            Instance = this;
        }
    
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

	public Vector3 GetPlayerPos()
	{
		return _targetPoint.transform.position;
	}

    public Vector3 GetVectorFromPoint(Vector3 point){
        return _targetPoint.transform.position - point;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, locatePlayerRange);
    }
}
