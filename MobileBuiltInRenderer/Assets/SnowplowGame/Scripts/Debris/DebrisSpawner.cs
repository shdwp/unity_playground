using System.Collections;
using System.Collections.Generic;
using Lib;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowplowGame.Scripts.Debris
{
    /// <summary>
    /// Class that spawns prefabs at target locations, with randomized rotations.
    /// Prefabs should have `TrackDebrisBehaviour`.
    /// </summary>
    public class DebrisSpawner : MonoBehaviour
    {
        // DI
        public RoadMovementCoordinator movCoordinator;
        
        /// <summary>
        /// Parent object for spawned debris
        /// </summary>
        public Transform root;
        
        /// <summary>
        /// Array of prefabs to use. Random one will be picked each time.
        /// </summary>
        public GameObject[] prefabs;
        
        /// <summary>
        /// Array of transforms which contain debris initial track information (see `TrackDebrisBehaviour.track`)
        /// </summary>
        public Transform[] trackRoots;
        
        /// <summary>
        /// Spawn rate
        /// </summary>
        public float spawnRate = 0.1f;
        
        /// <summary>
        /// Scale to override in prefabs
        /// </summary>
        public float scale = 0.035f;

        private Camera _camera;
        private Dictionary<int, GameObjectPool> _pools = new Dictionary<int, GameObjectPool>();

        private void Awake()
        {
            // create separate pools for each prefab type
            for (int i = 0; i < prefabs.Length; i++)
            {
                _pools[i] = new GameObjectPool(prefabs[i]);
            }

            _camera = Camera.main;
        }

        private void Start()
        {
            StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            while (true)
            {
                // free objects no longer visible in object pools
                foreach (var pool in _pools.Values)
                {
                    pool.Free(o => Camera.main.WorldToViewportPoint(o.transform.position).y < -0.2f);
                }
                    
                // spawn one debris object for each track
                for (int trackIdx = 0; trackIdx < trackRoots.Length; trackIdx++)
                {
                    // dequeue random prefab instance
                    var prefabIdx = Random.Range(0, prefabs.Length - 1);
                    var instance = _pools[prefabIdx].Dequeue(root);
                    
                    // setup parameters
                    var debrisBehaviour = instance.GetComponent<DebrisBehaviour>();
                    debrisBehaviour.Track = DebrisTrack.PositionFromRootTransform(trackRoots[trackIdx]);
                    
                    // @TODO: avoid setting those on subsequent calls
                    debrisBehaviour.movCoordinator = movCoordinator;
                    debrisBehaviour.scale = scale;
                    
                    // signal behaviour that it was set up anew
                    debrisBehaviour.Restart();
                }
                
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}