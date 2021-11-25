using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision { get; private set; }
        public GameObject HitGameObject { get; private set; }
        public IPuyo TargetPuyo { get; private set; }

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
            if (IsPartner(collider.gameObject)) {
                TargetPuyo.Partner.Rigidbody.isKinematic = true;
                return;
            }
            HasCollision = true;
            HitGameObject = collider.gameObject;
        }

        void OnTriggerExit(Collider collider)
        {
            if (IsPartner(collider.gameObject))
            {
                TargetPuyo.Partner.Rigidbody.isKinematic = false;
            }
            HasCollision = false;
            HitGameObject = null;
        }
    }
}