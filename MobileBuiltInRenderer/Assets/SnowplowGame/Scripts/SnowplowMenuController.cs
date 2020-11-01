using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnowplowGame.Scripts
{
    /// <summary>
    /// Menu scene controller. Starts game on button tap, displays last attempt score.
    /// </summary>
    public class SnowplowMenuController : MonoBehaviour
    {
        public Button startButton;
        public Text titleText;
        
        [NonSerialized]
        public float Score;
        
        private void Start()
        {
            startButton.onClick.AddListener(ButtonStartAction);

            if (Score > 0f)
            {
                // only show score when actual attempt happened
                titleText.text = $"Score: {Score}.";
            }
        }

        private void ButtonStartAction()
        {
            SceneManager.LoadSceneAsync("SnowplowGameScene");
        }
    }
}