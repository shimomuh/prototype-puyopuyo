using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    /// <summary>
    /// パートナーやスケルトンとの同期はこの子が行う
    /// </summary>
    public class PuyoController : SingletonMonoBehaviour<PuyoController>
    {
        UI.IPuyo controller;
        UI.IPuyo follower;
        UI.SkeltonColliderCollection skeltonColliderCollection;

        public void Observe(UI.IPuyo controller, UI.IPuyo follower, UI.SkeltonColliderCollection skeltonColliderCollection)
        {
            this.controller = controller;
            this.follower = follower;
            // TODO: 回転するときに controller 用のと follower 用に分けないといけない
            this.skeltonColliderCollection = skeltonColliderCollection;
            controller.RecognizePartner(follower.GameObject);
            follower.RecognizePartner(controller.GameObject);
        }
        
        public void Update()
        {
            if (controller == null || follower == null) { return; }
            CheckInputEvent();
            PropergateTouchEvent();
            PropergateStayEvent();
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            if (Input.GetKeyDown("left")) {
                if (!CanMove()) { return; }
                if (!skeltonColliderCollection.CanToLeft()) { return; }
                controller.ToLeft();
                follower.ToLeft();
                skeltonColliderCollection.ToLeft();
            }

            if (Input.GetKeyDown("right")) {
                if (!CanMove()) { return; }
                if (!skeltonColliderCollection.CanToRight()) { return; }
                controller.ToRight();
                follower.ToRight();
                skeltonColliderCollection.ToRight();
            }

            if (Input.GetKeyDown("down")) {
                if (!CanMove()) { return; }
                //if (!skeltonColliderCollection.CanToDown()) { return; }
                controller.ToDown();
                follower.ToDown();
                skeltonColliderCollection.ToDown();
            }
        }

        private bool CanMove()
        {
            if (!CanMoveAboutPuyoState()) { return false; }
            return true;
        }

        private bool CanMoveAboutPuyoState()
        {
            // どちらか一方だけをみればいいはず
            return controller.State.IsFalling || controller.State.IsTouching;
        }

        private void PropergateTouchEvent()
        {
            // 同期
            if (controller.State.IsJustTouch && follower.State.IsFalling) {
                follower.ToJustTouch();
                skeltonColliderCollection.ToJustTouch();
            }
            if (follower.State.IsJustTouch && controller.State.IsFalling) {
                controller.ToJustTouch();
                skeltonColliderCollection.ToJustTouch();
            }
            if (controller.State.IsJustTouch && follower.State.IsJustTouch) {
                if (controller.IsVerticalWithPartner()) {
                    controller.DoTouchAnimation();
                    follower.DoTouchAnimation();
                } else {
                    if (controller.IsGrounded) { controller.DoTouchAnimation(); }
                    if (follower.IsGrounded) { follower.DoTouchAnimation(); }
                }
                controller.TryToKeepTouching();
                follower.TryToKeepTouching();
                skeltonColliderCollection.TryToKeepTouching();
            }
        }

        private void PropergateStayEvent()
        {
            if (controller.State.IsJustStay && follower.State.IsTouching) {
                follower.ToJustStay();
                skeltonColliderCollection.ToJustStay();
            }
            if (follower.State.IsJustStay && controller.State.IsTouching) {
                controller.ToJustStay();
                skeltonColliderCollection.ToJustStay();
            }
            if (controller.State.IsJustStay && follower.State.IsJustStay) {
                controller.ToStay();
                follower.ToStay();
                skeltonColliderCollection.ToStay();
                skeltonColliderCollection.Dispose();
                DisposeObservables();
            }
        }

        public void DisposeObservables()
        {
            controller = null;
            follower = null;
            skeltonColliderCollection = null;
        }
    }
}