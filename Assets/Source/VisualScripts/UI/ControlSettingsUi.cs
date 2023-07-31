using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSettingsUi : MonoBehaviour
{
    [SerializeField] private GameObject waitingInputPanel;
    [SerializeField] private ControlRebindingButton[] controlRebindingButtons;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        
        _playerInput.actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Rebinds"));

        for (var i = 0; i < controlRebindingButtons.Length; i++)
        {
            controlRebindingButtons[i].OnStartRebinding.AddListener(()=>waitingInputPanel.SetActive(true));
            controlRebindingButtons[i].OnEndRebinding.AddListener(()=>waitingInputPanel.SetActive(false));

            controlRebindingButtons[i].OnStartRebinding.AddListener(HandleRebindStarted);
            controlRebindingButtons[i].OnEndRebinding.AddListener(HandleRebindEnded);
        }
    }

    private void HandleRebindStarted()
    {
        _playerInput.enabled = false;
    }

    private void HandleRebindEnded()
    {
        _playerInput.enabled = true;
        PlayerPrefs.SetString("Rebinds", _playerInput.actions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }
}
