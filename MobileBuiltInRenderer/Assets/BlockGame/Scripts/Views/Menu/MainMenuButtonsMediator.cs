using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.ScriptableObjects;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.Menu
{
    /// <summary>
    /// Mediator for main menu button list.
    /// Fires off events to start the game based on user choice.
    /// </summary>
    public class MainMenuButtonsMediator: Mediator
    {
        [Inject] public MainMenuButtonsView view { get; set; }
        [Inject] public IIGamePersistentState gameState { get; set; }
        [Inject] public TransitionToGameSignal transitionToGame { get; set; }

        public override void OnRegister()
        {
            // setup view Continue button
            view.continueAvailable = gameState.canContinue;
            
            view.selectedGameMode.AddListener(mode =>
            {
                switch (mode)
                {
                    case MainMenuButtonsView.SelectedGameMode.Continue:
                        // player selected Continue - transition to game without cleaning the state, which
                        // will continue previously saved game
                        transitionToGame.Dispatch(false, GridSpawnerType.GrabBag);
                        break;
                        
                    case MainMenuButtonsView.SelectedGameMode.NewWithGrabBagSpawner:
                        // player selected new game with grab bag spawner - transition to game clearing any
                        // possible saved state
                        transitionToGame.Dispatch(true, GridSpawnerType.GrabBag);
                        break;
                        
                    case MainMenuButtonsView.SelectedGameMode.NewWithSpecificFiguresSpawner:
                        // player selected new game with specific figure spawner - transition to game clearing any
                        // possible saved state
                        transitionToGame.Dispatch(true, GridSpawnerType.SpecificFigures);
                        break;
                }
            });
        }
    }
}