using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;

namespace BlockGame.Scripts.Controllers.ToView
{
    /// <summary>
    /// Command that sets up initial game state (if no previous state was detected in persistent storage)
    /// </summary>
    public class InitialGameSetupCommand : Command
    {
        [Inject] public ToGridViewComponent toGridView { get; set; }
        [Inject] public IGameFieldState state { get; set; }
        
        public override void Execute()
        {
            // setup initial grid based on IGridTransform values
            state.SetupInitialAttachedGrid();
            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
            
            // spawn new random tetromino
            state.SpawnNewDetachedGrid();
            toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
        }
    }
}