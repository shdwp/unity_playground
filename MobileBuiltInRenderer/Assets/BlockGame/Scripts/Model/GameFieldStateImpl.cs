using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.injector.api;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    /// <summary>
    /// Default implementation for IGameFieldState
    /// </summary>
    public class GameFieldStateImpl: IGameFieldState
    {
        [Inject] public IInjectionBinder binder { get; set; }
        [Inject] public IPartialGrid<CellDataModel> attachedGrid { get; set; }
        [Inject] public IPartialGrid<CellDataModel> detachedGrid { get; set; }
        
        [Inject] public IGridTransform transform { get; set; }
        [Inject] public IGridSpawner<CellDataModel> spawner { get; set; }

        public void SetupInitialAttachedGrid()
        {
            // get new instance from DI
            attachedGrid = binder.GetInstance<IPartialGrid<CellDataModel>>();
            
            // setup it based on DI transform
            attachedGrid.SetupEmptyFromTransform();
        }

        public void MergeDetachedGrid()
        {
            attachedGrid.Merge(detachedGrid);
        }

        public void SpawnNewDetachedGrid()
        {
            detachedGrid = spawner.SpawnRandomGrid();
        }

        public bool RemoveCompletedRowsIfNeeded()
        {
            // bool to indicate whether grid was altered during cycle
            var alteredGrid = false;
            
            for (int col = 0; col < attachedGrid.cols; col++)
            {
                // count occupied cells on this column
                var occupiedCols = attachedGrid.EnumerateOccupancyOverRow(col).Aggregate(0, (a, b) => a + (b ? 1 : 0));
                
                if (occupiedCols == attachedGrid.rows)
                {
                    // every cell was occupied (full row), meaning that it is completed and can be removed
                    attachedGrid.RemoveColumn(col);
                    
                    alteredGrid = true;
                    
                    // alter counter so that cycle dont skip lines
                    col--;
                }
            }
            
            return alteredGrid;
        }

        public bool TestGridsCollision()
        {
            // test for both intergrid collisions and bound collisions
            return !transform.IsGridInBounds(detachedGrid) || attachedGrid.DoesCollideWith(detachedGrid);
        }
    }
}