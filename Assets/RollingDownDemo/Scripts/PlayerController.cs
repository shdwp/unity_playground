using System;
using UnityEngine;

namespace RollingDownDemo.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _body;
        public float LateralForce;

        private void Start()
        {
            _body = GetComponent<Rigidbody>();
            Physics.gravity = new Vector3(0f, -5f, 0f);
        }

        private void Update()
        {
            _body.AddForce(new Vector3(LateralForce * Time.deltaTime, 0f, 0f));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _body.AddForce(new Vector3(0f, LateralForce, 0f));
            }
            // _body.AddTorque(new Vector3(LateralForce * Time.deltaTime, 0f, 0f));
        }
    }
}