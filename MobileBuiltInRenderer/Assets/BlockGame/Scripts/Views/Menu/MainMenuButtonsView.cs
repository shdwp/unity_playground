using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlockGame.Scripts.Views.Menu
{
    /// <summary>
    /// View for main menu button list
    /// 
    /// Creates buttons based on availability of some options and listens for their events.
    /// </summary>
    public class MainMenuButtonsView: View
    {
        /// <summary>
        /// Game mode that user selected
        /// </summary>
        public enum SelectedGameMode
        {
            Continue,
            NewWithGrabBagSpawner,
            NewWithSpecificFiguresSpawner,
        }
        
        /// <summary>
        /// Prefab to use for the button
        /// </summary>
        public GameObject buttonPrefab;

        /// <summary>
        /// Whether there's is "Continue" button present
        /// </summary>
        public bool continueAvailable = false;

        /// <summary>
        /// Signal that will be fired when user selects game mode by pressing the button
        /// </summary>
        public Signal<SelectedGameMode> selectedGameMode = new Signal<SelectedGameMode>();

        protected override void Start()
        {
            base.Start();

            if (continueAvailable)
            {
                // add "Continue" button on the top if needed
                AddButtonForGameMode("Continue", ContinueButtonAction);
            }
            
            // add the rest of the buttons
            AddButtonForGameMode("Grab Bag", NewGrabBagButtonAction);
            AddButtonForGameMode("Specific Shapes", NewSpecificFiguresButtonAction);
        }

        /// <summary>
        /// Add button for game mode with title and onClick callback
        /// </summary>
        /// <param name="title"></param>
        /// <param name="action"></param>
        private void AddButtonForGameMode(string title, UnityAction action)
        {
            var buttonObject = Instantiate(buttonPrefab, transform);
            var button = buttonObject.GetComponentInChildren<Button>();
            var text = buttonObject.GetComponentInChildren<Text>();

            text.text = title;
            button.onClick.AddListener(action);
        }

        /// <summary>
        /// Action for Continue button
        /// </summary>
        private void ContinueButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.Continue);
        }

        /// <summary>
        /// Action for new game with grab bag spawner button
        /// </summary>
        private void NewGrabBagButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.NewWithGrabBagSpawner);
        }

        /// <summary>
        /// Action for new game with specific figures spawner button
        /// </summary>
        private void NewSpecificFiguresButtonAction()
        {
            selectedGameMode.Dispatch(SelectedGameMode.NewWithSpecificFiguresSpawner);
        }
    }
}