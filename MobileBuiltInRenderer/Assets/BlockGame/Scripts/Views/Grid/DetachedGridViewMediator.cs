using BlockGame.Scripts.Signals;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class DetachedGridViewMediator: GridViewMediator
    {
        [Inject] public PlayerMoveDetachedGridSignal playerMove { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            
            playerMove.AddListener(dir =>
            {
                switch (dir)
                {
                    case PlayerMoveDetachedGridSignal.Direction.Left:
                        view.Move(Vector3.left);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Right:
                        view.Move(Vector3.right);
                        break;
                    
                    case PlayerMoveDetachedGridSignal.Direction.Down:
                        view.Push();
                        break;
                }
            });
        }
    }
}