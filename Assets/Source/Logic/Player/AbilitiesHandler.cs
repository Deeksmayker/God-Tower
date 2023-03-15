using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;


public class AbilitiesHandler : MonoCache
{
    [SerializeField] private LayerMask layersToSteal;
    [SerializeField] private Transform camRotationTarget;
    [SerializeField] private Transform rightHandShootPoint, leftHandShootPoint;
    
    private IActiveAbility _leftAbility, _rightAbility;
    
    private void Awake()
    {
        var ability = GetComponentInChildren<IActiveAbility>();

        if (ability != null)
        {
            _rightAbility = ability;
        }
    }
    
    public void SetNewRightAbility(GameObject newAbilityPrefab)
    {
        SetNewAbility(out _rightAbility, newAbilityPrefab);
    }

    public void SetNewLeftAbility(IActiveAbility newAbility)
    {
        _leftAbility = newAbility;
    }

    public void SetLeftAbilityInput(bool input)
    {
        _leftAbility?.SetInput(input);
    }

    public void SetRightAbilityInput(bool input)
    {
        _rightAbility?.SetInput(input);
    }

    public void SetRightStealInput(bool input)
    {
        if (!input)
            return;

        if (Physics.Raycast(camRotationTarget.position, camRotationTarget.forward, out var hit, 20, layersToSteal))
        {
            if (hit.transform.TryGetComponent<IGiveAbility>(out var giver) && giver.CanGiveAbility())
            {
                SetNewRightAbility(giver.GetAbilityPrefab());
            }
        }
    }

    private void SetNewAbility(out IActiveAbility abilitySide, GameObject newAbilityPrefab)
    {
        if (newAbilityPrefab.GetComponent<IActiveAbility>() == null)
            Debug.LogError("No ability on \"ability\" prefab");
        
        var ability = NightPool.Spawn(newAbilityPrefab);
        ability.transform.SetParent(transform);
        abilitySide = ability.GetComponent<IActiveAbility>();
        abilitySide.SetRotationTarget(camRotationTarget);
        abilitySide.SetShootPoint(rightHandShootPoint);
    }
}
