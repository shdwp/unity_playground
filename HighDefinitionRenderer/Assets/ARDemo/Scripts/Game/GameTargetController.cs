using System;
using System.Collections;
using ARDemo.Scripts.TargetRacetrack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ARDemo.Scripts.Game
{
    /// <summary>
    /// Controller for the target sphere. Follows the provided race track in random direction and with random speed.
    /// Game object should be a child of world object (the one positioned to real-world marker position)
    /// </summary>
    public class GameTargetController : MonoBehaviour
    {
        /// <summary>
        /// GameModel, only used to test whether game is at pause
        /// </summary>
        public GameModelController gameModel;
        
        /// <summary>
        /// Provided racetrack to follow
        /// </summary>
        public TargetRacetrackModel racetrack;
        
        /// <summary>
        /// Min and max speed. Resulting value will be calculated by lerp with random factor.
        /// </summary>
        public float minSpeed = 0.01f, maxSpeed = 0.1f;
        
        /// <summary>
        /// Min, max and exponential curve factor of delay. Resulting value will be calculated by lerp with factor following exponential curve.
        /// </summary>
        public float minDelay = 0.3f, maxDelay = 2.5f, delayCurveFactor = 3;

        private float _speed;

        private void Update()
        {
            if (gameModel != null && gameModel.isInProgress)
            {
                // if game is in progress advance racetrack position based on speed
                racetrack.Advance(_speed * Time.deltaTime);
                
                // set position to the racetrack position
                transform.localPosition = racetrack.position;
            }
        }

        private void OnEnable()
        {
            // endless coroutine which will be run as long as this object is enabled
            StartCoroutine(CoroutineSpeedRandomizer());
        }

        private void OnDisable()
        {
            // stop that endless coroutine
            StopAllCoroutines();
        }

        private IEnumerator CoroutineSpeedRandomizer()
        {
            while (true)
            {
                // get speed sign (~50%)
                var sign = (Random.value - 2f) > 0 ? 1f : -1f;
                
                // get random speed value
                _speed = Mathf.Lerp(minSpeed, maxSpeed, Random.value) * sign;

                // calculate delay before next speed change based on parameters
                var delay = Mathf.Lerp(maxDelay, minDelay, Mathf.Pow(Random.value, delayCurveFactor));
                
                yield return new WaitForSeconds(delay);
            }
        }
    }
}