using System;
using NTC.Global.Cache;
using UnityEngine;

public class RunnerAiController : MonoCache, IAiController
{
    private IAiMeleeAttackController _meleeAttackController;
    private IMeleeAttacker _meleeAttacker;
    private IAiMovementController _movementController;

    private void Awake()
    {
        _meleeAttacker = Get<IMeleeAttacker>();
        _meleeAttackController = Get<IAiMeleeAttackController>();
        _movementController = Get<IAiMovementController>();
    }

    protected override void OnEnabled()
    {
        _meleeAttacker.OnStartPreparingAttack += HandleStartMeleeAttack;
        _meleeAttacker.OnEndAttack += HandleEndMeleeAttack;
    }

    protected override void OnDisabled()
    {
        _meleeAttacker.OnStartPreparingAttack -= HandleStartMeleeAttack;
        _meleeAttacker.OnEndAttack -= HandleEndMeleeAttack;
    }
    
    private void HandleStartMeleeAttack()
    {
        _movementController.Stop();
    }

    private void HandleEndMeleeAttack()
    {
        _movementController.ResumeMoving();
    }
}
