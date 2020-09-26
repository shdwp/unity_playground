using UnityEngine;

namespace RollingDownDemo.Scripts.GameplayScene
{
    /// <summary>
    /// Very simple camera controller following target position
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FollowCameraController : MonoBehaviour
    {
        public Transform targetTransform;
        public float followSpeed = 10f;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            // calculate target object position, ignoring camera rotation
            // @TODO: figure out better approach
            var originalRotation = _camera.transform.rotation;
            _camera.transform.rotation = Quaternion.identity;
            
            var pos = _camera.WorldToViewportPoint(targetTransform.position);
            var targetPos = new Vector3(0.25f, 0.45f, 4f);
            
            _camera.transform.rotation = originalRotation;

            // move camera towards player based on followSpeed
            // @TODO: add some log10 movement smoothing?
            transform.position += (pos - targetPos) * (followSpeed * Time.deltaTime);
        }
    }
}