using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.UI {
    public interface ISkeltonColliderCollection {
        bool CanToLeft();
        bool CanToRight();
        bool CanToDown();
        void ToLeft();
        void ToRight();
        void ToDown();
        void ForceMove(Vector3 position);
        void LerpRotate(Vector3 position);
        void ToFall();
        void ToJustTouch();
        void ToCancelTouching();
        void TryToKeepTouching();
        void ToJustStay();
        void ToStay();
        void Dispose();
    }
    public class SkeltonColliderCollection : ISkeltonColliderCollection
    {
        private enum Direction {
            UpperLeft,
            LowerLeft,
            LowerRight,
            UpperRight
        }
        private Dictionary<int, Vector3> OFFSETS = new Dictionary<int, Vector3>()
        {
            { (int)Direction.UpperLeft, new Vector3(-1, 0, 0) },
            { (int)Direction.LowerLeft, new Vector3(-1, -1, 0) },
            { (int)Direction.LowerRight, new Vector3(1, -1, 0) },
            { (int)Direction.UpperRight, new Vector3(1, 0, 0) }
        };
        private Transform fieldTransform;
        private Vector3 landmarkPosition;
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
            bool hasCollision = false;
            foreach (var kvp in skeltonColliders)
            {
                if (ToDirection(kvp.Key) != Direction.UpperLeft) { continue; }
                if (kvp.Value.HasCollision) { hasCollision = true; }
            }
            return !hasCollision;
        }

        public bool CanToRight()
        {
            bool hasCollision = false;
            foreach (var kvp in skeltonColliders)
            {
                if (ToDirection(kvp.Key) != Direction.UpperRight) { continue; }
                if (kvp.Value.HasCollision) { hasCollision = true; }
            }
            return !hasCollision;
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
                kvp.Value.ForceMove(position);
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
