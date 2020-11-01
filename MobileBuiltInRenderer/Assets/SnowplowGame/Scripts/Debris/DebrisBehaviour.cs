using System;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowplowGame.Scripts.Debris
{
    /// <summary>
    /// Behaviour class for individual debris objects. First it follows provided `track`, then
    /// simply moves back based on `movCoordinator` speed.
    /// </summary>
    public class DebrisBehaviour : MonoBehaviour
    {
        // DI
        public RoadMovementCoordinator movCoordinator;
        
        /// <summary>
        /// Track to follow
        /// </summary>
        [NonSerialized]
        public DebrisTrack Track;
        
        /// <summary>
        /// Scale override
        /// </summary>
        public float scale = 0.035f;

        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        
        private readonly int _sidCreationTime = Shader.PropertyToID("_CreationTime");

        private void Awake()
        {
            _block = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();
            _renderer.SetPropertyBlock(_block);
        }

        private void Start()
        {
            // setup scale
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void Update()
        {
            if (Track.Advance(movCoordinator.speed * Time.deltaTime))
            {
                // track was advances and still not exhaused, simply follow the track
                transform.position = Track.position;
            }
            else
            {
                // track was finished, simply move according to the road movement
                transform.position += Vector3.left * (movCoordinator.speed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Restart debris after it was set anew (after object pool dequeue)
        /// </summary>
        public void Restart()
        {
            // update material block with new spawn time
            _block.SetFloat(_sidCreationTime, Time.timeSinceLevelLoad);
            _renderer.SetPropertyBlock(_block);

            // setup initial position and rotation
            transform.position = Track.position;
            transform.rotation = Random.rotation;
        }
    }
}