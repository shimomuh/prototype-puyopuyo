using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class SkeltonColliderGenerator : SingletonMonoBehaviour<SkeltonColliderGenerator>
    {
        private const string SKELTON_COLLIDER_PREFAB_PATH = "Prefabs/SkeltonCollider";

        public UI.SkeltonCollider Generate(Transform fieldTransform, Vector3 position, UI.Puyo targetPuyo)
        {
            GameObject skelton = Resources.Load<GameObject>(SKELTON_COLLIDER_PREFAB_PATH);
            GameObject skeltonObj = Instantiate(skelton);
            skeltonObj.name = skeltonObj.name.Replace("(Clone)","");
            skeltonObj.transform.SetParent(fieldTransform);
            skeltonObj.transform.position = position;
            skeltonObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var skeltonCollider = skeltonObj.GetComponent<UI.SkeltonCollider>();
            skeltonCollider.RecognizeTarget(targetPuyo);
            return skeltonCollider;
        }
    }
}
