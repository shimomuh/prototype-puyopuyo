using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision { get; private set; }
        private GameObject targetPuyo;

        public void RecognizeTarget(Puyo targetPuyo)
        {
            this.targetPuyo = targetPuyo.gameObject;
        }

        private bool IsTarget(GameObject gameObj)
        {
            return ReferenceEquals(targetPuyo, gameObj);
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
        }

        void OnTriggerExit(Collider collider)
        {
            HasCollision = false;
        }
    }
}