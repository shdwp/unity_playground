using System;
using BlockGame.Scripts.Contexts;

namespace BlockGame.Scripts.Model.Interfaces
{
    /// <summary>
    /// Type of a grid spawner, usually follows specific classes
    /// </summary>
    public enum GridSpawnerType
    {
        GrabBag,
        SpecificFigures,
        TrueRandom
    }
    
    /// <summary>
    /// Tetrominoe spawner, provides grid instances with random contents to be used in the game
    /// </summary>
    /// <typeparam name="TData">data class for per-cell data</typeparam>
    public interface IGridSpawner<TData> where TData: IEquatable<TData>
    {
        /// <summary>
        /// This instance type (used to determine which class will be used when persistency comes into play)
        /// </summary>
        GridSpawnerType type { get; }
        
        /// <summary>
        /// Instantiate and fill a random tetrominoe
        /// </summary>
        /// <returns></returns>
        IPartialGrid<TData> SpawnRandomGrid();
    }
}