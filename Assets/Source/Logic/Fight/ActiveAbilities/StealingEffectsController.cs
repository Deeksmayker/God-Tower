using NTC.Global.Cache;
using UnityEngine;

public class StealingEffectsController : MonoCache
{
    [SerializeField] private GameObject radiusObject;
    [SerializeField] private float startRadius = 20;
    [SerializeField] private float perStealRadiusIncrease = 5;
    [SerializeField] private float decreaseRate = 1;

    [ColorUsage(false, true)]
    [SerializeField] private Color startColor;
    [ColorUsage(false, true)]
    [SerializeField] private Color maxColor;
    [SerializeField] private float radiusForMaxColor = 50;

    [SerializeField] private float addedVerticalVelocityOnSteal = 10;

    private MeshRenderer _radiusObjectMeshRenderer;
    private MaterialPropertyBlock _propertyBlock = new();

    private float _currentRadius;
    private float _desiredRadius;

    private AbilitiesHandler _abilitiesHandler;
    private IMover _mover;

    private void Awake()
    {
        _abilitiesHandler = Get<AbilitiesHandler>();
        _mover = Get<IMover>();
        _currentRadius = startRadius;
        _desiredRadius = _currentRadius;

        _radiusObjectMeshRenderer = radiusObject.GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
        _radiusObjectMeshRenderer.GetPropertyBlock(_propertyBlock);

        SetRadius(startRadius);
    }

    protected override void OnEnabled()
    {
        _abilitiesHandler.OnNewAbility += HandleKillEnemy;
    }

    protected override void OnDisabled()
    {
        _abilitiesHandler.OnNewAbility -= HandleKillEnemy;
    }

    protected override void Run()
    {
        _desiredRadius -= Time.deltaTime * decreaseRate;
        _desiredRadius = Mathf.Clamp(_desiredRadius, startRadius, 200);

        _currentRadius = Mathf.Lerp(_currentRadius, _desiredRadius, Time.deltaTime * 5);
        
        SetRadius(_currentRadius);

        var radiusProgress = Mathf.Clamp01(Mathf.InverseLerp(startRadius, radiusForMaxColor, _currentRadius));

        _propertyBlock.SetColor("_Color", Color.Lerp(startColor, maxColor, radiusProgress));
        _radiusObjectMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void HandleKillEnemy()
    {
        _desiredRadius += perStealRadiusIncrease;

        if (_mover.GetVelocity().y > addedVerticalVelocityOnSteal)
            _mover.AddVerticalVelocity(addedVerticalVelocityOnSteal);
        else
            _mover.SetVerticalVelocity(addedVerticalVelocityOnSteal);
    }

    private void SetRadius(float newRadius)
    {
        _abilitiesHandler.SetStealRadius(newRadius);
        radiusObject.transform.localScale = 2 * newRadius * Vector3.one;
    }
}