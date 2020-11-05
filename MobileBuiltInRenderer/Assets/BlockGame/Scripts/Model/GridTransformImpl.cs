using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
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
            return new GridPosition
            {
                row = Mathf.RoundToInt(Mathf.InverseLerp(_bounds.max.y, _bounds.min.y, pos.y) * _rows),
                col = Mathf.RoundToInt(Mathf.InverseLerp(_bounds.min.x, _bounds.max.x, pos.x) * _cols),
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
                Mathf.Lerp(_bounds.max.y, _bounds.min.y, Mathf.InverseLerp(0, _cols, col))
            );
        }

        public Vector3 GridWorldCentroid<T>(IPartialGrid<T> grid) where T : IEquatable<T>
        {
            return GridToWorld((float) grid.rows / 2f, (float) grid.cols / 2f);
        }

        public GridPosition Clamp<T>(GridPosition pos, T[,] data) where T: IEquatable<T>
        {
            var minRow = Int32.MinValue;
            var minCol = Int32.MinValue;
            var maxRow = Int32.MaxValue;
            var maxCol = Int32.MaxValue;
            
            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    if (!data[row, col].Equals(default))
                    {
                        minCol = Math.Max(minRow, -col);
                        maxCol = Math.Min(maxCol, -col);

                        minRow = Math.Max(minRow, -row);
                        maxRow = Math.Min(maxRow, -row);
                    }
                }
            }
            
            pos.row = Mathf.Clamp(pos.row, minRow, _rows - maxRow);
            pos.col = Mathf.Clamp(pos.col, minCol, _cols - maxCol);
            return pos;
        }
    }
}