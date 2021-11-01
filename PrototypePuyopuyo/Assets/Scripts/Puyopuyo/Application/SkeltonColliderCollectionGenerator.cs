using com.amabie.SingletonKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Puyopuyo.Application {
    public class SkeltonColliderCollectionGenerator : SingletonMonoBehaviour<SkeltonColliderCollectionGenerator>
    {/*
        private Dictionary<Direction, Vector3> positionOffsetDict;
        private Dictionary<Direction, Puyopuyo.UI.SkeltonCollider> skeltonColliderDict;

        public void Generate(Transform parentTransform, Vector3 position)
        {
            Initialize();
            skeltonColliderDict = new Dictionary<Direction, Puyopuyo.UI.SkeltonCollider>() {};
            foreach (var kvp in positionOffsetDict)
            {
                skeltonColliderDict.Add(kvp.Key, SkeltonColliderGenerator.Instance.Generate(parentTransform, position + kvp.Value));
            }
        }

        public void DisposeCollection()
        {
            foreach (var kvp in skeltonColliderDict)
            {
                Destroy(kvp.Value.gameObject);
            }
            positionOffsetDict = null;
            skeltonColliderDict = null;
        }

        public bool HasCollision(List<Direction> directionList)
        {
            var hasCollision = false;
            foreach (var direction in directionList)
            {
                if (skeltonColliderDict[direction].HasCollision) { hasCollision = true; }
            }
            return hasCollision;
        }

        private void Initialize()
        {
            positionOffsetDict = new Dictionary<Direction, Vector3>()
            {
                { Direction.UpperLeft, new Vector3(-1, 1, 0) },
                { Direction.MiddleLeft, new Vector3(-1, 0, 0) },
                { Direction.LowerLeft, new Vector3(-1, -1, 0) },
                { Direction.LowerCenter, new Vector3(0, -1, 0) },
                { Direction.LowerRight, new Vector3(1, -1, 0) },
                { Direction.MiddleRight, new Vector3(1, 0, 0) },
                { Direction.UpperRight, new Vector3(1, 1, 0) }
            };
        }*/
    }
}
