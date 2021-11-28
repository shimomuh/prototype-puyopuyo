using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision => HitColliders.Count != 0;
        public List<Collider> HitColliders { get; private set; }
        public IPuyo TargetPuyo { get; private set; }

        protected new void Awake()
        {
            base.Awake();
            HitColliders = new List<Collider>();
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
            if (!HitColliders.Exists(c => c.gameObject.GetInstanceID() == collider.gameObject.GetInstanceID()))
            {
                HitColliders.Add(collider);
            }
            UnityEngine.Debug.Log(collider.gameObject.name);
            UnityEngine.Debug.Log(collider.ClosestPoint(transform.position));
        }

        void OnTriggerExit(Collider collider)
        {
            if (!HitColliders.Exists(c => c.gameObject.GetInstanceID() == collider.gameObject.GetInstanceID()))
            {
                HitColliders.Remove(collider);
            }
        }

        public float HeightBetweenClosestPoint()
        {
            if (!HasCollision) { return 0f; }
            var y = HitColliders.Min(c => c.gameObject.transform.position.y);
            return transform.position.y - y;
        }
    }
}