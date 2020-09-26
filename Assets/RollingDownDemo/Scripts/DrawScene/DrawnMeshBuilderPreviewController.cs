using System.Runtime.InteropServices;
using UnityEngine;

namespace RollingDownDemo.Scripts.DrawScene
{
    /// <summary>
    /// Controller for mesh preview object which displays drawn mesh
    /// </summary>
    [RequireComponent(typeof(Light))]
    [RequireComponent(typeof(MeshFilter))]
    public class DrawnMeshBuilderPreviewController: MonoBehaviour
    {
        // builder (shared with UserDrawingController)
        public DrawnMeshBuilder builder;
        private MeshFilter _meshFilter;
        
        // light which intensity will be based on number of vertices in builder
        private Light _light;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _light = GetComponent<Light>();
        }

        private void Start()
        {
            // disable light since user didn't draw anything yet
            _light.enabled = false;
        }

        private void Update()
        {
            if (builder != null)
            {
                // set mesh to filter
                _meshFilter.sharedMesh = builder.mesh;

                if (builder.vertCount > 0)
                {
                    // enable light and set its intensity
                    _light.enabled = true;
                    _light.intensity = builder.vertCount / 10f;
                }
            }
        }
    }
}