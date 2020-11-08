using System;
using BlockGame.Scripts.Controllers.ToView;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using strange.extensions.command.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers.FromView
{
    public class AttemptGridModelMoveCommand : Command
    {
        [Inject] public AttemptGridModelMoveData data { get; set; }
        [Inject] public Action<bool> callback { get; set; }
        
        [Inject] public IGameState state { get; set; }
        [Inject] public IGridTransform transform { get; set; }
        [Inject] public ToGridViewComponent toGridView { get; set; }
        
        public override void Execute()
        {
            if (data.type != GridType.Detached)
            {
                Debug.LogError($"Invalid grid type for UpdateGridModelPositionCommand, only supported for Detached");
                return;
            }

            var newWorldPos = data.position + data.translation;

            Func<float, int> rowFunction = Mathf.RoundToInt;
            Func<float, int> colFunction = Mathf.RoundToInt;
            
            if (data.translation.y > 0)
            {
                colFunction = Mathf.FloorToInt;
            } else if (data.translation.y < 0)
            {
                colFunction = Mathf.CeilToInt;
            }

            if (data.translation.x > 0)
            {
                rowFunction = Mathf.CeilToInt;
            } else if (data.translation.x < 0)
            {
                rowFunction = Mathf.FloorToInt;
            }

            var oldGridPos = state.detachedGrid.pos;
            var newGridPos = transform.WorldToGridCustom(newWorldPos, rowFunction, colFunction);
            
            if (oldGridPos != newGridPos)
            {
                state.detachedGrid.Rebase(newGridPos);
                
                if (state.TestGridsCollision())
                {
                    state.detachedGrid.Rebase(oldGridPos);
                    
                    if (data.translation.y < 0)
                    {
                        state.MergeDetachedGrid();
                        if (state.RemoveCompletedRowsIfNeeded())
                        {
                            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
                        }
                        else
                        {
                            toGridView.DispatchMergeGrid(GridType.Attached, state.detachedGrid);
                        }
                        
                        state.SpawnNewDetachedGrid();
                        toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
                    }

                    callback(false);
                }
                else
                {
                    callback(true);
                }
            }
            else
            {
                state.detachedGrid.Rebase(newGridPos);
                callback(true);
            }
        }
    }
}