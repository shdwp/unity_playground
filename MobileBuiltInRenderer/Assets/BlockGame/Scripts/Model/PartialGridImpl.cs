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
                    // calculate format character position for this cell
                    var ch = format[(col * 3) + row];
                    
                    if (ch != ' ')
                    {
                        // character is filled - setup the data to this specific cell
                        _data[row, col] = data;
                    }
                    
                    // otherwise cell data remains as `default(TData)`
                }
            }
        }

        public void SetupEmptyFromTransform()
        {
            _data = new TData[transform.rows, transform.cols];
            _rows = transform.rows;
            _cols = transform.cols;
            _pos = new GridPosition(0, 0);
        }
        
        public bool IsPositionOccupied(GridPosition position)
        {
            if (position.row < _data.GetLength(0) && position.col < _data.GetLength(1))
            {
                return !EqualityComparer<TData>.Default.Equals(_data[position.row, position.col], default);
            }
            else if (position.row >= 0 && position.col >= 0)
            {
                return false;
            }
            else
            {
                throw new ArgumentOutOfRangeException("position", "called with negative GridPosition");
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

            ClampToTransform();
        }

        public void Rebase(GridPosition pos)
        {
            _pos = pos;
        }

        public void ClampToTransform()
        {
            var minRow = Int32.MinValue;
            var minCol = Int32.MinValue;
            var maxRow = Int32.MaxValue;
            var maxCol = Int32.MaxValue;
            
            // iterate over filled cells and figure out extents of the grid contents
            // those will later be used as an additional constrains while position clamping
            for (int row = 0; row < _data.GetLength(0); row++)
            {
                for (int col = 0; col < _data.GetLength(1); col++)
                {
                    if (!_data[row, col].Equals(default))
                    {
                        minCol = Math.Max(minRow, -col);
                        maxCol = Math.Min(maxCol, -col);

                        minRow = Math.Max(minRow, -row);
                        maxRow = Math.Min(maxRow, -row);
                    }
                }
            }
            
            // clamp to new position used transform size and calculated extents
            _pos = new GridPosition(
                Mathf.Clamp(pos.row, minRow, transform.rows - maxRow),
                Mathf.Clamp(pos.col, minCol, transform.cols - maxCol)
            );
        }

        public void RemoveColumn(int colToRemove)
        {
            var newData = new TData[transform.rows, transform.cols];
            
            int newColIdx;
            for (int row = 0; row < _rows; row++)
            {
                newColIdx = _cols - 1;
                
                // iterate columns backwards in so that the gap will be closed using the top and not the bottom
                // (top falls down, contrary to bottom goes up)
                for (int col = _cols - 1; col >= 0; col--)
                {
                    if (col != colToRemove)
                    {
                        // copy each column that is not intended to be removed to the new array
                        newData[row, newColIdx] = _data[row, col];
                        newColIdx--;
                    }
                    
                    // while the column to be removed will be skipped
                }
            }

            _data = newData;
        }

        public void StoreData(out TData[] array, out int rows, out int cols, out GridPosition pos)
        {
            // store simple vars
            rows = _rows;
            cols = _cols;
            pos = _pos;
            
            // alloc array for cells
            array = new TData[_cols * _rows];
            
            // store cells in one-dimensional array
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
            // alloc two-dimensional array and setup variables
            _data = new TData[rows, cols];
            _rows = rows;
            _cols = cols;
            _pos = pos;

            // map input one-dimensional array to data array
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
                        // enumerator only yields filled cells
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