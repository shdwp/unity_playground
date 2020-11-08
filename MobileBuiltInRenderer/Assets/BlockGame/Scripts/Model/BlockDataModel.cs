using System;

namespace BlockGame.Scripts.Model
{
    public struct BlockDataModel: IEquatable<BlockDataModel>
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

        public BlockDataModel(Color color)
        {
            this.color = color;
        }

        public bool Equals(BlockDataModel other)
        {
            return color == other.color;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockDataModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)color;
        }
    }
}