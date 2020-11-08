using System;

namespace BlockGame.Scripts.Model.Interfaces
{
    public enum GridType
    {
        Attached,
        Detached
    }
        
    public interface IGameState
    {
        IPartialGrid<BlockDataModel> attachedGrid { get; set; }
        IPartialGrid<BlockDataModel> detachedGrid { get; set; }

        void SetupInitialAttachedGrid();
        bool TestGridsCollision();
        void MergeDetachedGrid();
        void SpawnNewDetachedGrid();
        bool RemoveCompletedRowsIfNeeded();
    }
}