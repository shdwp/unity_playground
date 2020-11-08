using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.ScriptableObjects;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.Menu
{
    public class MainMenuButtonsMediator: Mediator
    {
        [Inject] public MainMenuButtonsView view { get; set; }
        [Inject] public PlayerSelectedGameModeSignal playerSelectedGameMode { get; set; }
        [Inject] public IIGamePersistentState gameState { get; set; }
        [Inject] public TransitionToGameSignal transitionToGame { get; set; }

        public override void OnRegister()
        {
            view.continueAvailable = gameState.canContinue;
            
            playerSelectedGameMode.AddListener(mode =>
            {
                switch (mode)
                {
                    case SelectedGameMode.Continue:
                        transitionToGame.Dispatch(false, GridSpawnerType.GrabBag);
                        break;
                        
                    case SelectedGameMode.NewWithGrabBagSpawner:
                        transitionToGame.Dispatch(true, GridSpawnerType.GrabBag);
                        break;
                        
                    case SelectedGameMode.NewWithSpecificFiguresSpawner:
                        transitionToGame.Dispatch(true, GridSpawnerType.SpecificFigures);
                        break;
                }
            });
        }
    }
}