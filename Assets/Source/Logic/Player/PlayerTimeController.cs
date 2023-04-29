using System;
using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class PlayerTimeController : MonoCache
{
    [SerializeField] private float timeStopOnKickHit = 0.05f;
    [SerializeField] private float timeStopOnParry = 0.1f;
    
    [Inject] private TimeController _timeController;

    private Kicker _kicker;

    private void Awake()
    {
        _kicker = Get<Kicker>();
    }

    protected override void OnEnabled()
    {
        _kicker.OnParry += HandleParry;
        _kicker.OnHit += HandleKickHit;
    }
    
    protected override void OnDisabled()
    {
        _kicker.OnParry -= HandleParry;
        _kicker.OnHit -= HandleKickHit;
    }

    private void HandleParry()
    {
        _timeController.SetTimeScale(0, timeStopOnParry);
    }

    private void HandleKickHit()
    {
        _timeController.SetTimeScale(0, timeStopOnKickHit);
    }
}
