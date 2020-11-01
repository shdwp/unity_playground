using System.Collections.Generic;
using UnityEngine;

namespace SnowplowGame.Scripts.Debris
{
    /// <summary>
    /// Class that represents track (defined by n amount of points), providing ability to traverse it getting position data
    /// </summary>
    public class DebrisTrack
    {
        // transforms are stored here instead of points so that tracks can be changed in runtime if needed
        private Transform[] _points;

        private bool _exhaused = false;
        
        // start point index of current segment
        private int _fromIdx;
        
        // finish point index of current segment
        private int _toIdx => _fromIdx >= _points.Length - 1 ? 0 : _fromIdx + 1;
        
        // offset along the segment (from start position)
        private float _offset;

        // start position of current segment
        private Vector3 _from => _points[_fromIdx].position;
        
        // finish position of current segment
        private Vector3 _to => _points[_toIdx].position;

        /// <summary>
        /// Current position on the track
        /// </summary>
        public Vector3 position => Vector3.Lerp(_from, _to, Mathf.InverseLerp(0f, (_from - _to).magnitude, _offset));

        /// <summary>
        /// Constructor. `points` should at least contain 2 points
        /// </summary>
        /// <param name="points"></param>
        public DebrisTrack(Transform[] points)
        {
            Debug.Assert(points.Length > 1, "Track should contain at least 2 points!");
            
            _points = points;
            _fromIdx = 0;
            _offset = 0f;
        }

        /// <summary>
        /// Static helper method to create racetrack from root transform. Will scan `root`s children, positions of which
        /// will be interpreted as points of the racetrack.
        /// </summary>
        /// <param name="root"></param>
        /// <returns>Instance of model</returns>
        public static DebrisTrack PositionFromRootTransform(Transform root)
        {
            var points = new List<Transform>();
            for (var i = 0; i < root.childCount; i++)
            {
                points.Add(root.GetChild(i));
            }
            
            return new DebrisTrack(points.ToArray());
        }

        /// <summary>
        /// Make a clone of model, keeping the same points but discarding progress information
        /// </summary>
        /// <returns></returns>
        public DebrisTrack Clone()
        {
            return new DebrisTrack(_points);
        }

        /// <summary>
        /// Advance model along the track
        /// </summary>
        /// <param name="distance">positive distance in world-scale</param>
        /// <returns>false if path was exhaused, true if path still continues</returns>
        public bool Advance(float distance)
        {
            if (_exhaused)
            {
                return false;
            }
            
            _offset += distance;
            UpdatePointIndex();
            return !_exhaused;
        }

        /// <summary>
        /// Method to update point indexes if needed. Will call itself recursively until _offset
        /// is in bounds of current segment.
        /// </summary>
        private void UpdatePointIndex()
        {
            // calculate delta to check for overruns
            var delta = (_from - _to).magnitude - _offset;
            if (delta < 0f)
            {
                if (IncrementPointIndex())
                {
                    // update offset so that it's positioned on new segment
                    _offset = Mathf.Abs(delta);

                    if ((_from - _to).magnitude - _offset < 0f)
                    {
                        // delta is still negative, meaning that we're still in overrun and another point should be skipped
                        UpdatePointIndex();
                    }
                }
                else
                {
                    _exhaused = true;
                }
            }
        }

        /// <summary>
        /// Increment point index
        /// </summary>
        private bool IncrementPointIndex()
        {
            _fromIdx++;
            if (_fromIdx > _points.Length - 2)
            {
                _fromIdx = _points.Length - 2;
                return false;
            }

            return true;
        }
    }
}