using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class SceneTunnel : MonoBehaviour
{
    [SerializeField] private int definitionNextLevel = 1;
    private bool _tunnelActivated;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && !_tunnelActivated) // Player 
        {
            _tunnelActivated = true;
            GameManager.Instance.LoadSceneByDefinition(definitionNextLevel);
        }
    }
}
