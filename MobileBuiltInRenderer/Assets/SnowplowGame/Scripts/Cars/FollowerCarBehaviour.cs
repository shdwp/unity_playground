using System;
using System.Collections.Generic;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;

namespace SnowplowGame.Scripts.Cars
{
    /// <summary>
    /// Behaviour for follower cars - follows path of `followTarget` with delay of `delay` seconds
    /// </summary>
    public class FollowerCarBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Struct that saves target state
        /// </summary>
        private struct TargetState
        {
            public float Time;
            public Vector3 Position;
            public Quaternion Rotation;
        }

        // DI
        public RoadMovementCoordinator movCoordinator;
        public GameObject followTarget;
        
        /// <summary>
        /// Delay in seconds
        /// </summary>
        public float delay = 1f;

        /// <summary>
        /// List of target states, cleared on Update (number of frames * delay is kept in the list in order to follow)
        /// </summary>
        private List<TargetState> _targetStates = new List<TargetState>();

        private void Start()
        {
            // set initial position based on the delay, will be replaced later when target track info will be available
            
            var pos = transform.position;
            pos.x = -movCoordinator.speed * delay - 1f;
            transform.position = pos;
        }

        private void Update()
        {
            // insert current frame target state
            _targetStates.Insert(0, new TargetState
            {
                Time = Time.timeSinceLevelLoad,
                Position = followTarget.transform.position,
                Rotation = followTarget.transform.rotation,
            });

            // find applicable target state (based on delay)
            int i = 0;
            for (; i < _targetStates.Count; i++)
            {
                var pos = _targetStates[i];
                if (pos.Time <= Time.timeSinceLevelLoad - delay)
                {
                    // set object state to match target state at that moment
                    transform.position = new Vector3(pos.Position.x - delay * movCoordinator.speed, pos.Position.y, pos.Position.z);
                    transform.rotation = pos.Rotation;
                    
                    // cleanup excessive states
                    _targetStates.RemoveRange(i, _targetStates.Count - i);
                    break;
                }
            }
        }
    }
}