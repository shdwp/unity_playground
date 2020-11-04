using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using strange.extensions.mediation.impl;

namespace BlockGame.Scripts.Views.GridTransform
{
    /// <summary>
    /// Mediator for grid setup. Will fire SetupGridTransformSignal with values from editor when registered.
    /// </summary>
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