using System.Collections;
using ARDemo.Scripts.TargetTrack;
using UnityEngine;

namespace ARDemo.Scripts.Game
{
    public class GameTargetController : MonoBehaviour
    {
        public GameModelController gameModel;
        public TargetRacetrackModel racetrack;
        public float minSpeed = 0.01f, maxSpeed = 0.1f;
        public float minDelay = 0.3f, maxDelay = 2.5f, delayCurveFactor = 3;

        private float _speed;

        private void Start()
        {
            StartCoroutine(CoroutineSpeedRandomizer());
            _speed = -0.01f;
        }

        private void Update()
        {
            if (gameModel != null && gameModel.isInProgress)
            {
                racetrack.Advance(_speed * Time.deltaTime);
                transform.localPosition = racetrack.position;
            }
        }

        private void OnDisable()
        {
           StopAllCoroutines();
        }

        private IEnumerator CoroutineSpeedRandomizer()
        {
            while (true)
            {
                var sign = (Random.value - 2f) > 0 ? 1f : -1f;
                _speed = Mathf.Lerp(minSpeed, maxSpeed, Random.value) * sign;
                yield return new WaitForSeconds(Mathf.Lerp(maxDelay, minDelay, Mathf.Pow(Random.value, delayCurveFactor)));
            }
        }
    }
}