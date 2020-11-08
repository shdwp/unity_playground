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
            _pos = new GridPosition(0, 0);
            
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

        public void SetupEmptyFromTransform()
        {
            _data = new TData[transform.rows, transform.cols];
            _rows = transform.rows;
            _cols = transform.cols;
            _pos = new GridPosition(0, 0);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    _data[row, col] = default;
                }
            }
        }
        
        public bool IsPositionOccupied(GridPosition pos)
        {
            var data = default(TData);
            if (pos.row < _data.GetLength(0) && pos.col < _data.GetLength(1))
            {
                return !EqualityComparer<TData>.Default.Equals(_data[pos.row, pos.col], default);
            }
            else if (pos.row >= 0 && pos.col >= 0)
            {
                return false;
            }
            else
            {
                throw new ArgumentOutOfRangeException("pos", "called with negative GridPosition");
            }
        }

        public bool DoesCollideWith(IPartialGrid<TData> other)
        {
            foreach (var cell in other)
            {
                if (IsPositionOccupied(cell.pos))
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
        
        public void Translate(int x, int y)
        {
            _pos.row += x;
            _pos.col += y;

            _pos = transform.Clamp(_pos, _data);
        }

        public void Rebase(GridPosition pos)
        {
            _pos = pos;
        }

        public void Rotate(GridRotationDirection dir)
        {
        }

        public void RemoveCol(int colToRemove)
        {
            int newColIdx;
            var newData = new TData[transform.rows, transform.cols];
            
            for (int row = 0; row < _rows; row++)
            {
                newColIdx = _cols - 1;
                for (int col = _cols - 1; col >= 0; col--)
                {
                    if (col != colToRemove)
                    {
                        newData[row, newColIdx] = _data[row, col];
                        newColIdx--;
                    }
                }
            }

            _data = newData;
        }

        public void StoreData(out TData[] array, out int rows, out int cols, out GridPosition pos)
        {
            rows = _rows;
            cols = _cols;
            pos = _pos;
            
            array = new TData[_cols * _rows];
            
            var i = 0;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    array[i] = _data[row, col];
                    i++;
                }
            }
        }

        public void RestoreData(TData[] array, int rows, int cols, GridPosition pos)
        {
            _data = new TData[rows, cols];
            _rows = rows;
            _cols = cols;
            _pos = pos;

            var i = 0;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    _data[row, col] = array[i];
                    i++;
                }
            }
        }

        public IEnumerator<GridCell<TData>> GetEnumerator()
        {
            for (int col = 0; col < _cols; col++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    var data = _data[row, col];
                    if (!EqualityComparer<TData>.Default.Equals(data, default))
                    {
                        yield return new GridCell<TData>(row + pos.row, col + pos.col, data);
                    }
                }
            }
        }

        public IEnumerable<bool> EnumerateOccupancyOverRow(int colIdx)
        {
            for (int row = 0; row < _rows; row++)
            {
                yield return !EqualityComparer<TData>.Default.Equals(_data[row, colIdx], default);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}