using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Scripts.Model.Interfaces
{
    public struct GridPosition
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
    }

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
    
    public interface IGridTransform
    {
        int cols { get; }
        int rows { get; }
        
        GridPosition WorldToGrid(Vector3 pos);
        Vector3 GridToWorld(GridPosition pos);

        Vector3 GridWorldCentroid<T>(IPartialGrid<T> grid) where T: IEquatable<T>;

        IEnumerable<Vector3> TransformGridToWorldPoints<T>(IPartialGrid<T> grid) where T: IEquatable<T>;

        GridPosition Clamp<T>(GridPosition pos, T[,] data) where T : IEquatable<T>;

        void Setup(Bounds bounds, int rows, int cols);
    }
}