﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.LiveObjects;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        private GameInputAction _input;
        private CharacterController _controller;
        private Animator _anim;
        [SerializeField]
        private float _speed = 5.0f;
        private bool _playerGrounded;
        [SerializeField]
        private Detonator _detonator;
        private bool _canMove = true;
        [SerializeField]
        private CinemachineVirtualCamera _followCam;
        [SerializeField]
        private GameObject _model;
        [SerializeField] private float _rotateSpeed = 20f;
        private Vector3 _moveDirection;
        private Vector3 _direction;
        



        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete += ReleasePlayerControl;
            Laptop.onHackEnded += ReturnPlayerControl;
            Forklift.onDriveModeEntered += ReleasePlayerControl;
            Forklift.onDriveModeExited += ReturnPlayerControl;
            Forklift.onDriveModeEntered += HidePlayer;
            Drone.OnEnterFlightMode += ReleasePlayerControl;
            Drone.onExitFlightmode += ReturnPlayerControl;
        } 

        private void Start()
        {
            _input = new GameInputAction();
            _input.Player.Enable();
            _controller = GetComponent<CharacterController>();

            if (_controller == null)
                Debug.LogError("No Character Controller Present");

            _anim = GetComponentInChildren<Animator>();

            if (_anim == null)
                Debug.Log("Failed to connect the Animator");

            
        }

        private void Update()
        {
            if (_canMove == true)
                CalcutateMovement();

        }

        private void CalcutateMovement()
        {
            _playerGrounded = _controller.isGrounded;
            //float h = Input.GetAxisRaw("Horizontal");
            //float v = Input.GetAxisRaw("Vertical");
            _moveDirection = _input.Player.Movement.ReadValue<Vector2>();
            Debug.Log($"Movement value X: {_moveDirection.x}, Movement value Y: {_moveDirection.y}");


            //transform.Rotate(transform.up, h);
            transform.Rotate(transform.up, _moveDirection.x * _rotateSpeed * Time.deltaTime);

            //var direction = transform.forward * v;
            Debug.Log(transform.forward);
            _direction = transform.forward * _moveDirection.y;
            var velocity = _direction * _speed;


            _anim.SetFloat("Speed", Mathf.Abs(velocity.magnitude));


            if (_playerGrounded)
                velocity.y = 0f;
            if (!_playerGrounded)
            {
                velocity.y += -20f * Time.deltaTime;
            }
            
            _controller.Move(velocity * Time.deltaTime);                      

        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            switch(zone.GetZoneID())
            {
                case 1: //place c4
                    _detonator.Show();
                    break;
                case 2: //Trigger Explosion
                    TriggerExplosive();
                    break;
            }
        }

        private void ReleasePlayerControl()
        {
            _canMove = false;
            _followCam.Priority = 9;
        }

        private void ReturnPlayerControl()
        {
            _model.SetActive(true);
            _canMove = true;
            _followCam.Priority = 10;
        }

        private void HidePlayer()
        {
            _model.SetActive(false);
        }
               
        private void TriggerExplosive()
        {
            _detonator.TriggerExplosion();
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete -= ReleasePlayerControl;
            Laptop.onHackEnded -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= ReleasePlayerControl;
            Forklift.onDriveModeExited -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= HidePlayer;
            Drone.OnEnterFlightMode -= ReleasePlayerControl;
            Drone.onExitFlightmode -= ReturnPlayerControl;
        }

    }
}

