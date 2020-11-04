using BlockGame.Scripts.Signals;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.Grid
{
    public class GridTransformViewMediator: Mediator
    {
        [Inject] public GridTransformView view { get; set; }
        [Inject] public SetupGridTransformSignal setupSignal { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            
            setupSignal.Dispatch(view.bounds, new SetupGridTransformSignal.GridSize
            {
                rows = view.rows,
                cols = view.cols
            });
        }
    }
}