using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class StealingEffectsController : MonoCache
{
    [SerializeField] private GameObject radiusObject;
    [SerializeField] private float startRadius = 20;
    [SerializeField] private float maxRadius = 200;

    [ColorUsage(false, true)]
    [SerializeField] private Color startColor;
    [ColorUsage(false, true)]
    [SerializeField] private Color maxColor;
    [SerializeField] private float radiusForMaxColor = 50;

    [SerializeField] private float addedVerticalVelocityOnSteal = 10;

    private MeshRenderer _radiusObjectMeshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    private float _currentRadius;
    private float _desiredRadius;

    private PlayerStyleController _playerStyleController;
    private AbilitiesHandler _abilitiesHandler;
    private IMover _mover;

    private void Awake()
    {
        _abilitiesHandler = Get<AbilitiesHandler>();
        _playerStyleController = Get<PlayerStyleController>();
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
        _desiredRadius = Mathf.Lerp(startRadius, maxRadius, Mathf.Pow(_playerStyleController.GetCurrentStyle01(), 3));

        _currentRadius = Mathf.Lerp(_currentRadius, _desiredRadius, 10 * Time.deltaTime);
        
        SetRadius(_currentRadius);

        var radiusProgress = Mathf.Clamp01(Mathf.InverseLerp(startRadius, radiusForMaxColor, _currentRadius));

        _propertyBlock.SetColor("_Color", Color.Lerp(startColor, maxColor, radiusProgress));
        _radiusObjectMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void HandleKillEnemy()
    {
        if (_mover.GetVelocity().y > addedVerticalVelocityOnSteal)
            _mover.AddVerticalVelocity(addedVerticalVelocityOnSteal);
        else
            _mover.SetVerticalVelocity(addedVerticalVelocityOnSteal);
    }

    private void SetRadius(float newRadius)
    {
        _abilitiesHandler.SetStealRadius(newRadius);
        radiusObject.transform.localScale = 2 * newRadius * new Vector3(1, 100, 1);
    }
}