using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerInputHandler : MonoCache
{
    [SerializeField] private Transform lookRotationTransform;
    
    private IMover _mover;
    private IJumper _jumper;
    private IMeleeAttacker _meleeAttacker;
    private IActiveAbility _activeAbility;
    //private PickUpDetector _pickUpDetector;
    //private Thrower _thrower;
    
    private PlayerInput _playerInput;

    private void Awake()
    {
        _mover = Get<DefaultMover>();
        _playerInput = Get<PlayerInput>();
        _jumper = Get<IJumper>();
        _meleeAttacker = Get<IMeleeAttacker>();
        _activeAbility = GetComponentInChildren<IActiveAbility>();
        //_pickUpDetector = GetComponentInChildren<PickUpDetector>();
        //_thrower = GetComponentInChildren<Thrower>();
    }

    protected override void Run()
    {
        if (_mover != null)
        {
            _mover.SetHorizontalInput(_playerInput.actions["Move"].ReadValue<Vector2>());
        }

        if (_jumper != null)
        {
            _jumper.SetJumpInput(_playerInput.actions["Jump"].WasPressedThisFrame());
        }

        if (_meleeAttacker != null)
        {
            _meleeAttacker.SetInput(_playerInput.actions["Kick"].WasPressedThisFrame());
        }
        
        _activeAbility.SetInput(_playerInput.actions["RightAttack"].IsInProgress());
        
        /*if (_pickUpDetector != null)
        {
            if (_playerInput.actions["PickUp"].WasPressedThisFrame())
                _pickUpDetector.TryPickUp();
        }*/

        /*if (_thrower != null)
        {
            if (_playerInput.actions["Throw"].WasPressedThisFrame())
            {
                _thrower.TryToThrow(lookRotationTransform.forward);
            }
        }*/
    }
        
}
