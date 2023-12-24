using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Source.Logic.Utils.DebugConsoleCommands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class DebugConsoleController : MonoBehaviour
{
    private const float LINE_HEIGHT = 15f;

    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private PlayerStatisticViewObserver playerStatisticViewObserver;

    private List<DebugCommand> _debugCommands;

    private bool _isConsoleShowed;
    private string _input;

    private float _height = 0;
    
    private string _helpText;
    private bool _isHelpInvoked;
    private event Action HelpInvoked; 

    private void Start()
    {
        playerInputHandler.DebugButtonClicked += OnDebugToggle;

        InitializeCommands();
    }

    private void Update()
    {
        if (!_isConsoleShowed) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var commandName = string.Copy(_input);

            _input = string.Empty;
            GetDebugCommand(commandName).Execute();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (_input.Length >= 1)
                _input = _input.Remove(_input.Length - 1);
        }

        if (Input.inputString == string.Empty
            || Input.inputString == "\b"
            || Input.GetKey(KeyCode.BackQuote)
            || Input.GetKey(KeyCode.Space)
            || Input.GetKey(KeyCode.Backspace)
            || Input.GetKey(KeyCode.Return)) return;

        _input += Input.inputString;
    }

    private void OnGUI()
    {
        if (!_isConsoleShowed) return;

        GUI.Box(new Rect(0, 0, Screen.width, _height + 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        _input = GUI.TextField(new Rect(10f, 5f, Screen.width - 20f, _height + 20f), _input);
        
        if (_isHelpInvoked)
            HelpInvoked?.Invoke();
    }

    public void OnDebugToggle()
    {
        _isConsoleShowed = !_isConsoleShowed;

        playerInputHandler.SetActiveMoved(!_isConsoleShowed);

        _input = string.Empty;
    }

    private DebugCommand GetDebugCommand(string commandName)
    {
        for (int i = 0; i < _debugCommands.Count; i++)
        {
            if (_debugCommands[i].GetCommandName() == commandName)
            {
                return _debugCommands[i];
            }
        }

        throw new ArgumentException($"No commands with name: {commandName}");
    }

    private void InitializeCommands()
    {
        _debugCommands = new List<DebugCommand>()
        {
            new("help","Show all commands", () =>
            {
                if (_isHelpInvoked) return;
                
                _height = 20 * _debugCommands.Count - 1;

                HelpInvoked += () =>
                {
                    GUI.TextArea(new Rect(10f, 25f, Screen.width - 20f, _height + 20f), GetHelpText());
                };
                
                _isHelpInvoked = true;
            }),
            new("stat", "Enable statistic panel", () =>
            {
                if (playerStatisticViewObserver.Showed())
                    playerStatisticViewObserver.HidePanel();
                else
                    playerStatisticViewObserver.ShowPanel();
            }),
        };
    }

    private string GetHelpText()
    {
        return _debugCommands.Aggregate(string.Empty, 
            (current, debugCommand) => current + $"'{debugCommand.GetCommandName()}' - {debugCommand.GetDescription()}\n");
    }
}