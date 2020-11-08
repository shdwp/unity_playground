using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;

namespace BlockGame.Scripts.Controllers.ToView
{
    public class RestoredGameSetupCommand: Command
    {
        [Inject] public ToGridViewComponent toGridView { get; set; }
        [Inject] public IGameState state { get; set; }
        [Inject] public IIGamePersistentState persistentState { get; set; }

        public override void Execute()
        {
            persistentState.RestoreState(state);
            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
            toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
        }
    }
}