using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameInputAction _input;
    private Player _player;
    [SerializeField] Laptop _laptop;
    private Vector3 _moveDirection;
    private Vector3 _direction;


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL on Player Manager");
        }

        InitializeInput();
    }

    private void InitializeInput()
    {
        _input = new GameInputAction();
        _input.Player.Enable();
        _input.Player.Action.started += Action_started;
        _input.Player.Escape.performed += Escape_performed;
    }

    private void Escape_performed(InputAction.CallbackContext obj)
    {
        _laptop.LeaveCameras();
    }

    private void Action_started(InputAction.CallbackContext obj)
    {
        _laptop.SwitchCameras();
    }

    void Update()
    {
        _moveDirection = _input.Player.Movement.ReadValue<Vector2>();
        _player.CalcutateMovement(_moveDirection, _direction);
        
    }

    public void DisablePlayerInput()
    {
        _input.Player.Disable();
    }

    public void EnablePlayerInput()
    {
        _input.Player.Enable();
    }

}
