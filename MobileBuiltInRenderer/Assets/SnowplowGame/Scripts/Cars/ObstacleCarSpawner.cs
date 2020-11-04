using System;
using System.Collections;
using Lib;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowplowGame.Scripts.Cars
{
    /// <summary>
    /// Spawner class for obstacle cars - spawns cars ahead of player, with increasing difficulty.
    /// </summary>
    public class ObstacleCarSpawner : MonoBehaviour
    {
        // DI
        public RoadMovementCoordinator movCoordinator;
        public SnowplowGameController gameController;
        
        /// <summary>
        /// obstacle car prefab. Should have `ObstacleCarBehaviour` component
        /// </summary>
        public GameObject prefab;
        
        /// <summary>
        /// Transform acting as a parent for created obstacles
        /// </summary>
        public Transform root;
        
        // rate of spawn, will gradually lower itself
        private float _rate = 2.5f;

        private Camera _camera;
        private GameObjectPool _pool;
        private float _lastSpawn;

        private void Awake()
        {
            _pool = new GameObjectPool(prefab);
            _camera = Camera.main;
        }

        private void Start()
        {
            // start coroutine that will spawn obstacle cars
            StartCoroutine(ObstacleSpawnCoroutine());
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        private IEnumerator ObstacleSpawnCoroutine()
        {
            while (true)
            {
                // free objects no longer visible on the screen
                // @TODO: properly calculate bounds
                _pool.Free(o => _camera.WorldToViewportPoint(o.transform.position).y < -0.2f);

                // dequeue new car from object pool
                var instance = _pool.Dequeue(root);

                // setup behaviour
                // @TODO: skip setup if this was done before
                var behaviour = instance.GetComponent<ObstacleCarBehaviour>();
                behaviour.movCoordinator = movCoordinator;
                behaviour.gameController = gameController;

                // setup random position and rotation
                instance.transform.position = new Vector3(movCoordinator.bounds.max.x, 0f, Random.Range(movCoordinator.bounds.min.z, movCoordinator.bounds.max.z));
                instance.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                // increase difficulty by lowering rate
                _rate = Math.Max(0.3f, _rate * 0.93f);
                yield return new WaitForSeconds(_rate);
            }
        }
    }
}