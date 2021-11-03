using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : Puyo
    {
        public bool HasCollision { get; private set; }

        private void OnCollisionEnter()
        {
            // do nothing
        }

        void OnTriggerEnter(Collider collider)
        {
            HasCollision = true;
        }

        void OnTriggerExit(Collider collider)
        {
            HasCollision = false;
        }
    }
}