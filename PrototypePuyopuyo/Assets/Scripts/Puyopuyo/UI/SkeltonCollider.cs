using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision => HitGameObjectInstanceIds.Count != 0;
        public List<int> HitGameObjectInstanceIds { get; private set; }
        public IPuyo TargetPuyo { get; private set; }

        protected new void Awake()
        {
            base.Awake();
            HitGameObjectInstanceIds = new List<int>();
        }

        public void RecognizeTarget(IPuyo targetPuyo)
        {
            this.TargetPuyo = targetPuyo;
        }

        private bool IsSkelton(GameObject gameObj)
        {
            return gameObj.GetComponent<SkeltonCollider>() != null;
        }

        private bool IsTarget(GameObject gameObj)
        {
            return ReferenceEquals(TargetPuyo.GameObject, gameObj);
        }

        private bool IsPartner(GameObject gameObj)
        {
            return ReferenceEquals(TargetPuyo.Partner.GameObject, gameObj);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (IsSkelton(collider.gameObject)) { return; }
            if (IsTarget(collider.gameObject)) { return; }
            if (IsPartner(collider.gameObject)) { return; }
            if (!HitGameObjectInstanceIds.Contains(collider.gameObject.GetInstanceID()))
            {
                HitGameObjectInstanceIds.Add(collider.gameObject.GetInstanceID());
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (!HitGameObjectInstanceIds.Contains(collider.gameObject.GetInstanceID()))
            {
                HitGameObjectInstanceIds.Remove(collider.gameObject.GetInstanceID());
            }
        }
    }
}