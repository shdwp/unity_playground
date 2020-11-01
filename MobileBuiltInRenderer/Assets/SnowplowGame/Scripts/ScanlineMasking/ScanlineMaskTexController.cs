using System;
using System.Linq;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace SnowplowGame.Scripts
{
    /// <summary>
    /// Controller that updates masked area. Used in conjunction with ScrollingUVPositioner.
    /// Will set _ClearMask shader property to newly created texture, which will then be painted following
    /// scan line set by ScrollingUVPositioner's `uvDistance` property.
    /// </summary>
    public class ScanlineMaskTexController : MonoBehaviour
    {
        // DI
        public ScrollingUVPositioner positioner;
        
        /// <summary>
        /// Starting mask texture. Will be copied, should be readable.
        /// </summary>
        public Texture2D startingMask;

        private int _shaderidMask = Shader.PropertyToID("_ClearMask");
        private Vector3 _objectExtents;
        private Material _mat;
        private ScanlineTextureDrawing _drawing;
        private int _lastScanlineY;

        private void Awake()
        {
            // instantiate helper drawing class to help with scanline calculations
            _drawing = new ScanlineTextureDrawing(startingMask, true);

            // get object extends in order to map world position to mesh UV
            _objectExtents = positioner.scrollingObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
            
            // set created texture to the mat
            _mat = positioner.scrollingObject.GetComponent<Renderer>().sharedMaterial;
            _mat.SetTexture(_shaderidMask, _drawing.tex);
        }

        public void ContinueMaskedArea(Vector3 position, float width)
        {
            // update scanline helper with new scanline position information
            _drawing.UpdateScanlineFromUVDistance(positioner.uvDistance);

            // calculate amount of pixels to draw since last update
            var scrolledPixels = _drawing.CalculateTexturePixelsSinceScanline(_lastScanlineY);
            
            if (scrolledPixels > 0)
            {
                // calculate mask pixel position of provided vector
                var localPos = positioner.scrollingObject.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1f);
                var pixelPos = new Vector2(
                    Mathf.InverseLerp(_objectExtents.z, -_objectExtents.z, localPos.z) * _drawing.tex.width,
                    Mathf.InverseLerp(-_objectExtents.x, 0f, localPos.x) * _drawing.tex.height
                );

                // clear required amount of rows at scanline
                _drawing.ClearRows(scrolledPixels);
                
                // fill area
                _drawing.FillArea((int) pixelPos.x, (int) pixelPos.y, 35, scrolledPixels + 1);
                
                // save texture
                _drawing.Apply();
                
                // save scanline Y in property for subsequent scrolled pixels calculation
                _lastScanlineY = _drawing.scanlineTexY;
            }
        }
    }
}