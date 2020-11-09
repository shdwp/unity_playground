using System;

namespace BlockGame.Scripts.Model
{
    /// <summary>
    /// Data class for cell-specific information.
    /// Currently only holds color information.
    /// </summary>
    [Serializable]
    public struct CellDataModel: IEquatable<CellDataModel>
    {
        public enum Color
        {
            Empty,
            Red,
            Green,
            Yellow,
            Orange,
            Blue,
            Cyan,
            Purple,
        }

        public Color color;
        public static Color[] ALL_COLORS = {
            Color.Red,
            Color.Green,
            Color.Yellow,
            Color.Orange,
            Color.Blue,
            Color.Cyan,
            Color.Purple
        };

        public CellDataModel(Color color)
        {
            this.color = color;
        }

        public bool Equals(CellDataModel other)
        {
            return color == other.color;
        }

        public override bool Equals(object obj)
        {
            return obj is CellDataModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)color;
        }
    }
}