using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerUnit))]
public class PlayerInputHandler : MonoCache
{
    [SerializeField] private Transform lookRotationTransform;
    [SerializeField] private float jumpContinueInput = 0.1f;

    private float _jumpInputTimer;

    private bool _responseToInput = true;
    private bool _canShoot = true;
    
    private PlayerMovementController _mover;
    private IJumper _jumper;
    private IMeleeAttacker _meleeAttacker;
    private AbilitiesHandler _abilitiesHandler;
    private AirSlamer _slamer;
    private GroundSlider _slider;
    private Hook _hook;
    //private PickUpDetector _pickUpDetector;
    //private Thrower _thrower;
    
    private PlayerInput _playerInput;

    private void Awake()
    {
        _mover = Get<PlayerMovementController>();
        _playerInput = Get<PlayerInput>();
        _jumper = Get<IJumper>();
        _meleeAttacker = Get<IMeleeAttacker>();
        _abilitiesHandler = Get<AbilitiesHandler>();
        _slamer = Get<AirSlamer>();
        _slider = Get<GroundSlider>();
        _hook = Get<Hook>();
        //_pickUpDetector = GetComponentInChildren<PickUpDetector>();
        //_thrower = GetComponentInChildren<Thrower>();
    }

    protected override void Run()
    {
        if (!_responseToInput)
            return;

        if (_mover != null)
        {
            _mover.SetHorizontalInput(_playerInput.actions["Move"].ReadValue<Vector2>());

            _mover.SetDashInput(_playerInput.actions["Dash"].IsInProgress());
        }

        if (_jumper != null)
        {
            var jumpInput = _playerInput.actions["Jump"].WasPressedThisFrame();
            if (jumpInput)
                _jumpInputTimer = jumpContinueInput;

            var needToJump = jumpInput || _jumpInputTimer > 0;

            _jumper.SetJumpInput(needToJump);

            _jumpInputTimer -= Time.deltaTime;
        }

        if (_meleeAttacker != null)
        {
            _meleeAttacker.SetInput(_playerInput.actions["Kick"].WasPressedThisFrame());
        }

        if (_abilitiesHandler != null)
        {
            _abilitiesHandler.SetRightStealInput(_playerInput.actions["RightSteal"].WasPressedThisFrame());
            _abilitiesHandler.SetLeftStealInput(_playerInput.actions["LeftSteal"].WasPressedThisFrame());

            if (_canShoot)
            {
                _abilitiesHandler.SetRightAbilityInput(_playerInput.actions["RightAttack"].IsInProgress());
                _abilitiesHandler.SetLeftAbilityInput(_playerInput.actions["LeftAttack"].IsInProgress());
            }
        }

        if (_slamer != null)
        {
            _slamer.SetInput(_playerInput.actions["Slam"].WasPressedThisFrame());
        }

        if (_slider != null)
        {
            _slider.SetInput(_playerInput.actions["Slide"].IsInProgress());
        }

        if (_hook)
        {
            _hook.SetInput(_playerInput.actions["Hook"].WasPressedThisFrame());
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
        
    public void DisableInputResponse()
    {
        _responseToInput = false;
    }

    public void EnableInputResponse()
    {
        _responseToInput = true;
    }

    public void SetShootInputResponse(bool canShoot)
    {
        _canShoot = canShoot;
    }
}
