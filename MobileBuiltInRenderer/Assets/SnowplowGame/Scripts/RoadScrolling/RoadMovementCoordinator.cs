using UnityEngine;

namespace SnowplowGame.Scripts.RoadScrolling
{
    /// <summary>
    /// Top-level controller in charge of track movement coordination.
    /// Advances positioners by speed, provides other components with unified speed value and other information.
    /// </summary>
    public class RoadMovementCoordinator: MonoBehaviour
    {
        private float _distance = 0f;
        
        /// <summary>
        /// Distance since start
        /// </summary>
        public float Distance
        {
            get => _distance;
            set
            {
                Advance(value - _distance);
            }
        }
        
        // DI
        public ScrollingMeshTilePositioner RoadPositioner;
        public ScrollingUVPositioner SnowPositioner;
        
        /// <summary>
        /// Speed
        /// </summary>
        public float speed = 10f;
        
        /// <summary>
        /// Road bounds
        /// </summary>
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(86f, 0f, 44f));

        private void Update()
        {
            // advance road position
            Advance(speed);
        }

        private void Advance(float dist)
        {
            _distance += dist;
            
            // advance positioners
            RoadPositioner.Advance(dist);
            SnowPositioner.Advance(dist);
        }
    }
}