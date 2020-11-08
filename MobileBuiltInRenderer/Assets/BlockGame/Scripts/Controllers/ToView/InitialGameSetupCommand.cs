using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;

namespace BlockGame.Scripts.Controllers.ToView
{
    public class InitialGameSetupCommand : Command
    {
        [Inject] public ToGridViewComponent toGridView { get; set; }
        [Inject] public IGameState state { get; set; }
        
        public override void Execute()
        {
            state.SetupInitialAttachedGrid();
            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
            
            state.SpawnNewDetachedGrid();
            toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
        }
    }
}