using System;
using SnowplowGame.Scripts.RoadScrolling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnowplowGame.Scripts
{
    /// <summary>
    /// Game scene controller. Transitions to menu scene when car collision occurs.
    /// </summary>
    public class SnowplowGameController : MonoBehaviour
    {
        public RoadMovementCoordinator movCoordinator;

        public void CarCollision()
        {
            var score = Time.timeSinceLevelLoad;
            
            // transition to menu scene
            SceneManager.LoadSceneAsync("SnowplowMenuScene").completed += _ =>
            {
                var controller = FindObjectOfType<SnowplowMenuController>();
                
                // update score displayed in menu
                controller.Score = score;
            };
        }
    }
}