using System;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    /// <summary>
    /// Controller for debris created as a result of mesh cutting. Sets up required components to render the
    /// object, also destroys it when it gets out of the screen.
    /// </summary>
    public class DebrisController : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        /// <summary>
        /// Setup the debris object
        /// </summary>
        /// <param name="world">world transform, will be set as parent</param>
        /// <param name="substitute">object that was previously cut (new object will inherit its position, rotation and scale)</param>
        /// <param name="mesh">mesh of debris</param>
        /// <param name="mat">material to use</param>
        /// <param name="force">force to apply to rigidbody after object has been setup</param>
        public void InitialSetup(Transform world, Transform substitute, Mesh mesh, Material mat, Vector3 force)
        {
            gameObject.transform.SetParent(world, false);
            
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = mat;
            gameObject.AddComponent<Rigidbody>().AddForce(force);
            gameObject.AddComponent<MeshCollider>().convex = true;

            var scale = substitute.transform.localScale;
            var rotation = substitute.transform.localRotation;
            var position = substitute.transform.localPosition;

            gameObject.transform.localPosition = position;
            gameObject.transform.localScale = scale;
            gameObject.transform.localRotation = rotation;
        }

        private void Update()
        {
            var viewportPos = _camera.WorldToViewportPoint(transform.position);
            
            // @TODO: take mesh bounds into account when calculating position
            if (viewportPos.y < -1f)
            {
                Destroy(gameObject);
            }
        }
    }
}