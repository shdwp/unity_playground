using UnityEngine;

namespace SnowplowGame.Scripts.Cars
{
    /// <summary>
    /// Simple behavior designed to turn the wheel with set speed in euler angle
    /// </summary>
    public class WheelAnimationBehaviour : MonoBehaviour
    {
        public float speed = 100f;
        
        private void Update()
        {
            transform.rotation *= Quaternion.Euler(new Vector3(speed * Time.deltaTime, 0f, 0f));
        }
    }
}