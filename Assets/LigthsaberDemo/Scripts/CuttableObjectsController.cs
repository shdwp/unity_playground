using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LigthsaberDemo.Scripts
{
    public class CuttableObjectsController : MonoBehaviour
    {
        public Transform worldTransform;
        public GameObject[] prefabs;

        private int prefabIndex = 0;
        private GameObject _target;
        private Material _material;
        private Mesh _mesh;

        private void Start()
        {
            SpawnNewTarget();
        }

        public void Cut(Plane p)
        {
            var meshBuilders = new []
            {
                new MeshBuilder(),
                new MeshBuilder(),
            };

            var forceVectors = new[]
            {
                p.normal,
                -p.normal,
            };
            
            SeparateMeshWithPlane(p, _mesh, meshBuilders[0], meshBuilders[1]);

            for (var i = 0; i < meshBuilders.Count(); i++)
            {
                var debrisObject = new GameObject();
                debrisObject.AddComponent<DebrisController>().InitialSetup(
                    worldTransform,
                    _target.transform,
                    meshBuilders[i].CookMesh(),
                    _material,
                    forceVectors[i] * 400f
                );
            }

            Destroy(_target);
            StartCoroutine(SpawnNewTargetAfterDelayCoroutine());
        }

        private IEnumerator SpawnNewTargetAfterDelayCoroutine()
        {
            yield return new WaitForSeconds(2f);
            SpawnNewTarget();
            yield return null;
        }

        private void SpawnNewTarget()
        {
            if (prefabs.Length > 0)
            {
                _target = Instantiate(prefabs[prefabIndex++]);
                _target.transform.SetParent(worldTransform, false);

                _mesh = _target.GetComponent<MeshFilter>().sharedMesh;
                _material = _target.GetComponent<MeshRenderer>().sharedMaterial;

                prefabIndex = prefabIndex > prefabs.Length - 1 ? 0 : prefabIndex;
            }
        }

        private void SeparateMeshWithPlane(Plane p, Mesh mesh, MeshBuilder aboveMeshBuilder, MeshBuilder belowMeshBuilder)
        {
            var transformedNormal = ((Vector3)(_target.transform.localToWorldMatrix.transpose * p.normal)).normalized;
            p.SetNormalAndPosition(transformedNormal, transform.InverseTransformPoint(Vector3.zero));
            
            var trigs = mesh.GetTriangles(0);
            var verts = mesh.vertices;
            var normals = mesh.normals;

            for (var trigIdx = 0; trigIdx < trigs.Length; trigIdx += 3)
            {
                var checkedEdgeMask = 0;
                
                for (var vertAIdx = 0; vertAIdx < 3; vertAIdx++)
                {
                    var vertA = verts[trigs[trigIdx + vertAIdx]];
                    var vertANormal = normals[trigs[trigIdx + vertAIdx]];
                    var vertAAbove = p.GetSide(vertA);
                    if (vertAAbove)
                    {
                        aboveMeshBuilder.InsertSurfaceVert(vertA, vertANormal);
                    }
                    else
                    {
                        belowMeshBuilder.InsertSurfaceVert(vertA, vertANormal);
                    }

                    for (var vertBIdx = 0; vertBIdx < 3; vertBIdx++)
                    {
                        if (vertAIdx == vertBIdx || (checkedEdgeMask & 1 << (vertAIdx + vertBIdx)) > 0)
                        {
                            continue;
                        }
                        
                        var vertB = verts[trigs[trigIdx + vertBIdx]];
                        var vertBNormal = normals[trigs[trigIdx + vertBIdx]];
                        var vertBAbove = p.GetSide(vertB);

                        if (vertAAbove != vertBAbove)
                        {
                            var intersection = FindEdgeIntersection(
                                p,
                                vertAAbove ? vertA : vertB,
                                !vertBAbove ? vertB : vertA
                            );

                            aboveMeshBuilder.InsertSurfaceVert(intersection, vertANormal);
                            belowMeshBuilder.InsertSurfaceVert(intersection, vertANormal);
                            
                            aboveMeshBuilder.InsertIntersectionVert(intersection, -p.normal);
                            belowMeshBuilder.InsertIntersectionVert(intersection, p.normal);
                        }
                        
                        checkedEdgeMask |= 1 << (vertAIdx + vertBIdx);
                    }
                }

                aboveMeshBuilder.CommitSurface();
                belowMeshBuilder.CommitSurface();
            }

            aboveMeshBuilder.CommitIntersectionSurface();
            belowMeshBuilder.CommitIntersectionSurface();
        }
        
        private Vector3 FindEdgeIntersection(Plane plane, Vector3 pointAbove, Vector3 pointBelow)
        {
            var ray = new Ray(pointAbove, (pointAbove - pointBelow).normalized);
            plane.Raycast(ray, out var dist);
            return ray.origin + ray.direction * dist;
        }
    }
}