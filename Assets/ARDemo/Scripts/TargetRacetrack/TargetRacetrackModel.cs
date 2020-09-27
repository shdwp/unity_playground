using System.Collections.Generic;
using UnityEngine;

namespace ARDemo.Scripts.TargetTrack
{
    public class TargetRacetrackModel
    {
        private Transform[] _points;
        private int _fromIdx;
        private int _toIdx => _fromIdx >= _points.Length - 1 ? 0 : _fromIdx + 1;
        private float _offset;

        public Vector3 from => _points[_fromIdx].position;
        public Vector3 to => _points[_toIdx].position;

        public Vector3 position => Vector3.Lerp(from, to, Mathf.InverseLerp(0f, (from - to).magnitude, _offset));

        public TargetRacetrackModel(Transform[] points)
        {
            Debug.Assert(points.Length > 1, "Track should contain at least 2 points!");
            
            _points = points;
            _fromIdx = 0;
            _offset = 0f;
        }

        public static TargetRacetrackModel PositionFromRooTransform(Transform root)
        {
            var points = new List<Transform>();
            for (var i = 0; i < root.childCount; i++)
            {
                points.Add(root.GetChild(i));
            }
            
            return new TargetRacetrackModel(points.ToArray());
        }

        public TargetRacetrackModel Clone()
        {
            return new TargetRacetrackModel(_points);
        }

        public void Advance(float distance)
        {
            _offset += distance;
            UpdatePointIndex();
        }

        private void UpdatePointIndex()
        {
            // was advanced backwards 
            if (_offset < 0f)
            {
                // go to previous waypoint
                DecrementPointIndex();
                
                // update offset so that it's positioned on new segment
                _offset = (from - to).magnitude + _offset;

                if (_offset < 0f)
                {
                    // offset is still negative, meaning that another point should be skipped
                    UpdatePointIndex();
                } 
                
                // either way we do not need to run overrun calculations since we're going backwards
                return;
            } 
            
            // calculate delta to check for overruns
            var delta = (from - to).magnitude - _offset;
            if (delta < 0f)
            {
                // go to next waypoing
                IncrementPointIndex();
                
                // update offset so that it's positioned on new segment
                _offset = Mathf.Abs(delta);

                if ((from - to).magnitude - _offset < 0f)
                {
                    // delta is still negative, meaning that we're still in overrun and another point should be skipped
                    UpdatePointIndex();
                }
            }
        }

        private void IncrementPointIndex()
        {
            _fromIdx++;
            if (_fromIdx > _points.Length - 1)
            {
                _fromIdx = 0;
            }
        }

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