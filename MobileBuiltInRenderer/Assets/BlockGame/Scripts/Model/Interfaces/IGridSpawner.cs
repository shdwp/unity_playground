using System;

namespace BlockGame.Scripts.Model.Interfaces
{
    public interface IGridSpawner<TData> where TData: IEquatable<TData>
    {
        IPartialGrid<TData> SpawnRandomGrid();
    }
}