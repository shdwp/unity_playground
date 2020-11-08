using BlockGame.Scripts.Signals.FromView;
using BlockGame.Scripts.Views.Signals;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.Menu
{
    public class GameBackButtonMediator: Mediator
    {
        [Inject] public PlayerGoBackToMenuSignal goBackToMenuSignal { get; set; }
        [Inject] public TransitionToMenuSignal transitionToMenuSignal { get; set; }

        public override void OnRegister()
        {
            goBackToMenuSignal.AddListener(() =>
            {
                transitionToMenuSignal.Dispatch();
            });
        }
    }
}