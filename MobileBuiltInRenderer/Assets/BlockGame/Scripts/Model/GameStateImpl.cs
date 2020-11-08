using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.injector.api;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    public class GameStateImpl: IGameState
    {
        [Inject] public IInjectionBinder binder { get; set; }
        [Inject] public IPartialGrid<BlockDataModel> attachedGrid { get; set; }
        [Inject] public IPartialGrid<BlockDataModel> detachedGrid { get; set; }
        
        [Inject] public IGridTransform transform { get; set; }
        [Inject] public IGridSpawner<BlockDataModel> spawner { get; set; }

        public void SetupInitialAttachedGrid()
        {
            attachedGrid = binder.GetInstance<IPartialGrid<BlockDataModel>>();
            attachedGrid.SetupEmptyFromTransform();
        }

        public void MergeDetachedGrid()
        {
            attachedGrid.Merge(detachedGrid);
        }

        public void SpawnNewDetachedGrid()
        {
            detachedGrid = spawner.SpawnRandomGrid();
            detachedGrid.Rebase(new GridPosition(0, 0));
        }

        public bool RemoveCompletedRowsIfNeeded()
        {
            var alteredGrid = false;
            
            for (int col = 0; col < attachedGrid.cols; col++)
            {
                var occupiedCols = attachedGrid.EnumerateOccupancyOverRow(col).Aggregate(0, (a, b) => a + (b ? 1 : 0));
                if (occupiedCols == attachedGrid.rows)
                {
                    attachedGrid.RemoveCol(col);
                    alteredGrid = true;
                    col--;
                }
            }
            
            return alteredGrid;
        }

        public bool TestGridsCollision()
        {
            return !transform.IsGridInBounds(detachedGrid) || attachedGrid.DoesCollideWith(detachedGrid);
        }
    }
}