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

        private bool IsTarget(GameObject gameObj)
        {
            return ReferenceEquals(TargetPuyo.GameObject, gameObj);
        }

        private bool IsSkelton(GameObject gameObject)
        {
            return gameObject.GetComponent<SkeltonCollider>() != null;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (IsSkelton(collider.gameObject)) { return; }
            if (IsTarget(collider.gameObject)) { return; }
            HasCollision = true;
            HitGameObject = collider.gameObject;
        }

        void OnTriggerExit(Collider collider)
        {
            HasCollision = false;
            HitGameObject = null;
        }
    }
}