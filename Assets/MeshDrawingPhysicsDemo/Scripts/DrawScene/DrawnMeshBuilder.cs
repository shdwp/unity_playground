using System.Collections.Generic;
using UnityEngine;

namespace RollingDownDemo.Scripts.DrawScene
{
    /**
     * User drawn mesh builder class - provides API to generate tube-like mesh following user inputs.
     * 
     * Vertex coding scheme (assuming that mesh grows to the right)
     * 
     * top:
     * A        D
     * +--------+
     * |        |
     * +--------+
     * B        E
     *
     * bottom:
     * C          F
     * +----------+
     *
     * forward:
     * Ba       De
     * +--------+
     * |        |
     * +--------+
     * C        F
     *
     * We can get away with using 6 vertices (3 edges, 3 faces) since
     * it's actually preferable for the figure to be shaded as tube
     */
    public class DrawnMeshBuilder
    {
        private bool _cooked = true;
        
        /// <summary>
        /// boolean indicating that next call to `mesh` getter should redo the mesh
        /// </summary>
        public bool cooked => _cooked;

        /// <summary>
        /// vert count
        /// </summary>
        public int vertCount => _verts.Count;

        private Mesh _mesh = new Mesh();
        /// <summary>
        /// Result mesh getter. Will redo the mesh if any changes were detected since last call
        /// </summary>
        public Mesh mesh => GetMeshCookIfNeeded();

        // raw data
        private List<Vector3> _verts = new List<Vector3>();
        private List<int> _trigs = new List<int>();

        // current segment start
        private Vector2? _segmentStart;

        /// <summary>
        /// Add initial section of the segment, should be called when user starts to draw something,
        /// and then `AddContinuationSegment` for each consecutive input.
        /// </summary>
        /// <param name="from">Position in mesh coordinate space</param>
        /// <param name="to">Position in mesh coordinate space</param>
        /// <param name="thickness">Thickness of the section</param>
        public void AddSegment(Vector2 from, Vector2 to, float thickness)
        {
            var halfThickness = thickness / 2f;
            var size = GetXYThicknessOfSegment(from, to, halfThickness);
            
            // add new verts - 3 for start and 3 for finish
            _verts.AddRange(new []
            {
                /* A */ new Vector3(from.x + size.x, from.y - size.y, halfThickness), 
                /* B */ new Vector3(from.x - size.x, from.y + size.y, -halfThickness), 
                /* C */ new Vector3(from.x + size.x, from.y - size.y, -halfThickness), 
                
                /* D */ new Vector3(to.x + size.x, to.y - size.y, halfThickness), 
                /* E */ new Vector3(to.x - size.x, to.y + size.y, -halfThickness), 
                /* F */ new Vector3(to.x + size.x, to.y - size.y, -halfThickness), 
            });

            var count = _verts.Count;
            int AIdx = count - 6, BIdx = count - 5, CIdx = count - 4, DIdx = count - 3, EIdx = count - 2, FIdx = count - 1;
            
            // add trigs
            _trigs.AddRange(new []
            {
                /* top 1 */ AIdx, DIdx, EIdx,
                /* top 2 */ AIdx, EIdx, BIdx,
                
                /* front 1 */ FIdx, BIdx, EIdx,
                /* front 2 */ BIdx, FIdx, CIdx,
                
                /* back 1 */ DIdx, AIdx, FIdx,
                /* back 2 */ CIdx, FIdx, AIdx,
            });

            // update variable and force mesh to recook
            _segmentStart = to;
            _cooked = false;
        }

        /// <summary>
        /// Add continuation of the segment, should be called after `AddSegment` was called.
        /// Will add geometry spanning from previous segment `to` (either one from `AddSegment` or from `AddContinuationSegment`)
        /// to parameter `to`.
        /// </summary>
        /// <param name="to">Position in mesh coordinate space</param>
        /// <param name="thickness">Thickness of the section</param>
        public void AddContinuationSegment(Vector2 to, float thickness)
        {
            if (!_segmentStart.HasValue)
            {
                Debug.Assert(false, "Failed to add continuation segment - no previous verts");
                return;
            }

            var halfThickness = thickness / 2f;
            var size = GetXYThicknessOfSegment(_segmentStart.Value, to, halfThickness);
            
            // get existing ABC vertices (which were EDF in previous iteration)
            var count = _verts.Count;
            int AIdx = count - 3, BIdx = count - 2, CIdx = count - 1;
            
            // add verts
            _verts.AddRange(new []
            {
                /* D */ new Vector3(to.x + size.x, to.y - size.y, halfThickness), 
                /* E */ new Vector3(to.x - size.x, to.y + size.y, -halfThickness), 
                /* F */ new Vector3(to.x + size.x, to.y - size.y, -halfThickness), 
            });
            
            count = _verts.Count;
            int DIdx = count - 3, EIdx = count - 2, FIdx = count - 1;
            
            // add trigs
            _trigs.AddRange(new []
            {
                /* top 1 */ AIdx, DIdx, EIdx,
                /* top 2 */ AIdx, EIdx, BIdx,
                
                /* front 1 */ FIdx, BIdx, EIdx,
                /* front 2 */ BIdx, FIdx, CIdx,
                
                /* back 1 */ DIdx, AIdx, FIdx,
                /* back 2 */ CIdx, FIdx, AIdx,
            });

            // update variable and force mesh to recook
            _segmentStart = to;
            _cooked = false;
        }

        /// <summary>
        /// Reset mesh
        /// </summary>
        public void Reset()
        {
            _verts = new List<Vector3>();
            _trigs = new List<int>();
            _segmentStart = null;
            _cooked = false;
        }

        /// <summary>
        /// Getter method for the mesh - simply returns it if no changes were detected,
        /// or updates vertices, triangles, bounds and normals if there were changes
        /// </summary>
        /// <returns>Mesh instance</returns>
        private Mesh GetMeshCookIfNeeded()
        {
            if (_cooked)
            {
                return _mesh;
            }
            else
            {
                _mesh.SetVertices(_verts);
                _mesh.SetTriangles(_trigs, 0);
                _mesh.RecalculateBounds();
                _mesh.RecalculateNormals();

                _cooked = true;
                return _mesh;
            }
        }

        /// <summary>
        /// Calculate x and y segment thickness based on the angle of the segment
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="halfThickness">half of target thickness</param>
        /// <returns>vector of cos/sin values</returns>
        private Vector2 GetXYThicknessOfSegment(Vector2 from, Vector2 to, float halfThickness)
        {
            // calculate x and y thickness based on angle of the segment
            var normalizedDistance = (to - from).normalized;
            var xSize = Vector2.Dot(Vector2.up, normalizedDistance) * halfThickness;
            var ySize = Vector2.Dot(Vector2.right, normalizedDistance) * halfThickness;
            
            return new Vector2(xSize, ySize);
        }
    }
}