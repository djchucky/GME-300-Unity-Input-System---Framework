using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;

        //inputs
        [SerializeField] PlayerManager _playerManager;
        private GameInputAction _input;
        private Vector3 _movement;
        private Vector3 _rotateInput;
        private float _yaw;
        private float _thrusting;
        private float _decreasing;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void Start()
        {
            _input = new GameInputAction();
            _input.Drone.Escape.performed += Escape_performed;
      
        }

        private void Escape_performed(InputAction.CallbackContext obj)
        {
             if (_inFlightMode && _input.Drone.enabled == true)
            {
                _inFlightMode = false;
                onExitFlightmode?.Invoke();
                ExitFlightMode();
            }
            
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                //swap input map
                _playerManager.DisablePlayerInput();
                _input.Drone.Enable();
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);
            _input.Drone.Disable();
            _playerManager.EnablePlayerInput();
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();

               /* if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode();
                }*/
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            /*if (Input.GetKey(KeyCode.LeftArrow))
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }


            if (Input.GetKey(KeyCode.RightArrow))
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }*/

            _rotateInput = _input.Drone.Rotate.ReadValue<Vector2>();
            _yaw = transform.localEulerAngles.y;
            

            if(_rotateInput.x < 0)
            {
                _yaw -= _speed / 3;           
            }
            else if (_rotateInput.x > 0)
            {
                _yaw += _speed / 3;
            }

            Vector3 currentEuler = transform.localEulerAngles;
            transform.localRotation = Quaternion.Euler(currentEuler.x, _yaw, currentEuler.z);
        }

        private void CalculateMovementFixedUpdate()
        {
            _thrusting = _input.Drone.Thrust.ReadValue<float>();
            _decreasing = _input.Drone.NotThrusting.ReadValue<float>();
            //if (Input.GetKey(KeyCode.Space))
            if (_thrusting > 0)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            //if (Input.GetKey(KeyCode.V))
            if(_decreasing > 0)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        private void CalculateTilt()
        {
            _movement = _input.Drone.Movement.ReadValue<Vector2>();
            //if (Input.GetKey(KeyCode.A)) 
              if (_movement.x < 0)
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            //else if (Input.GetKey(KeyCode.D))
              else if (_movement.x > 0)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            //else if (Input.GetKey(KeyCode.W))
            else if (_movement.y > 0)
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);            
            //else if (Input.GetKey(KeyCode.S))
            else if (_movement.y < 0)
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            else 
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
