using System;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

namespace BlockGame.Scripts.Views.Menu
{
    public class GameBackButtonView: View
    {
        [Inject] public PlayerGoBackToMenuSignal goBackToMenu { get; set; }

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