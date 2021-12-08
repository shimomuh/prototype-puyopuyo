using System.Collections.Generic;

namespace Puyopuyo.Domain
{
    public class PuyoPosition
    {
        public static readonly Position UPPER = new Position(1);
        public static readonly Position RIGHT = new Position(2);
        public static readonly Position LOWER = new Position(3);
        public static readonly Position LEFT = new Position(4);


        public class Position
        {
            private Dictionary<int, string> ValueNameKvp = new Dictionary<int, string>()
            {
                { 1, "UPPER" },
                { 2, "RIGHT" },
                { 3, "LOWER" },
                { 4, "LEFT" }
            };

            public Position(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public override string ToString() => ValueNameKvp[Value];
        }
    }
}