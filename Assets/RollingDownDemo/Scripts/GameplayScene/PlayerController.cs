using System;
using UnityEngine;

namespace RollingDownDemo.Scripts.GameplayScene
{
    /// <summary>
    /// Player controller, behaviour which lets user control the player object
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Player-following camera, which will be rotated based on the direction of the gravity
        /// </summary>
        public Camera playerCamera;
        
        /// <summary>
        /// Scene controller
        /// </summary>
        public GameplaySceneController sceneController;
        
        /// <summary>
        /// Speed of gravity rotation (in degrees)
        /// </summary>
        public float gravityRotationSpeed = 35;
        
        /// <summary>
        /// Force of the jump
        /// </summary>
        public float jumpForce = 100;
        
        private Rigidbody _body;
        // default gravity, will be rotated based on user input
        private Vector3 _defaultGravity = new Vector3(0f, -5f, 0f);
        
        // current angle of gravity rotation (comp. z)
        private float _gravityRotationAngle = 0f;
        
        // whether jump was used, resets on racetrack collisions
        private bool _jumpUsed = false;

        // layers collisions with which resets ability to jump
        private LayerMask _jumpResetLayerMask;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _jumpResetLayerMask = LayerMask.NameToLayer("Racetrack");
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                // rotate gravity CCW
                _gravityRotationAngle -= gravityRotationSpeed * Time.deltaTime;
            } 
            else if (Input.GetKey(KeyCode.D))
            {
                // rotate gravity CW
                _gravityRotationAngle += gravityRotationSpeed * Time.deltaTime;
            }
            else
            {
                // rotate gravity to its neutral position
                if (Mathf.Abs(_gravityRotationAngle) < gravityRotationSpeed * Time.deltaTime)
                {
                    _gravityRotationAngle = 0f;
                } 
                else if (_gravityRotationAngle > 0)
                {
                    _gravityRotationAngle += -gravityRotationSpeed * Time.deltaTime;
                }
                else
                {
                    _gravityRotationAngle += gravityRotationSpeed * Time.deltaTime;
                }
            }
            
            // calculate gravity rotation quat
            _gravityRotationAngle = Mathf.Clamp(_gravityRotationAngle, -60f, 60f);
            var rotation = Quaternion.Euler(0f, 0f, _gravityRotationAngle);
            
            // setup gravity vector and camera rotation
            Physics.gravity = rotation * _defaultGravity;
            playerCamera.transform.rotation = rotation;

            if (!_jumpUsed && Input.GetKeyDown(KeyCode.W))
            {
                // throw player object in the air (if jump is available)
                _body.AddForce(jumpForce * Vector3.up);
                _jumpUsed = true;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if ((other.gameObject.layer & _jumpResetLayerMask) > 0)
            {
                // reset jump bool
                _jumpUsed = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                // proceed to level completion if player hit "Finish" trigger
                sceneController.LevelCompleted();
            }
        }
    }
}