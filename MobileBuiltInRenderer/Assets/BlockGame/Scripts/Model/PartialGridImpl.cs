using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Model
{
    public class PartialGridImpl: IPartialGrid
    {
        public GridPosition pos => _pos;
        
        [Inject] public IGridTransform transform { get; set; }
        
        private GridPosition _pos;
        private int _rows, _cols;
        private int[,] _data;
        
        public PartialGridImpl()
        {
        }

        public void Setup3x3(string format)
        {
            _data = new int[3,3];
            _rows = 3;
            _cols = 3;
            
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var ch = format[(col * 3) + row];
                    if (ch != ' ')
                    {
                        _data[row, col] = 1;
                    }
                }
            }
        }

        public void SetupFloor()
        {
            _data = new int[transform.rows, 1];
            _rows = transform.rows;
            _cols = 1;

            for (int row = 0; row < _rows; row++)
            {
                _data[row, 0] = 1;
            }
            
            _pos = new GridPosition(0, _rows - 1);
        }

        public bool IsPositionOccupied(GridPosition pos)
        {
            return _data[pos.row, pos.col] > 0;
        }

        public bool DoesCollideWith(IPartialGrid other)
        {
            foreach (var occupied in this)
            {
                if (other.IsPositionOccupied(occupied))
                {
                    return true;
                }
            }

            return false;
        }

        public void Merge(IPartialGrid other)
        {
            foreach (var pos in other)
            {
                _data[pos.row, pos.col] = 1;
            }
        }
        
        public void Translate(int y, int x)
        {
            _pos.col += y;
            _pos.row += x;

            _pos = transform.Clamp(_pos, _data);
        }

        public void Rotate(GridRotationDirection dir)
        {
        }

        public IEnumerator<GridPosition> GetEnumerator()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (_data[row, col] > 0)
                    {
                        yield return new GridPosition(row + pos.row, col + pos.row);
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