using System;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    public class DebrisController : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void InitialSetup(Transform world, Transform substitute, Mesh mesh, Material mat, Vector3 force)
        {
            gameObject.transform.SetParent(world, false);
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = mat;
            gameObject.AddComponent<Rigidbody>().AddForce(force);

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
            
            // @TODO: include mesh bounds
            if (viewportPos.y < -1f)
            {
                Destroy(gameObject);
            }
        }
    }
}