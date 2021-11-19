using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Domain
{
    /// <summary>
    /// ローレイヤーなぷよの回転管理クラス
    /// enum で処理しているとキャストがえげつない回数行われてコストが酷い
    /// とはいえ、
    /// int 管理だけでやろうとすると、数値が何を表しているのかわからない
    /// enum 管理だけでやろうとすると、条件分岐の応酬になる
    /// ので爆誕した。いわゆる ValueObject。
    /// ただ、Application 層で定義された位置に依存する作りなので
    /// Domain に定義されているのが適切とは言い難い
    /// </summary>
    public class PuyoRotation
    {
        public static readonly Direction ROTATE_RIGHT = new Direction(1);
        public static readonly Direction ROTATE_LEFT = new Direction(-1);

        public static readonly Position UPPER = new Position(1);
        public static readonly Position RIGHT = new Position(2);
        public static readonly Position LOWER = new Position(3);
        public static readonly Position LEFT = new Position(4);

        private static readonly List<Position> positions = new List<Position>() { UPPER, RIGHT, LOWER, LEFT };

        public static Position GetCurrentPosition(Vector3 followee, Vector3 follower)
        {
            // 横方向が同じ
            if (followee.x == follower.x) { return followee.y > follower.y ? LOWER : UPPER; }
            else { return followee.x > follower.x ? LEFT : RIGHT; }
        }


        public static Position GetNextPosition(Direction rotateDirection, Position currentPosition)
        {
            var nextPosition = (currentPosition.Value + rotateDirection.Value) % 4;
            return nextPosition == 0 ? LEFT : positions.Find(p => p.Value == nextPosition);
        }

        public static Vector3 GetNextPosition(Vector3 followee, Position nextPosition)
        {
            return new Vector3(GetX(followee, nextPosition), GetY(followee, nextPosition), 0);
        }

        private static float GetX(Vector3 followee, Position nextPosition)
        {
            if (nextPosition.Value == UPPER.Value || nextPosition.Value == LOWER.Value)
            {
                return followee.x;
            }
            return nextPosition.Value == LEFT.Value ? followee.x - 1 : followee.x + 1;
        }

        private static float GetY(Vector3 followee, Position nextPosition)
        {
            if (nextPosition.Value == LEFT.Value || nextPosition.Value == RIGHT.Value)
            {
                return followee.y;
            }
            return nextPosition.Value == LOWER.Value ? followee.y - 1 : followee.y + 1;
        }

        public class Direction
        {
            private Dictionary<int, string> ValueNameKvp = new Dictionary<int, string>()
            {
                { 1,  "ROTATE_RIGHT" },
                { -1, "ROTATE_LEFT" }
            };

            public Direction(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public override string ToString() => ValueNameKvp[Value];
        }

        public class Position
        {
            private Dictionary<int, string> ValueNameKvp = new Dictionary<int, string>()
            {
                { 1,  "UPPER" },
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