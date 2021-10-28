using com.amabie.SingletonKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyopuyoGenerator : SingletonMonoBehaviour<PuyopuyoGenerator>
    {
        private Puyopuyo.UI.Puyo controller;
        private Puyopuyo.UI.Puyo follower;

        private void Awake()
        {
            base.Awake();
            Permanent();
        }

        public void Generate(Transform parentTransform, Vector3 initialPosition)
        {
            controller = Puyopuyo.Application.PuyoGenerator.Instance.Generate(parentTransform, initialPosition);
            follower = Puyopuyo.Application.PuyoGenerator.Instance.Generate(parentTransform, initialPosition + new Vector3(0, 1, 0));
        }

        public void Update()
        {
            if (controller == null || follower == null) { return; }
            if (controller.State.IsTouch) { follower.ToTouch(); }
            if (follower.State.IsTouch) { controller.ToTouch(); }
        }
    }
}
