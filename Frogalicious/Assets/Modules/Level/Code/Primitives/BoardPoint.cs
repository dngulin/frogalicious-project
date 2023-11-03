namespace Frog.Level.Primitives
{
    public struct BoardPoint
    {
        public int X;
        public int Y;

        public BoardPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(in BoardPoint l, in BoardPoint r) => l.X == r.X && l.Y == r.Y;
        public static bool operator !=(in BoardPoint l, in BoardPoint r) => !(l == r);

        public override bool Equals(object obj) => obj is BoardPoint other && this == other;
        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}