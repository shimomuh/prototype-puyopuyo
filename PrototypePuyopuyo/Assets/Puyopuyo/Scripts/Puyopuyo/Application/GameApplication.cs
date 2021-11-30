using UnityEngine;
using System;

namespace Puyopuyo.Application {
    public class GameApplication : MonoBehaviour
    {
        private void Awake() {
            // フレームレートの設定
            UnityEngine.Application.targetFrameRate = 60;
            MakeSingletonMonoBehavoiur("PuyoGenerator");
            MakeSingletonMonoBehavoiur("PuyopuyoGenerator");
            MakeSingletonMonoBehavoiur("PuyoController");
            MakeSingletonMonoBehavoiur("SkeltonColliderCollectionGenerator");
            MakeSingletonMonoBehavoiur("SkeltonColliderGenerator");
        }

        private void MakeSingletonMonoBehavoiur(string name)
        {
            var gameObj = new GameObject();
            gameObj.name = name;
            gameObj.AddComponent(ApplicationName(name));
        }

        private Type ApplicationName(string name)
        {
            return Type.GetType($"Puyopuyo.Application.{name}");
        }
    }
}