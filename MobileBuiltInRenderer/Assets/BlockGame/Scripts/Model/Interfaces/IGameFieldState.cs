using System;

namespace BlockGame.Scripts.Model.Interfaces
{
    /// <summary>
    /// Grid type - attached is the one that connected to the ground and not moving,
    /// detached is the one that falls from the top
    /// </summary>
    public enum GridType
    {
        Attached,
        Detached
    }
        
    /// <summary>
    /// Overall game field state, holds attached and detached grids and provides
    /// methods to control their interaction (collision detection, merging, spawning, etc)
    /// </summary>
    public interface IGameFieldState
    {
        /// <summary>
        /// Attached grid instance
        /// </summary>
        IPartialGrid<CellDataModel> attachedGrid { get; }
        
        /// <summary>
        /// Detached grid instance. Swapped when new tetrominoes are spawned
        /// </summary>
        IPartialGrid<CellDataModel> detachedGrid { get; }

        /// <summary>
        /// Setup initial field following size in IGridTransform
        /// </summary>
        void SetupInitialAttachedGrid();
        
        /// <summary>
        /// Test whether detached grid collides either with grid bounds or attached grid
        /// </summary>
        /// <returns>true if collision happened, false otherwise</returns>
        bool TestGridsCollision();
        
        /// <summary>
        /// Merge detached grid to attached grid
        /// </summary>
        void MergeDetachedGrid();
        
        /// <summary>
        /// Spawn new detached grid with IGridSpawner
        /// </summary>
        void SpawnNewDetachedGrid();
        
        /// <summary>
        /// Remove completed rows if there's any
        /// </summary>
        /// <returns>true if any rows were removed, false otherwise</returns>
        bool RemoveCompletedRowsIfNeeded();
    }
}