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
    
    public interface IGridTransform
    {
        int cols { get; }
        int rows { get; }
        
        GridPosition WorldToGrid(Vector3 pos);
        Vector3 GridToWorld(GridPosition pos);

        IEnumerable<Vector3> TransformGridToWorldPoints(IPartialGrid grid);

        GridPosition Clamp(GridPosition pos, int[,] data);

        void Setup(Bounds bounds, int rows, int cols);
    }
}