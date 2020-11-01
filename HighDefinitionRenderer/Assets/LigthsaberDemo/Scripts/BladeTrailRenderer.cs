using System;
using System.Collections.Generic;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    /// <summary>
    /// Simple component that renders trail behind a blade.
    /// Blade is defined by two points, start and the end. Component generates mesh
    /// with vertices in world-space coordinates, which will disappear after time period.
    ///
    /// Initially creates one segment (rectangular shape of two trigs), which will be extended to connect to blade,
    /// then each time when travel distance gets past`resolutionDistance` new segment is created.
    ///
    /// Generated mesh doesn't have normals, UVs or backfaces since it's usually used with highly emissive material anyway.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class BladeTrailRenderer : MonoBehaviour
    {
        /// <summary>
        /// Blade definition transform references
        /// </summary>
        public Transform bladeBeginning, bladeEnd;
        
        /// <summary>
        /// Length at which segment (rectangular shape of 2 trigs) is created.
        /// Effectively describes how "smooth" the trail will be
        /// </summary>
        public float segmentLength = 0.5f;
        
        /// <summary>
        /// Time span for which each segment will be visible.
        /// </summary>
        public float trailDurationSeconds = 0.35f;
        
        private MeshFilter _filter;
        private Mesh _mesh;
        
        // verts of segments (each segment is 4 verts)
        private List<Vector3> _verts = new List<Vector3>();
        
        // trigs of segments (each segment is 6 indices)
        private List<int> _trigs = new List<int>();
        
        // list of times when each segment was created (Time.timeSinceLevelLoad)
        private List<float> _segmentCreationTime = new List<float>();

        // position of trailing vectors of last created segment 
        private Vector3? _currentSegmentBeginning, _currentSegmentEnd;
        
        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _filter.sharedMesh = _mesh;
        }

        private void Update()
        {
            var dirtyMesh = false;
            
            if (_trigs.Count > 0)
            {
                // go over segments array and remove segments which expired
                RemoveExpiredSegments();

                // invalidate mesh
                dirtyMesh = true;
            }

            if (!_currentSegmentBeginning.HasValue || !_currentSegmentEnd.HasValue)
            {
                // first iteration, populate _lastA/B variables so first segment can be created
                _currentSegmentBeginning = bladeBeginning.position;
                _currentSegmentEnd = bladeEnd.position;
            }
            else
            {
                // subsequent iterations
                
                // figure out max travel distance
                var currentSegmentLength = Math.Max(
                    Vector3.Distance(_currentSegmentBeginning.Value, bladeBeginning.position),
                    Vector3.Distance(_currentSegmentEnd.Value, bladeEnd.position)
                );

                if (_verts.Count > 0)
                {
                    // there are existing segments, first update it so it actually
                    // connects to the blade (or to the next segment, which will be created at blade position)
                    ExtendSegment(bladeBeginning.position, bladeEnd.position);

                    // invalidate mesh
                    dirtyMesh = true;
                }

                if (currentSegmentLength > segmentLength)
                {
                    // current segment length exceeded `segmentLength`, meaning that new segment should be created
                    // next time this segment will be updated
                    AddSegment(bladeBeginning.position, bladeEnd.position, _currentSegmentBeginning.Value, _currentSegmentEnd.Value);
                    
                    // setup current segment variables
                    _currentSegmentBeginning = bladeBeginning.position;
                    _currentSegmentEnd = bladeEnd.position;

                    // invalidate mesh
                    dirtyMesh = true;
                }
                
                if (_trigs.Count == 0)
                {
                    // at this point all segments are removed and distance covered in current motion is not enough
                    // to start new segment, therefore nullify variables so the segment can start anew
                    _currentSegmentBeginning = null;
                    _currentSegmentEnd = null;
                }
            }

            if (dirtyMesh)
            {
                // populate actual rendered mesh with new values since they were updated
                CommitMeshChanges();
            }
            
        }

        /// <summary>
        /// Add new trail segment
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        private void AddSegment(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
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
            
            _segmentCreationTime.Add(Time.timeSinceLevelLoad);
        }

        /// <summary>
        /// Extend last trail segment to connect to new c, d vectors
        /// </summary>
        /// <param name="newC"></param>
        /// <param name="newD"></param>
        private void ExtendSegment(Vector3 newC, Vector3 newD)
        {
            int cIdx = _verts.Count - 2, dIdx = cIdx + 1;

            _verts[cIdx] = newC;
            _verts[dIdx] = newD;
        }

        /// <summary>
        /// Remove expired segments
        /// </summary>
        private void RemoveExpiredSegments()
        {
            // create copy of times so 
            var segmentTimes = _segmentCreationTime.ToArray();
            
            // track number of removed segments to calculate indexes without reverse-iteration
            var numberOfRemovedSegments = 0;
            
            // iterate over segment times (array is always in sync with _trigs and _verts)
            for (int i = 0; i < segmentTimes.Length; i++)
            {
                var timeDelta = Time.timeSinceLevelLoad - segmentTimes[i];
                if (timeDelta > trailDurationSeconds)
                {
                    // time list is always ascending, meaning that expired segment is always first
                    
                    // remove respective trigs, verts and modify existing trigs to point to new vert idxs
                    _trigs.RemoveRange(0, 6);
                    _verts.RemoveRange(0, 4);
                    for (var ti = 0; ti < _trigs.Count; ti++)
                    {
                        _trigs[ti] -= 4;
                    }
                    
                    // remove time from actual list
                    _segmentCreationTime.RemoveAt(i - numberOfRemovedSegments);
                    numberOfRemovedSegments++;
                }
                else
                {
                    // segment has not expired, meaning that following segments can't expire as well
                    break;
                }
            }
        }

        /// <summary>
        /// Update rendered _mesh with data from _verts and _trigs
        /// </summary>
        private void CommitMeshChanges()
        {
            // remove trigs so it's safe to update vertices
            _mesh.triangles = new int[0];
            
            // update data and recalculate bounds
            _mesh.SetVertices(_verts);
            _mesh.SetTriangles(_trigs, 0);
            _mesh.RecalculateBounds();
        }
    }
}