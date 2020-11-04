using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;

namespace BlockGame.Scripts.Controllers.ToView
{
    /// <summary>
    /// Command to setup game state based on state found in persistent storage
    /// </summary>
    public class RestoredGameSetupCommand: Command
    {
        [Inject] public ToGridViewComponent toGridView { get; set; }
        [Inject] public IGameFieldState state { get; set; }
        [Inject] public IIGamePersistentState persistentState { get; set; }

        public override void Execute()
        {
            // restore state
            persistentState.RestoreState(state);
            
            // send signals to view to update their contents
            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
            toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
        }
    }
}