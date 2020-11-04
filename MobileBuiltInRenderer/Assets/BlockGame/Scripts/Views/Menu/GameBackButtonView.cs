using System;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace BlockGame.Scripts.Views.Menu
{
    /// <summary>
    /// View for the back button present on game scene.
    /// </summary>
    public class GameBackButtonView: View
    {
        /// <summary>
        /// Fired when back button is pressed
        /// </summary>
        public Signal goBackToMenu = new Signal();
        
        public Button backButton;
        
        protected override void Awake()
        {
            base.Awake();
            
            backButton.onClick.AddListener(BackButtonPressed);
        }

        private void BackButtonPressed()
        {
            goBackToMenu.Dispatch();
        }
    }
}