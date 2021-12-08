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
            controller.Puyo.AdaptRandomMaterial();
            follower.Puyo.AdaptRandomMaterial();
            controller.Puyo.GameObject.layer = LayerMask.NameToLayer("Outline");
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
            return controller.IsFalling && follower.IsFalling;
        }

        private bool CanMoveAboutPuyoState()
        {
            // どちらか一方だけをみればいいはず
            return controller.IsFalling || controller.IsTouching;
        }

        private void RotateTo(Domain.PuyoRotation.Direction rotateDirection)
        {
            if (isRotating) { return; }
            followerOriginalPosition = Domain.PuyoRotation.GetCurrentPosition(controller.Puyo.GameObject.transform.position, follower.Puyo.GameObject.transform.position);
            followerNextPosition = Domain.PuyoRotation.GetNextPosition(rotateDirection, followerOriginalPosition);

            if (followerNextPosition == Domain.PuyoRotation.LEFT)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT)
                {
                    if (!controller.CanToLeft()) {
                        if (!controller.CanToRight()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(1, 0, 0));
                    }
                    // 回転と自由落下が組み合わさって食い込まないような処置
                    // 処理が複雑化するようなら違うソリューションで解決するのはアリ
                    if (controller.IsDangerRotateLeft())
                    {
                        controller.Stop();
                        follower.Stop();
                    }
                }
                if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT) {
                    if (!controller.CanToLeft()) {
                        if (!controller.CanToRight()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(1, 0, 0));
                    }
                    // 回転と自由落下が組み合わさって食い込まないような処置
                    // 処理が複雑化するようなら違うソリューションで解決するのはアリ
                    if (controller.IsDangerRotateLeft())
                    {
                        controller.Stop();
                        follower.Stop();
                    }
                }
            }
            if (followerNextPosition == Domain.PuyoRotation.RIGHT)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT)
                {
                    if (!controller.CanToRight())
                    {
                        if (!controller.CanToLeft()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(-1, 0, 0));
                    }
                    // 回転と自由落下が組み合わさって食い込まないような処置
                    // 処理が複雑化するようなら違うソリューションで解決するのはアリ
                    if (controller.IsDangerRotateRight())
                    {
                        controller.Stop();
                        follower.Stop();
                    }
                }
                if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT)
                {
                    if (!controller.CanToRight()) {
                        if (!controller.CanToLeft()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(-1, 0, 0));
                    }
                    // 回転と自由落下が組み合わさって食い込まないような処置
                    // 処理が複雑化するようなら違うソリューションで解決するのはアリ
                    if (controller.IsDangerRotateRight())
                    {
                        controller.Stop();
                        follower.Stop();
                    }
                }
            }
            if (followerNextPosition == Domain.PuyoRotation.LOWER)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT) {
                    // 競り上がりの処理
                    if (controller.IsDangerRotateLeft())
                    {
                        controller.Stop();
                        follower.Stop();
                        var y = controller.HeightBetweenClosestPoint();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(0, y, 0));
                    }
                }
                else if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT)
                {
                    // 競り上がりの処理
                    if (controller.IsDangerRotateRight())
                    {
                        controller.Stop();
                        follower.Stop();
                        var y = controller.HeightBetweenClosestPoint();
                        controller.ForceMove(controller.Puyo.GameObject.transform.position + new Vector3(0, y, 0));
                    }
                }
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
            if (rotateProgress >= 1f) { FinishToRotate(); }
        }

        private void FinishToRotate()
        {
            rotateProgress = 0f;
            isRotating = false;
            followerNextPosition = null;
            controller.Puyo.Rigidbody.isKinematic = false;
            follower.Puyo.Rigidbody.isKinematic = false;
            controller.Restart();
            follower.Restart();
            controller.ForceChangeState();
            follower.ForceChangeState();
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
                controller.Puyo.GameObject.layer = LayerMask.NameToLayer("Default");
                PuyoChainController.Instance.Check();
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