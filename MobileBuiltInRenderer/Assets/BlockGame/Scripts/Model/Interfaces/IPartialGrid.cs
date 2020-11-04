using System;
using System.Collections.Generic;

namespace BlockGame.Scripts.Model.Interfaces
{
    /// <summary>
    /// Class that represents a partial game grid. It contains number of filled or unfilled cells, always rectangular in shape,
    /// positioned in the bounds of the grid transform.
    ///
    /// Filled cells will return their respective TData instances, where unfilled cells will be of `default(TData)`.
    ///
    /// Currently one partial grid is used for the blocks on the ground, and another is used for the tetrominoe that is currently falling.
    /// </summary>
    /// <typeparam name="TData">data class to use for each cell</typeparam>
    public interface IPartialGrid<TData>: IEnumerable<GridCell<TData>> where TData: IEquatable<TData>
    {
        /// <summary>
        /// Position of this grid on the grid transform
        /// </summary>
        GridPosition pos { get; }
        
        /// <summary>
        /// Amount of rows
        /// </summary>
        int rows { get; }
        
        /// <summary>
        /// Amount of cols
        /// </summary>
        int cols { get; }

        /// <summary>
        /// Setup current instance with 3x3 grid based on string scheme format (total 9 characters, no newlines).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        void Setup3x3(TData data, string format);
        
        /// <summary>
        /// Setup empty grid with size matching grid transform (full field grid)
        /// </summary>
        void SetupEmptyFromTransform();

        /// <summary>
        /// Check whether specific grid-space position in occupied in this grid.
        /// Position is provide in grid-space, meaning that this grid's position on the grid transform
        /// will be applied.
        ///
        /// Should not be called with negative grid position, but may be called with grid position
        /// greater that size of this grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsPositionOccupied(GridPosition pos);
        
        /// <summary>
        /// Check for intergrid collisions.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if grids overlap</returns>
        bool DoesCollideWith(IPartialGrid<TData> other);

        /// <summary>
        /// Rebase this grid so it's origin matches provided grid position.
        /// </summary>
        /// <param name="pos"></param>
        void Rebase(GridPosition pos);
        
        /// <summary>
        /// Translate grid (shift position). Clamps resulting grid to transform.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        void Translate(int y, int x);
        
        /// <summary>
        /// Clamp grid so it doesn't go out of bounds of transform
        /// </summary>
        void ClampToTransform();
        
        /// <summary>
        /// Remove specific column from the grid
        /// </summary>
        /// <param name="colToRemove"></param>
        void RemoveColumn(int colToRemove);
        
        /// <summary>
        /// Merge this grid with cells from other grid
        /// </summary>
        /// <param name="other"></param>
        void Merge(IPartialGrid<TData> other);

        IEnumerable<bool> EnumerateOccupancyOverRow(int colIdx);

        /// <summary>
        /// Store this grid's data in out variables.
        /// @TODO: replace with instance serialization
        /// </summary>
        /// <param name="array"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="pos"></param>
        void StoreData(out TData[] array, out int rows, out int cols, out GridPosition pos);
        
        /// <summary>
        /// Restore this grid's data with information from variables.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="pos"></param>
        void RestoreData(TData[] array, int rows, int cols, GridPosition pos);
    }
}