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
        UI.SkeltonColliderCollection controllerSkeltonColliderCollection;
        UI.SkeltonColliderCollection followerSkeltonColliderCollection;

        public void Observe(UI.IPuyo controller, UI.IPuyo follower, UI.SkeltonColliderCollection controllerSkeltonColliderCollection, UI.SkeltonColliderCollection followerSkeltonColliderCollection)
        {
            this.controller = controller;
            this.follower = follower;
            this.controllerSkeltonColliderCollection = controllerSkeltonColliderCollection;
            this.followerSkeltonColliderCollection = followerSkeltonColliderCollection;
            controller.RecognizePartner(follower.GameObject);
            follower.RecognizePartner(controller.GameObject);
        }
        
        public void Update()
        {
            if (controller == null || follower == null) { return; }
            CheckInputEvent();
            PropergateTouchEvent();
            PropergateCancelTouchEvent();
            PropergateStayEvent();
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            if (Input.GetKeyDown("left")) {
                if (!CanSlide()) { return; }
                if (!controllerSkeltonColliderCollection.CanToLeft()) { return; }
                if (!followerSkeltonColliderCollection.CanToLeft()) { return; }
                controller.ToLeft();
                follower.ToLeft();
                controllerSkeltonColliderCollection.ToLeft();
                followerSkeltonColliderCollection.ToLeft();
            }

            if (Input.GetKeyDown("right")) {
                if (!CanSlide()) { return; }
                if (!controllerSkeltonColliderCollection.CanToRight()) { return; }
                if (!followerSkeltonColliderCollection.CanToRight()) { return; }
                controller.ToRight();
                follower.ToRight();
                controllerSkeltonColliderCollection.ToRight();
                followerSkeltonColliderCollection.ToRight();
            }

            if (Input.GetKeyDown("down")) {
                if (!CanDown()) { return; }
                //if (!controllerSkeltonColliderCollection.CanToDown()) { return; }
                controller.ToDown();
                follower.ToDown();
                controllerSkeltonColliderCollection.ToDown();
                followerSkeltonColliderCollection.ToDown();
            }
        }

        private bool CanSlide()
        {
            if (!CanMoveAboutPuyoState()) { return false; }
            return true;
        }

        private bool CanDown()
        {
            return controller.State.IsFalling && follower.State.IsFalling;
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
                controllerSkeltonColliderCollection.ToJustTouch();
                followerSkeltonColliderCollection.ToJustTouch();
            }
            if (follower.State.IsJustTouch && controller.State.IsFalling) {
                controller.ToJustTouch();
                controllerSkeltonColliderCollection.ToJustTouch();
                followerSkeltonColliderCollection.ToJustTouch();
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
                controllerSkeltonColliderCollection.TryToKeepTouching();
                followerSkeltonColliderCollection.TryToKeepTouching();
            }
        }

        private void PropergateCancelTouchEvent()
        {
            if (controller.State.IsTouching && follower.State.IsCancelTouching) {
                controller.ToCancelTouching();
                controllerSkeltonColliderCollection.ToCancelTouching();
                followerSkeltonColliderCollection.ToCancelTouching();
            }
            if (controller.State.IsCancelTouching && follower.State.IsTouching) {
                follower.ToCancelTouching();
                controllerSkeltonColliderCollection.ToCancelTouching();
                followerSkeltonColliderCollection.ToCancelTouching();
            }
            if (controller.State.IsCancelTouching && follower.State.IsCancelTouching) {
                controller.ToFall();
                follower.ToFall();
                controllerSkeltonColliderCollection.ToFall();
                followerSkeltonColliderCollection.ToFall();
            }
        }

        private void PropergateStayEvent()
        {
            if (controller.State.IsJustStay && follower.State.IsTouching) {
                follower.ToJustStay();
                controllerSkeltonColliderCollection.ToJustStay();
                followerSkeltonColliderCollection.ToJustStay();
            }
            if (follower.State.IsJustStay && controller.State.IsTouching) {
                controller.ToJustStay();
                controllerSkeltonColliderCollection.ToJustStay();
                followerSkeltonColliderCollection.ToJustStay();
            }
            if (controller.State.IsJustStay && follower.State.IsJustStay) {
                controller.ToStay();
                follower.ToStay();
                controllerSkeltonColliderCollection.ToStay();
                followerSkeltonColliderCollection.ToStay();
                DisposeObservables();
            }
        }

        public void DisposeObservables()
        {
            controllerSkeltonColliderCollection.Dispose();
            followerSkeltonColliderCollection.Dispose();

            controller = null;
            follower = null;
            controllerSkeltonColliderCollection = null;
            followerSkeltonColliderCollection = null;
        }
    }
}