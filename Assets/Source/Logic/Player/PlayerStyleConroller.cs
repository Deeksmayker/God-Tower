using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class PlayerStyleController : MonoCache
{
    [SerializeField] private float styleReduceRate;
    [SerializeField] private float stylePerKill;

    [Header("Fullscreen Effect")]
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private float maxAlpha = 5;
    [SerializeField] private float minSpeed, maxSpeed;


    private float _currentStyle;

    [Inject] private PostProcessingController _postProcessingController;

    private AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _abilitiesHandler = Get<AbilitiesHandler>();
    }

    protected override void OnEnabled()
    {
        _abilitiesHandler.OnNewAbility += HandleEnemyKill;
    }

    protected override void OnDisabled()
    {
        _abilitiesHandler.OnNewAbility -= HandleEnemyKill;
    }

    protected override void Run()
    {
        _currentStyle -= styleReduceRate * Time.deltaTime;

        _currentStyle = Mathf.Clamp01(_currentStyle);

        _postProcessingController.SetMotionBlurIntensity(Mathf.Pow(_currentStyle, 3));

        fullscreenMaterial.SetFloat("_Alpha", Mathf.Lerp(0, maxAlpha, Mathf.Pow(_currentStyle, 3)));
        fullscreenMaterial.SetFloat("_Speed", Mathf.Lerp(minSpeed, maxSpeed, Mathf.Pow(_currentStyle, 3)));
    }

    public void HandleEnemyKill()
    {
        _currentStyle += stylePerKill;
    }

    public void SetStyleToMax()
    {
        _currentStyle = 1;
    }

    public float GetCurrentStyle01()
    {
        return _currentStyle;
    }
}