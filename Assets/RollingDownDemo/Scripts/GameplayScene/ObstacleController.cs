using UnityEngine;

namespace RollingDownDemo.Scripts.GameplayScene
{
    /// <summary>
    /// Controller attached to obstacle classes, which enables gravity on collisions
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private LayerMask _mask;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _mask = LayerMask.NameToLayer("Obstacles") | LayerMask.NameToLayer("Player");
        }

        private void OnCollisionEnter(Collision other)
        {
            if ((other.gameObject.layer & _mask) > 0)
            {
                // enable gravity when collision with either another obstacle or player has been detected
                _rigidbody.useGravity = true;
            }
        }
    }
}