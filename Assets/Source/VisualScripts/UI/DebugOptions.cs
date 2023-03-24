using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;


public class DebugOptions : MonoCache
{
    [Inject] private PlayerInput _input;

    protected override void Run()
    {
        if (_input.actions["Restart"].WasPressedThisFrame())
        {
            SceneManager.LoadScene(0);
        }
    }
}
