namespace Frog.Level.Primitives
{
    public readonly struct BoardPoint
    {
        public static BoardPoint Up => new BoardPoint(0, 1);
        public static BoardPoint Right => new BoardPoint(1, 0);
        public static BoardPoint Down => new BoardPoint(0, -1);
        public static BoardPoint Left => new BoardPoint(-1, 0);

        public readonly int X;
        public readonly int Y;

        public BoardPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static BoardPoint operator +(in BoardPoint l, in BoardPoint r) => new BoardPoint(l.X + r.X, l.Y + r.Y);
        public static BoardPoint operator -(in BoardPoint l, in BoardPoint r) => new BoardPoint(l.X - r.X, l.Y - r.Y);

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