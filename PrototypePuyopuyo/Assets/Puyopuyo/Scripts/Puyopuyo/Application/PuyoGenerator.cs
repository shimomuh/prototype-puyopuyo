using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyoGenerator : SingletonMonoBehaviour<PuyoGenerator>
    {
        private const string PUYO_PREFAB_PATH = "Prefabs/Puyo";

        public UI.Puyo Generate(Transform parentTransform, Vector3 initialPosition, UI.PuyoMaterial puyoMaterial)
        {
            GameObject puyoSkelton = Resources.Load<GameObject>(PUYO_PREFAB_PATH);
            GameObject puyoObj = Instantiate(puyoSkelton);
            puyoObj.name = puyoObj.name.Replace("(Clone)","");
            puyoObj.transform.SetParent(parentTransform);
            puyoObj.transform.position = initialPosition;
            puyoObj.GetComponent<UI.IPuyo>().AdaptMaterial(puyoMaterial);
            return puyoObj.GetComponent<UI.Puyo>();
        }
    }
}
