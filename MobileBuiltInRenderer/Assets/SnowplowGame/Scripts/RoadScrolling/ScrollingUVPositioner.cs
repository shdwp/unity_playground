using System;
using UnityEngine;

namespace SnowplowGame.Scripts.RoadScrolling
{
    /// <summary>
    /// Class that manipulates UV in the way to make object appear as moving with constant speed.
    /// Only provides UV-space conversions and shader property update, actual movement is done in the shader.
    /// </summary>
    public class ScrollingUVPositioner : MonoBehaviour
    {
        /// <summary>
        /// Target object
        /// </summary>
        public GameObject scrollingObject;
        
        [NonSerialized]
        public float uvDistance = 0f;
        
        // half width of target mesh bounds
        private float _scrollingObjectHalfWidth;
        
        private Material _mat;
        private readonly int _sidUVDistance = Shader.PropertyToID("_UVDistance");
        
        private void Awake()
        {
            _mat = scrollingObject.GetComponent<Renderer>().sharedMaterial;
            _scrollingObjectHalfWidth = scrollingObject.GetComponent<MeshFilter>().mesh.bounds.size.x / 2f;
        }

        /// <summary>
        /// Advance by distance. Will update value in shader property
        /// </summary>
        /// <param name="dist"></param>
        public void Advance(float dist)
        {
            uvDistance += dist / _scrollingObjectHalfWidth * Time.deltaTime;

            _mat.SetFloat(_sidUVDistance, uvDistance);
        }
    }
}