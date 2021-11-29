using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision => HitColliders.Count != 0;
        // TODO: HitColliders でなく InstanceID のリストでもよい
        // 本来 ClosestPointOnBounds で Collider を使いたかったが必要なくなった
        // 今後 Collider でなく GameObject だけでよい可能性もあるが一旦このままで。
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
        }

        void OnTriggerExit(Collider collider)
        {
            if (HitColliders.Exists(c => c.gameObject.GetInstanceID() == collider.gameObject.GetInstanceID()))
            {
                HitColliders.Remove(collider);
            }
        }

        public float HeightBetweenClosestPoint()
        {
            if (!HasCollision) { return 0f; }
            // NOTE: 計算効率考慮で本質とは違う計算をしている
            // transform.position.y + 1 の部分は Mathf.Abs が正しい
            return (((transform.position.y + 1) * 10 % 10) == 5) ? 0.5f : 1f;
        }
    }
}