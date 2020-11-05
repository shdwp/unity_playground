using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;

namespace BlockGame.Scripts.Controllers.ToGridView
{
    public class InitialGameSetupCommand : Command
    {
        [Inject] public ToGridViewComponent toGridView { get; set; }
        [Inject] public IGameState state { get; set; }
        
        public override void Execute()
        {
            state.attachedGrid = injectionBinder.GetInstance<IPartialGrid<BlockDataModel>>();
            state.attachedGrid.SetupFullFieldWithFloor(new BlockDataModel(BlockDataModel.Color.Cyan));
            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
            
            state.detachedGrid = injectionBinder.GetInstance<IPartialGrid<BlockDataModel>>();
            state.detachedGrid.Setup3x3(new BlockDataModel(BlockDataModel.Color.Purple),  " x " +
                                                                                          "xx " +
                                                                                          "x  ");
            toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
        }
    }
}