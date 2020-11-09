using System;
using BlockGame.Scripts.Controllers.ToView;
using BlockGame.Scripts.Model.Interfaces;
using BlockGame.Scripts.Signals;
using BlockGame.Scripts.Signals.FromView;
using strange.extensions.command.impl;
using UnityEngine;

namespace BlockGame.Scripts.Controllers.FromView
{
    /// <summary>
    /// Command that runs when views initiate detached grid (falling tetromino) movement (either gravity or user input).
    /// Will decide whether move is possible (grid collision), returning the result via callback. Move is always commited if possible
    /// and view should always apply if `true` was returned.
    /// </summary>
    public class AttemptGridModelMoveCommand : Command
    {
        [Inject] public AttemptGridModelMoveData data { get; set; }
        [Inject] public Action<bool> callback { get; set; }
        
        [Inject] public IGameFieldState state { get; set; }
        [Inject] public IGridTransform transform { get; set; }
        [Inject] public ToGridViewComponent toGridView { get; set; }
        
        public override void Execute()
        {
            if (data.type != GridType.Detached)
            {
                Debug.LogError($"Invalid grid type for UpdateGridModelPositionCommand, only supported for Detached");
                return;
            }

            // calculate new world-space position
            var newWorldPos = data.position + data.translation;

            // figure out grid transform rounding functions.
            // during transformations different functions will be applied to figure out the position on the grid
            // if move to be actually commited
            
            // for example, when moving left, x (row) component function will be Floor, meaning that 
            // collisions will appear right as block moves from its grid-assigned position to the left
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

            // copy old position (used to rollback the move)
            var oldGridPos = state.detachedGrid.pos;
            
            // calculate new position on the grid based on world position and rounding functions
            var newGridPos = transform.WorldToGridCustom(newWorldPos, rowFunction, colFunction);
            
            // update position of detached grid
            state.detachedGrid.Rebase(newGridPos);
            
            // check whether position actually changed during the move, in order to not run collision detection on every frame
            if (oldGridPos != newGridPos)
            {
                
                if (state.TestGridsCollision())
                {
                    // collision happened, but this not always results in block placement and subsequent spawn
                    // so for now update the position to match the old one
                    state.detachedGrid.Rebase(oldGridPos);
                    
                    if (data.translation.y < 0)
                    {
                        // move in question was a downward move, meaning that collision should indeed result in
                        // tetromino placement and new figure spawn
                        
                        // merge two grids
                        state.MergeDetachedGrid();
                        
                        // since grid was changed - check for completed rows
                        if (state.RemoveCompletedRowsIfNeeded())
                        {
                            // some of the rows were removed, so tell the view to reload the grid
                            // @TODO: make interface so only certain blocks will be removed in order to not run
                            // @TODO: full update
                            toGridView.DispatchReplaceGrid(GridType.Attached, state.attachedGrid);
                        }
                        else
                        {
                            // no rows were removed, so only current detached grid should be merged onto the existing
                            // view structure
                            toGridView.DispatchMergeGrid(GridType.Attached, state.detachedGrid);
                        }
                        
                        // spawn new tetromino
                        state.SpawnNewDetachedGrid();
                        
                        // replace detached grid view with spawned tetromino
                        toGridView.DispatchReplaceGrid(GridType.Detached, state.detachedGrid);
                    }

                    // move resulted in collision and view should be denied of it
                    callback(false);
                }
                else
                {
                    // move didn't result in collision meaning that move can be performed
                    callback(true);
                }
            }
            else
            {
                // move didn't result in position change therefore no collision is possible
                callback(true);
            }
        }
    }
}