using System;
using UnityEngine;

namespace RollingDownDemo.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class DrawnMeshBuilderPreviewController: MonoBehaviour
    {
        public DrawnMeshBuilder builder;
        private MeshFilter _meshFilter;

        private bool _x;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            if (builder != null)
            {
                if (!_x)
                {
                    // builder.AddSegment(new Vector2(0f, 0f), new Vector2(1f, 0f),  0.1f);
                    // builder.AddContinuationSegment(new Vector2(1.5f, 1f), 0.1f);
                    //builder.AddContinuationSegment(new Vector2(1f, 2f), 0.2f);
                    //builder.AddContinuationSegment(new Vector2(2f, 3f), 0.05f);
                    // builder.AddSegment(new Vector2(0f, 0f), new Vector2(0.5f, 0.1f),  0.1f);
                    _x = true;
                }
                
                _meshFilter.mesh = builder.mesh;
            }
        }
    }
}