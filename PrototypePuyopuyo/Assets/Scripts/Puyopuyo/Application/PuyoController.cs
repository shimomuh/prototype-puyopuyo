using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    /// <summary>
    /// パートナーやスケルトンとの同期はこの子が行う
    /// </summary>
    public class PuyoController : SingletonMonoBehaviour<PuyoController>
    {
        UI.IPuyoWithSkeltonColliderCollection controller;
        UI.IPuyoWithSkeltonColliderCollection follower;
        private bool isRotating;
        private Domain.PuyoRotation.Position followerOriginalPosition;
        private Domain.PuyoRotation.Position followerNextPosition;
        private float rotateSpeed = 10.0f;
        private float rotateProgress = 0.0f;

        public void Observe(UI.IPuyoWithSkeltonColliderCollection controller, UI.IPuyoWithSkeltonColliderCollection follower)
        {
            this.controller = controller;
            this.follower = follower;
            controller.RecognizePartner(follower);
            follower.RecognizePartner(controller);
        }
        
        public void Update()
        {
            if (controller == null || follower == null) { return; }
            CheckInputEvent();
            UpdateRotate();
            PropergateTouchEvent();
            PropergateCancelTouchEvent();
            PropergateStayEvent();
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (!CanSlide()) { return; }
                if (!controller.CanToLeft()) { return; }
                if (!follower.CanToLeft()) { return; }
                controller.ToLeft();
                follower.ToLeft();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                if (!CanSlide()) { return; }
                if (!controller.CanToRight()) { return; }
                if (!follower.CanToRight()) { return; }
                controller.ToRight();
                follower.ToRight();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (!CanDown()) { return; }
                controller.ToDown();
                follower.ToDown();
            }
            // 左回り
            if (Input.GetKeyDown(KeyCode.Space)) {
                RotateTo(Domain.PuyoRotation.ROTATE_LEFT);
            }
            // 右回り
            if (Input.GetKeyDown(KeyCode.Return)) {
                RotateTo(Domain.PuyoRotation.ROTATE_RIGHT);
            }
        }

        private bool CanSlide()
        {
            if (!CanMoveAboutPuyoState()) { return false; }
            return true;
        }

        private bool CanDown()
        {
            return controller.IsFalling && follower.Puyo.State.IsFalling;
        }

        private bool CanMoveAboutPuyoState()
        {
            // どちらか一方だけをみればいいはず
            return controller.Puyo.State.IsFalling || controller.Puyo.State.IsTouching;
        }

        private void RotateTo(Domain.PuyoRotation.Direction rotateDirection)
        {
            if (isRotating) { return; }
            followerOriginalPosition = Domain.PuyoRotation.GetCurrentPosition(controller.Puyo.GameObject.transform.position, follower.Puyo.GameObject.transform.position);
            followerNextPosition = Domain.PuyoRotation.GetNextPosition(rotateDirection, followerOriginalPosition);

            if (followerNextPosition == Domain.PuyoRotation.LEFT) {
                if (!controller.CanToLeft()) { return; }
            }
            if (followerNextPosition == Domain.PuyoRotation.RIGHT)
            {
                if (!controller.CanToRight()) { return; }
            }
            // ひとまず回転中、反発はなしにした
            follower.Puyo.Rigidbody.isKinematic = true;
            controller.Puyo.Rigidbody.isKinematic = true;
            isRotating = true;

        }

        private void UpdateRotate()
        {
            if (!isRotating) { return; }
            var startPos = follower.Puyo.GameObject.transform.position;
            var endPos = Domain.PuyoRotation.GetNextPosition(controller.Puyo.GameObject.transform.position, followerNextPosition);
            rotateProgress += rotateSpeed * Time.deltaTime;
            if (rotateProgress >= 1f) { rotateProgress = 1f; }
            follower.LerpRotate(Vector3.Lerp(startPos, endPos, rotateProgress));
            if (rotateProgress >= 1f)
            {
                rotateProgress = 0f;
                isRotating = false;
                followerNextPosition = null;
                controller.Puyo.Rigidbody.isKinematic = false;
                follower.Puyo.Rigidbody.isKinematic = false;
            }
        }

        private void PropergateTouchEvent()
        {
            // 同期
            if (controller.IsJustTouch && follower.IsFalling)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
            }
            if (follower.IsJustTouch && controller.IsFalling)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
            }
            if (controller.IsJustTouch && follower.IsJustTouch)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
                if (controller.Puyo.IsVerticalWithPartner()) {
                    controller.Puyo.DoTouchAnimation();
                    follower.Puyo.DoTouchAnimation();
                } else {
                    if (controller.Puyo.IsGrounded) { controller.Puyo.DoTouchAnimation(); }
                    if (follower.Puyo.IsGrounded) { follower.Puyo.DoTouchAnimation(); }
                }
                controller.TryToKeepTouching();
                follower.TryToKeepTouching();
            }
        }

        private void PropergateCancelTouchEvent()
        {
            if (controller.IsTouching && follower.IsCancelTouching) {
                controller.ToCancelTouching();
                follower.ToCancelTouching();
            }
            if (controller.IsCancelTouching && follower.IsTouching) {
                controller.ToCancelTouching();
                follower.ToCancelTouching();
            }
            if (controller.IsCancelTouching && follower.IsCancelTouching)
            {
                controller.ToCancelTouching();
                follower.ToCancelTouching();
                controller.ToFall();
                follower.ToFall();
            }
        }

        private void PropergateStayEvent()
        {
            if (controller.IsJustStay && follower.IsTouching) {
                controller.ToJustStay();
                follower.ToJustStay();
            }
            if (follower.IsJustStay && controller.IsTouching) {
                controller.ToJustStay();
                follower.ToJustStay();
            }
            if (controller.IsJustStay && follower.IsJustStay)
            {
                controller.ToJustStay();
                follower.ToJustStay();
                controller.ToStay();
                follower.ToStay();
                DisposeObservables();
            }
        }

        public void DisposeObservables()
        {
            controller.Dispose();
            follower.Dispose();
            controller = null;
            follower = null;
        }
    }
}