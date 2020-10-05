using System;
using System.Collections.Generic;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class BladeTrailRenderer : MonoBehaviour
    {
        public Transform bladeBeginning, bladeEnd;
        public float resolutionDistance = 0.35f;
        public float trailDurationSeconds = 0.35f;
        
        private MeshFilter _filter;
        private Mesh _mesh;
        
        private List<Vector3> _verts = new List<Vector3>();
        private List<int> _trigs = new List<int>();
        private List<float> _segmentTimes = new List<float>();

        private Vector3? _lastA, _lastB;
        
        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _filter.sharedMesh = _mesh;
        }

        private void Update()
        {
            if (_trigs.Count > 0)
            {
                RemoveExcessivePlanes();
            }

            if (_lastA.HasValue && _lastB.HasValue)
            {
                var maxDist = Math.Max(
                    Vector3.Distance(_lastA.Value, bladeBeginning.position),
                    Vector3.Distance(_lastB.Value, bladeEnd.position)
                );

                if (_verts.Count > 0)
                {
                    UpdateLastTrailPlane(bladeBeginning.position, bladeEnd.position);
                }

                if (maxDist > resolutionDistance)
                {
                    AddTrailPlane(bladeBeginning.position, bladeEnd.position, _lastA.Value, _lastB.Value);
                    
                    _lastA = bladeBeginning.position;
                    _lastB = bladeEnd.position;
                }
            }
            else
            {
                _lastA = bladeBeginning.position;
                _lastB = bladeEnd.position;
            }

            CommitMeshChanges();
        }

        private void AddTrailPlane(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            int aIdx = _verts.Count, bIdx = aIdx + 1, cIdx = bIdx + 1, dIdx = cIdx + 1;
            
            _verts.AddRange(new []
            {
                a, b, c, d
            });
            
            _trigs.AddRange(new []
            {
                aIdx, bIdx, cIdx,
                bIdx, dIdx, cIdx
            });
            
            _segmentTimes.Add(Time.realtimeSinceStartup);
        }

        private void UpdateLastTrailPlane(Vector3 a, Vector3 b)
        {
            int cIdx = _verts.Count - 2, dIdx = cIdx + 1;

            _verts[cIdx] = a;
            _verts[dIdx] = b;
        }

        private void RemoveExcessivePlanes()
        {
            var segmentTimes = _segmentTimes.ToArray();
            for (int i = 0; i < segmentTimes.Length; i++)
            {
                var delta = Time.realtimeSinceStartup - segmentTimes[i];
                if (delta > trailDurationSeconds)
                {
                    _trigs.RemoveRange(0, 6);
                    _verts.RemoveRange(0, 4);
                    for (var ti = 0; ti < _trigs.Count; ti++)
                    {
                        _trigs[ti] -= 4;
                    }
                    
                    _segmentTimes.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }

        private void CommitMeshChanges()
        {
            _mesh.triangles = new int[0];
            
            _mesh.SetVertices(_verts);
            _mesh.SetTriangles(_trigs, 0);
            _mesh.RecalculateBounds();
        }
    }
}