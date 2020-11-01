using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    /// <summary>
    /// Simple mesh builder used during mesh splitting process.
    /// </summary>
    public class MeshBuilder
    {
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

        /// <summary>
        /// Insert new vert to act a existing surface vert.
        /// Call be called any number of times, only requirement being that resulting surface should be convex.
        /// Verts then will be triangulated into actual trigs when `CommitExistingSurface` will be called.
        /// Normal will also be used as a mean to determine index winding order.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="normal"></param>
        public void InsertExistingSurfaceVert(Vector3 p, Vector3 normal)
        {
            _surfaceVerts.Add(new Vert
            {
                point = p,
                normal = normal,
            });
        }

        /// <summary>
        /// Insert new vert to act as a intersection surface (cut area) vert.
        /// Call be called any number of times, only requirement being that resulting surface should be convex.
        /// Verts then will be triangulated into actual trigs when `CommitIntersectionSurface` will be called.
        /// Normal will also be used as a mean to determine index winding order.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="normal"></param>
        public void InsertIntersectionSurfaceVert(Vector3 p, Vector3 normal)
        {
            _intersectionVerts.Add(new Vert
            {
                point = p,
                normal = normal,
            });
        }

        /// <summary>
        /// Create new surface (number of verts) from vertices added by `InsertExistingSurfaceVert`.
        /// Will add respective trigs and remove inserted verts so new surface can be started.
        /// </summary>
        public void CommitExistingSurface()
        {
            if (_surfaceVerts.Count > 0)
            {
                CommitSurface(_surfaceVerts);
                _surfaceVerts.Clear();
            }
        }

        /// <summary>
        /// Create new surface (number of verts) from vertices added by `InsertIntersectionSurfaceVert`.
        /// Will add respective trigs and remove inserted verts so new surface can be started.
        /// </summary>
        public void CommitIntersectionSurface()
        {
            if (_intersectionVerts.Count > 0)
            {
                CommitSurface(_intersectionVerts);
                _intersectionVerts.Clear();
            }
        }

        /// <summary>
        /// Create mesh with data currently in the builder
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Triangulate vertsEnumerable, creating a number of trigs.
        /// Triangle winding order will be determined by normal of the first vert.
        /// @TODO: this can really use a better triangulation and winding algorithm.
        /// </summary>
        /// <param name="vertsEnumerable"></param>
        private void CommitSurface(IEnumerable<Vert> vertsEnumerable)
        {
            // create list so vertices can be sorted based on winding order
            var verts = new List<Vert>(vertsEnumerable);
            
            // this will be used as a normal for winding order calculation
            var windingNormal = verts.First().normal;
            
            // calculate centroid of verts
            var center = LightsaberDemoLib.CalculateVectorsCentroid(verts.Select(a => a.point));
            
            // sort vertices based on their angle from centroid, leaving them in CW order
            verts.Sort((first, second) =>
            {
                var a = first.point - center;
                var b = second.point - center;
                
                return Vector3.SignedAngle(center, a, windingNormal).CompareTo(Vector3.SignedAngle(center, b, windingNormal));
            });
            
            // triangulate and push indices into _meshTrigs list
            for (int i = 0; i < verts.Count - 2; i += 1)
            {
                _meshTrigs.Add(_meshVerts.Count);
                _meshTrigs.Add(_meshVerts.Count + i + 1);
                _meshTrigs.Add(_meshVerts.Count + i + 2);
            }

            // add vertices and normals
            _meshVerts.AddRange(verts.Select(a => a.point));
            _meshNormals.AddRange(verts.Select(a => a.normal));
        }
    }
}