using thegame.Infrastructure.Common;

namespace thegame.Models
{
    public class Vec
    {
        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X, Y;

        public Vec GetNextPosition(Direction direction)
        {
            Vec vec = new Vec(X, Y);
            switch (direction)
            {
                case Direction.Up:
                    vec.Y -= 1;
                    break;
                case Direction.Left:
                    vec.X -= 1;
                    break;
                case Direction.Right:
                    vec.X += 1;
                    break;
                case Direction.Down:
                    vec.Y += 1;
                    break;
            }

            return vec;
        }

        public Vec Clone() => new Vec(X, Y);

        public override bool Equals(object obj) => Equals(obj as Vec);

        bool Equals(Vec secondVec) => X == secondVec.X && Y == secondVec.Y;

        public override string ToString() => $"{{X: {X} | Y: {Y}}}";
    }
}