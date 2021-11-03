using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.UI {
    public interface ISkeltonColliderCollection {
        void ToLeft();
        void ToRight();
        void ToDown();
    }
    public class SkeltonColliderCollection : ISkeltonColliderCollection
    {
        private enum Direction {
            UpperLeft,
            MiddleLeft,
            LowerLeft,
            LowerRight,
            MiddleRight,
            UpperRight
        }
        private Dictionary<int, Vector3> OFFSETS = new Dictionary<int, Vector3>()
        {
            { (int)Direction.UpperLeft, new Vector3(-1, 1, 0) },
            { (int)Direction.MiddleLeft, new Vector3(-1, 0, 0) },
            { (int)Direction.LowerLeft, new Vector3(-1, -1, 0) },
            { (int)Direction.LowerRight, new Vector3(1, -1, 0) },
            { (int)Direction.MiddleRight, new Vector3(1, 0, 0) },
            { (int)Direction.UpperRight, new Vector3(1, 1, 0) }
        };
        private Transform fieldTransform;
        private Vector3 landmarkPosition;
        private Dictionary<int, SkeltonCollider> skeltonColliders = new Dictionary<int, SkeltonCollider>();

        public SkeltonColliderCollection(Transform fieldTransform, Vector3 landmarkPosition)
        {
            foreach (var kvp in OFFSETS)
            {
                skeltonColliders.Add(kvp.Key, Application.SkeltonColliderGenerator.Instance.Generate(fieldTransform, landmarkPosition + kvp.Value));
            }
        }

        private Direction ToDirection(int key)
        {
            return (Direction)Enum.ToObject(typeof(Direction), key);
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

        public void ToJustTouch()
        {
            foreach (var kvp in skeltonColliders)
            {
                UnityEngine.Debug.Log(ToDirection(kvp.Key).ToString());
                kvp.Value.ToJustTouch();
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
            skeltonColliders = new Dictionary<int, SkeltonCollider>();
        }
    }
}
