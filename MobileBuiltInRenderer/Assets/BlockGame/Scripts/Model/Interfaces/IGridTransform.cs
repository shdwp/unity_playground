using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Scripts.Model.Interfaces
{
    /// <summary>
    /// Struct that represents position on the grid, i.e. row and column information
    /// </summary>
    [Serializable]
    public struct GridPosition: IEquatable<GridPosition>
    {
        public int row, col;

        public GridPosition(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public override string ToString()
        {
            return $"GridPosition {{row {row}, cell {col}}}";
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.row + b.row, a.col + b.col);
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !(a == b);
        }

        public bool Equals(GridPosition other)
        {
            return row == other.row && col == other.col;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (row * 397) ^ col;
            }
        }
    }

    /// <summary>
    /// Single grid cell representation, provides both position (GridPosition) and instance-specific data (generic)
    /// </summary>
    /// <typeparam name="DataT"></typeparam>
    public struct GridCell<DataT>
    {
        public GridPosition pos;
        public DataT data;

        public GridCell(int row, int col, DataT data)
        {
            pos = new GridPosition(row, col);
            this.data = data;
        }
    }
    
    /// <summary>
    /// Class that transforms world-space coordinates to grid positions.
    /// Setup should be performed in order to determine world-space bounds and grid size.
    /// </summary>
    public interface IGridTransform
    {
        /// <summary>
        /// Amount of columns
        /// </summary>
        int cols { get; }
        
        /// <summary>
        /// Amount of rows
        /// </summary>
        int rows { get; }

        /// <summary>
        /// Setup grid transform using world-space bounds and grid size
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        void Setup(Bounds bounds, int rows, int cols);
        
        /// <summary>
        /// Transform from world-space to grid-space.
        /// Can return values outside of the grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        GridPosition WorldToGrid(Vector3 pos);
        
        /// <summary>
        /// Transform from world-space to grid-space, using custom rounding functions for both components.
        /// Can return values outside of the grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rowFunction">rounding function like Math.Round</param>
        /// <param name="colFunction">rounding function like Math.Round</param>
        /// <returns></returns>
        GridPosition WorldToGridCustom(Vector3 pos, Func<float, int> rowFunction, Func<float, int> colFunction);
        
        /// <summary>
        /// Transform from grid-space to world-space.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vector3 GridToWorld(GridPosition pos);

        /// <summary>
        /// Calculate and return partial grid centroid in world-space.
        /// </summary>
        /// <param name="grid"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Vector3 GridWorldCentroid<T>(IPartialGrid<T> grid) where T: IEquatable<T>;

        /// <summary>
        /// Map partial grid to enumerable of world-space points, one for each filled cell
        /// </summary>
        /// <param name="grid"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<Vector3> TransformGridToWorldPoints<T>(IPartialGrid<T> grid) where T: IEquatable<T>;
        
        /// <summary>
        /// Check whether partial grid is in bounds for this transform
        /// </summary>
        /// <param name="grid"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IsGridInBounds<T>(IPartialGrid<T> grid) where T: IEquatable<T>;
    }
}