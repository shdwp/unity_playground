using BlockGame.Scripts.Controllers.ToGridView;
using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals.ToGridView;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers
{
    public class UpdateGridModelPositionCommand : Command
    {
        [Inject] public GridType type { get; set; }
        [Inject] public Vector3 pos { get; set; }
        
        [Inject] public IGameState state { get; set; }
        [Inject] public IGridTransform transform { get; set; }
        [Inject] public ToGridViewComponent toGridView { get; set; }
        
        public override void Execute()
        {
            if (type != GridType.Detached)
            {
                Debug.LogError($"Invalid grid type for UpdateGridModelPositionCommand, only supported for Detached");
                return;
            }
            
            state.detachedGrid.Rebase(transform.WorldToGrid(pos));

            var grid = state.TestAndApplyDetachedGridCollisions();
            if (grid != null)
            {
                toGridView.DispatchMergeGrid(GridType.Attached, grid);
            }
        }
    }
}