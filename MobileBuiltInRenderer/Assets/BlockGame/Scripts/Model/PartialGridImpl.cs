using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    public class PartialGridImpl<TData>: IPartialGrid<TData> where TData: IEquatable<TData>
    {
        public GridPosition pos => _pos;

        public int rows => _rows;
        public int cols => _cols;

        [Inject] public IGridTransform transform { get; set; }
        
        private GridPosition _pos;
        private int _rows, _cols;
        private TData[,] _data;

        public void Setup3x3(TData data, string format)
        {
            _data = new TData[3,3];
            _rows = 3;
            _cols = 3;
            
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var ch = format[(col * 3) + row];
                    if (ch != ' ')
                    {
                        _data[row, col] = data;
                    }
                }
            }
        }

        public void SetupFullFieldWithFloor(TData data)
        {
            _data = new TData[transform.rows, transform.cols];
            _rows = transform.rows;
            _cols = transform.cols;
            _pos = new GridPosition(0, 0);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (col == _cols - 1)
                    {
                        _data[row, col] = data;
                    }
                    else
                    {
                        _data[row, col] = default;
                    }
                }
            }
        }

        public bool IsPositionOccupied(GridPosition pos)
        {
            return !_data[pos.row, pos.col].Equals(default);
        }

        public bool DoesCollideWith(IPartialGrid<TData> other)
        {
            foreach (var cell in this)
            {
                if (other.IsPositionOccupied(cell.pos))
                {
                    return true;
                }
            }

            return false;
        }

        public void Merge(IPartialGrid<TData> other)
        {
            foreach (var cell in other)
            {
                _data[cell.pos.row, cell.pos.col] = cell.data;
            }
        }
        
        public void Translate(int y, int x)
        {
            _pos.col += y;
            _pos.row += x;

            _pos = transform.Clamp(_pos, _data);
        }

        public void Rebase(GridPosition pos)
        {
            _pos = pos;
        }

        public void Rotate(GridRotationDirection dir)
        {
        }

        public IEnumerator<GridCell<TData>> GetEnumerator()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    var data = _data[row, col];
                    if (!data.Equals(default))
                    {
                        yield return new GridCell<TData>(row + pos.row, col + pos.row, data);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}