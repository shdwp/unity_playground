using System.Collections.Generic;
using UnityEngine;

namespace ARDemo.Scripts.TargetRacetrack
{
    /// <summary>
    /// Model class for target racetrack. Tracks position along the track, providing means of advancing it either forward or backward.
    /// </summary>
    public class TargetRacetrackModel
    {
        // transforms are stored here instead of points so that tracks can be changed in runtime if needed
        private Transform[] _points;
        
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
        public TargetRacetrackModel(Transform[] points)
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
        public static TargetRacetrackModel PositionFromRooTransform(Transform root)
        {
            var points = new List<Transform>();
            for (var i = 0; i < root.childCount; i++)
            {
                points.Add(root.GetChild(i));
            }
            
            return new TargetRacetrackModel(points.ToArray());
        }

        /// <summary>
        /// Make a clone of model, keeping the same points but discarding progress information
        /// </summary>
        /// <returns></returns>
        public TargetRacetrackModel Clone()
        {
            return new TargetRacetrackModel(_points);
        }

        /// <summary>
        /// Advance model along the track
        /// </summary>
        /// <param name="distance">positive or negative distance in world-scale</param>
        public void Advance(float distance)
        {
            _offset += distance;
            UpdatePointIndex();
        }

        /// <summary>
        /// Method to update point indexes if needed. Will call itself recursively until _offset
        /// is in bounds of current segment.
        /// </summary>
        private void UpdatePointIndex()
        {
            // was advanced backwards 
            if (_offset < 0f)
            {
                // go to previous waypoint
                DecrementPointIndex();
                
                // update offset so that it's positioned on new segment
                _offset = (_from - _to).magnitude + _offset;

                if (_offset < 0f)
                {
                    // offset is still negative, meaning that another point should be skipped
                    UpdatePointIndex();
                } 
                
                // either way we do not need to run overrun calculations since we're going backwards
                return;
            } 
            
            // calculate delta to check for overruns
            var delta = (_from - _to).magnitude - _offset;
            if (delta < 0f)
            {
                // go to next waypoing
                IncrementPointIndex();
                
                // update offset so that it's positioned on new segment
                _offset = Mathf.Abs(delta);

                if ((_from - _to).magnitude - _offset < 0f)
                {
                    // delta is still negative, meaning that we're still in overrun and another point should be skipped
                    UpdatePointIndex();
                }
            }
        }

        /// <summary>
        /// Increment point index. Loops back to 0
        /// </summary>
        private void IncrementPointIndex()
        {
            _fromIdx++;
            if (_fromIdx > _points.Length - 1)
            {
                _fromIdx = 0;
            }
        }

        /// <summary>
        /// Decrement point index. Loops back to last point
        /// </summary>
        private void DecrementPointIndex()
        {
            _fromIdx--;
            if (_fromIdx < 0)
            {
                _fromIdx = _points.Length - 1;
            }
        }
    }
}