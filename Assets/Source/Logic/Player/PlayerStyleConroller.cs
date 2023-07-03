using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class PlayerStyleController : MonoCache
{
    [SerializeField] private float styleReduceRate;
    [SerializeField] private float stylePerKill;

    [Header("Components")]
    [SerializeField] private float addedDashDuration;

    [Header("Fullscreen Effect")]
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private float maxAlpha = 5;
    [SerializeField] private float minSpeed, maxSpeed;

    [SerializeField] private AudioSource styleMeterAudioSource;
    [SerializeField] private AudioClip styleFullOneShotClip;

    private float _currentStyle;

    [Inject] private PostProcessingController _postProcessingController;

    private AbilitiesHandler _abilitiesHandler;
    private PlayerMovementController _mover;

    private void Awake()
    {
        _abilitiesHandler = Get<AbilitiesHandler>();
        _mover = Get<PlayerMovementController>();
    }

    protected override void OnEnabled()
    {
        _abilitiesHandler.OnStealAbility += HandleEnemyKill;
    }

    protected override void OnDisabled()
    {
        _abilitiesHandler.OnStealAbility -= HandleEnemyKill;

        _currentStyle = 0;

        _postProcessingController.SetMotionBlurIntensity(Mathf.Pow(_currentStyle, 3));

        fullscreenMaterial.SetFloat("_Alpha", Mathf.Lerp(0, maxAlpha, Mathf.Pow(_currentStyle, 3)));
        fullscreenMaterial.SetFloat("_Speed", Mathf.Lerp(minSpeed, maxSpeed, Mathf.Pow(_currentStyle, 3)));
    }

    protected override void Run()
    {
        var removedStyle = _mover.IsGrounded() ? styleReduceRate : styleReduceRate / 2;

        _currentStyle -= removedStyle * Time.deltaTime;

        _currentStyle = Mathf.Clamp01(_currentStyle);

        _mover.SetDashDuration(Mathf.Lerp(_mover.GetBaseDashDuration(), _mover.GetBaseDashDuration() + addedDashDuration, _currentStyle));

        _postProcessingController.SetMotionBlurIntensity(Mathf.Pow(_currentStyle, 3));
        fullscreenMaterial.SetFloat("_Alpha", Mathf.Lerp(0, maxAlpha, Mathf.Pow(_currentStyle, 3)));
        fullscreenMaterial.SetFloat("_Speed", Mathf.Lerp(minSpeed, maxSpeed, Mathf.Pow(_currentStyle, 3)));

        ControlStyleMeterAudio();
    }

    public void HandleEnemyKill()
    {
        _currentStyle += stylePerKill;
    }

    public void SetStyleToMax()
    {
        if (_currentStyle < 0.8f)
        {
            styleMeterAudioSource.PlayOneShot(styleFullOneShotClip);
        }

        _currentStyle = 1;
    }

    public float GetCurrentStyle01()
    {
        return _currentStyle;
    }

    private void ControlStyleMeterAudio()
    {
        styleMeterAudioSource.volume = GetCurrentStyle01() * GetCurrentStyle01();
        styleMeterAudioSource.pitch = Mathf.Sqrt(GetCurrentStyle01());
    }
}