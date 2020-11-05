﻿using System;
using System.Collections.Generic;

namespace BlockGame.Scripts.Model.Interfaces
{
    public enum GridRotationDirection
    {
        CW, CCW
    }
    
    public interface IPartialGrid<TData>: IEnumerable<GridCell<TData>> where TData: IEquatable<TData>
    {
        GridPosition pos { get; }
        int rows { get; }
        int cols { get; }

        bool IsPositionOccupied(GridPosition pos);
        bool DoesCollideWith(IPartialGrid<TData> other);

        void Setup3x3(TData data, string format);
        void SetupFullFieldWithFloor(TData data);

        void Rebase(GridPosition pos);
        void Translate(int y, int x);
        
        void Merge(IPartialGrid<TData> other);
    }
}