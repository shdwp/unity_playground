using System.Collections.Generic;

namespace BlockGame.Scripts.Model.Interfaces
{
    public enum GridRotationDirection
    {
        CW, CCW
    }
    
    public interface IPartialGrid: IEnumerable<GridPosition>
    {
        GridPosition pos { get; }

        bool IsPositionOccupied(GridPosition pos);
        bool DoesCollideWith(IPartialGrid other);
        
        void Setup3x3(string format);
        void SetupFloor();
        void Translate(int y, int x);
        void Merge(IPartialGrid other);
    }
}