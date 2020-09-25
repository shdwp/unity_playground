using System.Collections.Generic;
using UnityEngine;

namespace RollingDownDemo.Scripts
{
    /**
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
        private List<Vector3> _verts = new List<Vector3>();
        private List<int> _trigs = new List<int>();

        private bool _cooked = true;
        public bool cooked => _cooked;

        private Mesh _mesh = new Mesh();
        public Mesh mesh => GetMeshCookIfNeeded();

        private Vector2? _segmentStart;

        public void AddSegment(Vector2 from, Vector2 to, float size)
        {
            var halfSize = size / 2f;
            var normalizedDistance = (to - from).normalized;
            var xSize = Vector2.Dot(Vector2.up, normalizedDistance) * halfSize;
            var ySize = Vector2.Dot(Vector2.right, normalizedDistance) * halfSize;
            
            _verts.AddRange(new []
            {
                /* A */ new Vector3(from.x + xSize, from.y - ySize, halfSize), 
                /* B */ new Vector3(from.x - xSize, from.y + ySize, -halfSize), 
                /* C */ new Vector3(from.x + xSize, from.y - ySize, -halfSize), 
                
                /* D */ new Vector3(to.x + xSize, to.y - ySize, halfSize), 
                /* E */ new Vector3(to.x - xSize, to.y + ySize, -halfSize), 
                /* F */ new Vector3(to.x + xSize, to.y - ySize, -halfSize), 
            });

            var count = _verts.Count;
            int AIdx = count - 6, BIdx = count - 5, CIdx = count - 4, DIdx = count - 3, EIdx = count - 2, FIdx = count - 1;
            
            _trigs.AddRange(new []
            {
                /* top 1 */ AIdx, DIdx, EIdx,
                /* top 2 */ AIdx, EIdx, BIdx,
                
                /* front 1 */ FIdx, BIdx, EIdx,
                /* front 2 */ BIdx, FIdx, CIdx,
                
                /* back 1 */ DIdx, AIdx, FIdx,
                /* back 2 */ CIdx, FIdx, AIdx,
            });

            _segmentStart = to;
            _cooked = false;
        }

        public void AddContinuationSegment(Vector2 to, float size)
        {
            if (!_segmentStart.HasValue)
            {
                Debug.Assert(false, "Failed to add continuation segment - no previous verts");
                return;
            }

            var halfSize = size / 2f;
            var normalizedDistance = (to - _segmentStart.Value).normalized;
            var xSize = Vector2.Dot(Vector2.up, normalizedDistance) * halfSize;
            var ySize = Vector2.Dot(Vector2.right, normalizedDistance) * halfSize;
            
            // get previous EDF->ABC vertices
            var count = _verts.Count;
            int AIdx = count - 3, BIdx = count - 2, CIdx = count - 1;
            
            _verts.AddRange(new []
            {
                /* D */ new Vector3(to.x + xSize, to.y - ySize, halfSize), 
                /* E */ new Vector3(to.x - xSize, to.y + ySize, -halfSize), 
                /* F */ new Vector3(to.x + xSize, to.y - ySize, -halfSize), 
            });
            
            count = _verts.Count;
            int DIdx = count - 3, EIdx = count - 2, FIdx = count - 1;
            
            _trigs.AddRange(new []
            {
                /* top 1 */ AIdx, DIdx, EIdx,
                /* top 2 */ AIdx, EIdx, BIdx,
                
                /* front 1 */ FIdx, BIdx, EIdx,
                /* front 2 */ BIdx, FIdx, CIdx,
                
                /* back 1 */ DIdx, AIdx, FIdx,
                /* back 2 */ CIdx, FIdx, AIdx,
            });

            _segmentStart = to;
            _cooked = false;
        }

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
    }
}