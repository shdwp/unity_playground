using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    public class MeshBuilder
    {
        public Vector3 centroid;
        
        private struct Vert
        {
            public Vector3 point;
            public Vector3 normal;
        }
        
        private List<Vector3> _meshVerts = new List<Vector3>();
        private List<int> _meshTrigs = new List<int>();
        private List<Vector3> _meshNormals = new List<Vector3>();
        
        private List<Vert> _surfaceVerts = new List<Vert>();
        private List<Vert> _intersectionVerts = new List<Vert>();

        public void InsertSurfaceVert(Vector3 p, Vector3 normal)
        {
            _surfaceVerts.Add(new Vert
            {
                point = p,
                normal = normal,
            });
        }

        public void InsertIntersectionVert(Vector3 p, Vector3 normal)
        {
            _intersectionVerts.Add(new Vert
            {
                point = p,
                normal = normal,
            });
        }

        public void CommitSurface()
        {
            if (_surfaceVerts.Count > 0)
            {
                CommitSurface(_surfaceVerts);
                _surfaceVerts.Clear();
            }
        }

        public void CommitIntersectionSurface()
        {
            if (_intersectionVerts.Count > 0)
            {
                CommitSurface(_intersectionVerts);
                _intersectionVerts.Clear();
            }
        }

        public void OffsetToCentroid()
        {
            centroid = _meshVerts.Aggregate(Vector3.zero, (a, b) => a + b) / _meshVerts.Count();
            for (var i = 0; i < _meshVerts.Count(); i++)
            {
                _meshVerts[i] = _meshVerts[i] - centroid;
            }
        }
        
        public Mesh CookMesh()
        {
            var mesh = new Mesh();
            
            mesh.SetVertices(_meshVerts);
            mesh.SetTriangles(_meshTrigs, 0);
            mesh.SetNormals(_meshNormals);
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void CommitSurface(IEnumerable<Vert> vertsEnumerable)
        {
            var verts = new List<Vert>(vertsEnumerable);
            var windingNormal = verts.First().normal;
            var center = CalculateVectorsCenter(verts);
            verts.Sort((first, second) =>
            {
                var a = first.point - center;
                var b = second.point - center;
                
                return Vector3.SignedAngle(center, a, windingNormal).CompareTo(Vector3.SignedAngle(center, b, windingNormal));
            });
            
            for (int i = 0; i < verts.Count - 2; i += 1)
            {
                _meshTrigs.Add(_meshVerts.Count);
                _meshTrigs.Add(_meshVerts.Count + i + 1);
                _meshTrigs.Add(_meshVerts.Count + i + 2);
            }

            _meshVerts.AddRange(verts.Select(a => a.point));
            _meshNormals.AddRange(verts.Select(a => a.normal));
        }

        private Vector3 CalculateVectorsCenter(IEnumerable<Vert> input)
        {
            return input.Aggregate(Vector3.zero, (a, b) => a + b.point) / input.Count();
        }
    }
}