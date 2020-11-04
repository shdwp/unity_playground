using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.Menu
{
    /// <summary>
    /// Mediator for back button view on the game scene.
    /// Will figure TransitionToMenuSignal when user presses the button.
    /// </summary>
    public class GameBackButtonMediator: Mediator
    {
        [Inject] public GameBackButtonView view { get; set; }
        [Inject] public TransitionToMenuSignal transitionToMenuSignal { get; set; }

        public override void OnRegister()
        {
            view.goBackToMenu.AddListener(() =>
            {
                transitionToMenuSignal.Dispatch();
            });
        }
    }
}