using com.amabie.SingletonKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyoController : SingletonMonoBehaviour<PuyoController>
    {
        Puyopuyo.UI.Puyo controller;
        Puyopuyo.UI.Puyo follower;
        private const string WALL_TAG = "Wall";
        private const string SKELTON_TAG = "Skelton";

        public void Observe(Puyopuyo.UI.Puyo controller, Puyopuyo.UI.Puyo follower)
        {
            this.controller = controller;
            this.follower = follower;
            controller.ToBeControllable();
            follower.ToBeControllable();
            NotifyEnvironment();
        }

        private void NotifyEnvironment()
        {
            controller.IgnoreTouchObjectTags = new List<string>() { WALL_TAG, SKELTON_TAG };
            follower.IgnoreTouchObjectTags = new List<string>() { WALL_TAG, SKELTON_TAG };
        }

        public void Update()
        {
            CheckInputEvent();
            PropergateTouchEvent();
            PropergateStayEvent();
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            if (Input.GetKeyDown("left")) {
                if (SkeltonColliderCollectionGenerator.Instance.HasCollision(new List<Direction> { Direction.UpperLeft, Direction.MiddleLeft, Direction.LowerLeft })) { return; }
                controller.Left();
                follower.Left();
            }

            if (Input.GetKeyDown("right")) {
                if (SkeltonColliderCollectionGenerator.Instance.HasCollision(new List<Direction> { Direction.UpperRight, Direction.MiddleRight, Direction.LowerRight })) { return; }
                controller.Right();
                follower.Right();
            }

            if (Input.GetKeyDown("down")) {
                controller.Down();
                follower.Down();
            }
        }

        private void PropergateTouchEvent()
        {
            if (controller == null || follower == null) { return; }
            if (controller.State.IsTouch && follower.State.IsFall) {
                follower.ToTouch();
                // TODO: 仮に ToTouch 時点で Dispose してみる
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
            if (follower.State.IsTouch && controller.State.IsFall) {
                controller.ToTouch();
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
        }

        private void PropergateStayEvent()
        {
            // TODO: なんか機能してなさそう...
            if (controller == null || follower == null) { return; }
            // 本当はこれだけだとダメだが一旦これで。
            if (controller.State.IsStay && follower.State.IsTouch) {
                follower.ToStay();
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
            if (follower.State.IsStay && controller.State.IsTouch) {
                controller.ToStay();
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
        }

        public void DisposeObservables()
        {
            this.controller = null;
            this.follower = null;
        }
    }
}