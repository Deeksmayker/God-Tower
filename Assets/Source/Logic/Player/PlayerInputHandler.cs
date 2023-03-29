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
    private AbilitiesHandler _abilitiesHandler;
    private AirSlamer _slamer;
    private GroundSlider _slider;
    //private PickUpDetector _pickUpDetector;
    //private Thrower _thrower;
    
    private PlayerInput _playerInput;

    private void Awake()
    {
        _mover = Get<DefaultMover>();
        _playerInput = Get<PlayerInput>();
        _jumper = Get<IJumper>();
        _meleeAttacker = Get<IMeleeAttacker>();
        _abilitiesHandler = Get<AbilitiesHandler>();
        _slamer = Get<AirSlamer>();
        _slider = Get<GroundSlider>();
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

        if (_abilitiesHandler != null)
        {
            _abilitiesHandler.SetRightStealInput(_playerInput.actions["RightSteal"].WasPressedThisFrame());
            _abilitiesHandler.SetRightAbilityInput(_playerInput.actions["RightAttack"].IsInProgress());
            _abilitiesHandler.SetLeftStealInput(_playerInput.actions["LeftSteal"].WasPressedThisFrame());
            _abilitiesHandler.SetLeftAbilityInput(_playerInput.actions["LeftAttack"].IsInProgress());
        }

        if (_slamer != null)
        {
            _slamer.SetInput(_playerInput.actions["Slam"].WasPressedThisFrame());
        }

        if (_slider != null)
        {
            _slider.SetInput(_playerInput.actions["Slide"].IsInProgress());
        }
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
