using System;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;

namespace SnowplowGame.Scripts.Cars
{
    /// <summary>
    /// Behaviour class for obstacle cars. Deals with collision detection for game over.
    /// Expects that there will be only one trigger in the scene - player car.
    /// </summary>
    public class ObstacleCarBehaviour : MonoBehaviour
    {
        // DI
        public RoadMovementCoordinator movCoordinator;
        public SnowplowGameController gameController;
        
        private void Update()
        {
            // move cars down the road
            transform.position += Vector3.left * (movCoordinator.speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // end the game since collision with play has occured
            gameController.CarCollision();
        }
    }
}