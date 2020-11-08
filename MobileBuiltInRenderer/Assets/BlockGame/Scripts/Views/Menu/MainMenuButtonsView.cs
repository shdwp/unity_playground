using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlockGame.Scripts.Views.Menu
{
    public class MainMenuButtonsView: View
    {
        public GameObject buttonPrefab;

        public bool continueAvailable = false;

        [Inject] public PlayerSelectedGameModeSignal selectedGameMode { get; set; }

        protected override void Start()
        {
            base.Start();

            if (continueAvailable)
            {
                AddButtonForGameMode("Continue", ContinueButtonAction);
            }
            
            AddButtonForGameMode("Grab Bag", NewGrabBagButtonAction);
            AddButtonForGameMode("Specific Shapes", NewSpecificFiguresButtonAction);
        }

        private void AddButtonForGameMode(string title, UnityAction action)
        {
            var buttonObject = Instantiate(buttonPrefab, transform);
            var button = buttonObject.GetComponentInChildren<Button>();
            var text = buttonObject.GetComponentInChildren<Text>();

            text.text = title;
            button.onClick.AddListener(action);
        }

        private void ContinueButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.Continue);
        }

        private void NewGrabBagButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.NewWithGrabBagSpawner);
        }

        private void NewSpecificFiguresButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.NewWithSpecificFiguresSpawner);
        }
    }
}