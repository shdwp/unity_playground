using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    /// <summary>
    /// Default implementation for grid transform
    /// </summary>
    public class GridTransformImpl: IGridTransform
    {
        public int cols => _cols;
        public int rows => _rows;
        
        private Bounds _bounds;
        private int _rows;
        private int _cols;

        public void Setup(Bounds bounds, int rows, int cols)
        {
            _bounds = bounds;
            _rows = rows;
            _cols = cols;
        }

        public IEnumerable<Vector3> TransformGridToWorldPoints<T>(IPartialGrid<T> grid) where T: IEquatable<T>
        {
            return grid.Select(a => GridToWorld(a.pos));
        }
        
        public GridPosition WorldToGrid(Vector3 pos)
        {
            return WorldToGridCustom(pos, Mathf.RoundToInt, Mathf.RoundToInt);
        }

        public GridPosition WorldToGridCustom(Vector3 pos, Func<float, int> rowFunction, Func<float, int> colFunction)
        {
            // calculate world-space distance per row and column
            var perRow = (_bounds.extents.x * 2f) / _rows;
            var perCol = (_bounds.extents.y * 2f) / _cols;

            // rebase position to the bounds
            var centeredPos = pos - _bounds.min;
            
            // calculate resulting positions using the functions provided
            return new GridPosition
            {
                row = rowFunction(centeredPos.x / perRow),
                col = colFunction(_cols - centeredPos.y / perCol),
            };
        }

        public Vector3 GridToWorld(GridPosition pos)
        {
            return GridToWorld(pos.row, pos.col);
        }

        public Vector3 GridToWorld(float row, float col)
        {
            return new Vector3(
                Mathf.Lerp(_bounds.min.x, _bounds.max.x, Mathf.InverseLerp(0, _rows, row)),
                Mathf.Lerp(_bounds.max.y, _bounds.min.y, Mathf.InverseLerp(0, _cols, col)),
                _bounds.center.z
            );
        }

        public Vector3 GridWorldCentroid<T>(IPartialGrid<T> grid) where T : IEquatable<T>
        {
            return GridToWorld((float)grid.pos.row + grid.rows / 2f, (float)grid.pos.col + grid.cols / 2f);
        }

        public bool IsGridInBounds<T>(IPartialGrid<T> grid) where T : IEquatable<T>
        {
            foreach (var cell in grid)
            {
                if (cell.pos.row < 0 || cell.pos.row >= _rows || cell.pos.col < 0 || cell.pos.col >= _cols)
                {
                    return false;
                }
            }

            return true;
        }
    }
}