using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.UI {
    public interface ISkeltonColliderCollection {
        bool CanToLeft();
        bool CanToRight();
        bool CanToDown();
        bool HasCollisionAtLowerLeft();
        bool HasCollisionAtLowerRight();
        void ToLeft();
        void ToRight();
        void ToDown();
        void ForceMove(Vector3 position);
        void LerpRotate(Vector3 position);
        void Stop();
        void Restart();
        void ToFall();
        void ToJustTouch();
        void ToCancelTouching();
        void TryToKeepTouching();
        void ToJustStay();
        void ToStay();
        void Dispose();
        float HeightBetweenClosestPoint();
        void ForceChangeState();
    }
    public class SkeltonColliderCollection : ISkeltonColliderCollection
    {
        private enum Direction {
            UpperLeft,
            LowerLeft,
            Lower,
            LowerRight,
            UpperRight
        }
        private Dictionary<int, Vector3> OFFSETS = new Dictionary<int, Vector3>()
        {
            { (int)Direction.UpperLeft, new Vector3(-1, 0, 0) },
            { (int)Direction.LowerLeft, new Vector3(-1, -1, 0) },
            { (int)Direction.Lower, new Vector3(0, -1, 0) },
            { (int)Direction.LowerRight, new Vector3(1, -1, 0) },
            { (int)Direction.UpperRight, new Vector3(1, 0, 0) }
        };
        private Dictionary<int, SkeltonCollider> skeltonColliders = new Dictionary<int, SkeltonCollider>();

        public SkeltonColliderCollection(Transform fieldTransform, Vector3 landmarkPosition, Puyo targetPuyo)
        {
            foreach (var kvp in OFFSETS)
            {
                skeltonColliders.Add(kvp.Key, Application.SkeltonColliderGenerator.Instance.Generate(fieldTransform, landmarkPosition + kvp.Value, targetPuyo));
            }
        }

        private Direction ToDirection(int key)
        {
            return (Direction)Enum.ToObject(typeof(Direction), key);
        }

        public bool CanToLeft()
        {
            return !HasCollision(Direction.UpperLeft);
        }

        public bool CanToRight()
        {
            return !HasCollision(Direction.UpperRight);
        }

        public bool CanToDown()
        {
            bool hasCollision = false;
            foreach (var kvp in skeltonColliders)
            {
                if (ToDirection(kvp.Key) == Direction.UpperLeft || ToDirection(kvp.Key) == Direction.UpperRight) { continue; }
                if (kvp.Value.HasCollision) { hasCollision = true; }
            }
            return !hasCollision;
        }

        public bool HasCollisionAtLowerLeft()
        {
            return HasCollision(Direction.LowerLeft);
        }

        public bool HasCollisionAtLowerRight()
        {
            return HasCollision(Direction.LowerRight);
        }

        private bool HasCollision(Direction targetDirection)
        {
            bool hasCollision = false;
            foreach (var kvp in skeltonColliders)
            {
                if (ToDirection(kvp.Key) != targetDirection) { continue; }
                if (kvp.Value.HasCollision)
                {
                    if (kvp.Value.HitColliders.Exists(c => c.gameObject.GetInstanceID() == kvp.Value.TargetPuyo.Partner.GameObject.GetInstanceID())) { continue; }
                    hasCollision = true;
                }
            }
            return hasCollision;
        }

        public void ToLeft()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToLeft();
            }
        }

        public void ToRight()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToRight();
            }
        }

        public void ToDown()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToDown();
            }
        }

        public void ForceMove(Vector3 position)
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ForceMove(position + OFFSETS[kvp.Key]);
            }
        }

        public void LerpRotate(Vector3 position)
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ForceMove(OFFSETS[kvp.Key] + position);
            }
        }

        public void ToFall()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToFall();
            }
        }

        public void Stop()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.Stop();
            }
        }

        public void Restart()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.Restart();
            }
        }

        public void ToJustTouch()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToJustTouch();
            }
        }

        public void ToCancelTouching()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToCancelTouching();
            }
        }

        public void TryToKeepTouching()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.TryToKeepTouching();
            }
        }

        public void ToJustStay()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToJustStay();
            }
        }

        public void ToStay()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ToStay();
            }
        }

        public float HeightBetweenClosestPoint()
        {
            foreach (var kvp in skeltonColliders)
            {
                if (ToDirection(kvp.Key) != Direction.Lower) { continue; }
                if (!HasCollision(Direction.Lower)) { continue; }
                return kvp.Value.HeightBetweenClosestPoint();
            }
            return 0;
        }

        public void ForceChangeState()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.ForceChangeState();
            }
        }

        public void Dispose()
        {
            foreach (var kvp in skeltonColliders)
            {
                kvp.Value.Destroy();
            }
            skeltonColliders = null;
        }
    }
}