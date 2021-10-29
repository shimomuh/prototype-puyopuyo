using UnityEngine;

namespace Puyopuyo.UI {
    public class SkeltonCollider : MonoBehaviour
    {
        public bool HasCollision { get; private set; }
        private const string SKELTON_TAG = "Skelton";

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == SKELTON_TAG) { return; }
            HasCollision = true;
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.tag == SKELTON_TAG) { return; }
            Debug.Log("kitenai");
            HasCollision = false;
        }
    }
}