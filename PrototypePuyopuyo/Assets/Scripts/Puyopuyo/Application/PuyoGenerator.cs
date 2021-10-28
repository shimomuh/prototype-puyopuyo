using com.amabie.SingletonKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyoGenerator : SingletonMonoBehaviour<PuyoGenerator>
    {
        private string PUYO_PREFAB_PATH = "Prefabs/Puyo";

        private void Awake()
        {
            base.Awake();
            Permanent();
        }

        public Puyopuyo.UI.Puyo Generate(Transform parentTransform, Vector3 initialPosition)
        {
            GameObject puyoSkelton = Resources.Load<GameObject>(PUYO_PREFAB_PATH);
            GameObject puyoObj = Instantiate(puyoSkelton);
            puyoObj.name = puyoObj.name.Replace("(Clone)","");
            puyoObj.transform.SetParent(parentTransform);
            puyoObj.transform.position = initialPosition;
            return puyoObj.GetComponent<Puyopuyo.UI.Puyo>();
        }
    }
}