using BlockGame.Scripts.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace BlockGame.Scripts.Views.Control
{
    public class KeyboardControlMediator : Mediator
    {
        [Inject] public KeyboardControlView view { get; set; }
        
        [Inject] public PlayerMoveDetachedGridSignal playerMove { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            
            view.directionKeyPressed.AddListener((direction) =>
            {
                switch (direction)
                {
                    case KeyboardControlView.Direction.Left:
                        playerMove.Dispatch(PlayerMoveDetachedGridSignal.Direction.Left);
                        break;
                    
                    case KeyboardControlView.Direction.Right:
                        playerMove.Dispatch(PlayerMoveDetachedGridSignal.Direction.Right);
                        break;
                    
                    case KeyboardControlView.Direction.Down:
                        playerMove.Dispatch(PlayerMoveDetachedGridSignal.Direction.Down);
                        break;
                }
            });
        }
    }
}