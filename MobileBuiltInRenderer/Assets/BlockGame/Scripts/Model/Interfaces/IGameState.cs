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

        IPartialGrid<BlockDataModel> TestAndApplyDetachedGridCollisions();
    }
}