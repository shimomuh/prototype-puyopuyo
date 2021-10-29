using com.amabie.SingletonKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Application {
    public class SkeltonColliderGenerator : SingletonMonoBehaviour<SkeltonColliderGenerator>
    {
        private const string SKELTON_COLLIDER_PREFAB_PATH = "Prefabs/SkeltonCollider";

        public Puyopuyo.UI.SkeltonCollider Generate(Transform parentTransform, Vector3 position)
        {
            GameObject skelton = Resources.Load<GameObject>(SKELTON_COLLIDER_PREFAB_PATH);
            GameObject skeltonObj = Instantiate(skelton);
            skeltonObj.name = skeltonObj.name.Replace("(Clone)","");
            skeltonObj.transform.SetParent(parentTransform);
            skeltonObj.transform.position = position;
            return skeltonObj.GetComponent<Puyopuyo.UI.SkeltonCollider>();
        }
    }
}
