
using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    /// <summary>
    /// ぷよの操作を扱う
    /// パートナーの同期はこの子が行う
    /// </summary>
    public class PuyoController : SingletonMonoBehaviour<PuyoController>
    {
        UI.IPuyo controller;
        UI.IPuyo follower;
        private bool isRotating;
        private Domain.PuyoPosition.Position followerOriginalPosition;
        private Domain.PuyoPosition.Position followerNextPosition;
        private float rotateSpeed = 10.0f;
        private float rotateProgress = 0.0f;
        private bool occuredInputEventInThisFrame;

        public void Observe(UI.IPuyo controller, UI.IPuyo follower)
        {
            this.controller = controller;
            this.follower = follower;
            controller.UnderControllWith(follower);
            follower.UnderControllWith(controller);
            controller.GameObject.layer = LayerMask.NameToLayer("Outline");
        }
        
        public void Update()
        {
            if (controller == null || follower == null) { return; }
            CheckInputEvent();
            if (!occuredInputEventInThisFrame)
            {
                controller.UpdatePerFrame();
                follower.UpdatePerFrame();
            }
            UpdateRotate();
            PropergateTouchEvent();
            PropergateCancelTouchEvent();
            PropergateStayEvent();
            occuredInputEventInThisFrame = false;
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (!CanSlide()) { return; }
                if (!controller.CanMoveToLeft()) { return; }
                if (!follower.CanMoveToLeft()) { return; }
                occuredInputEventInThisFrame = true;
                controller.ToLeft();
                follower.ToLeft();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                if (!CanSlide()) { return; }
                if (!controller.CanMoveToRight()) { return; }
                if (!follower.CanMoveToRight()) { return; }
                occuredInputEventInThisFrame = true;
                controller.ToRight();
                follower.ToRight();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (!CanDown()) { return; }
                occuredInputEventInThisFrame = true;
                controller.ToDown();
                follower.ToDown();
            }
            // 左回り
            if (Input.GetKeyDown(KeyCode.Space)) {
                occuredInputEventInThisFrame = true;
                RotateTo(Domain.PuyoRotation.ROTATE_LEFT);
            }
            // 右回り
            if (Input.GetKeyDown(KeyCode.Return)) {
                occuredInputEventInThisFrame = true;
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
            return controller.State.IsFalling && follower.State.IsFalling;
        }

        private bool CanMoveAboutPuyoState()
        {
            return (controller.State.IsFalling || controller.State.IsTouching)
                && (follower.State.IsFalling || follower.State.IsTouching);
        }

        /// <summary>
        /// TODO: 現状その場で回していても落下タイミングによって Touching にいきなり切り替わるバグあり
        /// </summary>
        /// <param name="rotateDirection"></param>
        private void RotateTo(Domain.PuyoRotation.Direction rotateDirection)
        {
            if (isRotating) { return; }
            followerOriginalPosition = Domain.PuyoRotation.GetCurrentPosition(controller.GameObject.transform.position, follower.GameObject.transform.position);
            followerNextPosition = Domain.PuyoRotation.GetNextPosition(rotateDirection, followerOriginalPosition);

            if (followerNextPosition == Domain.PuyoPosition.LEFT)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT)
                {
                    if (!controller.CanMoveToLeft())
                    {
                        if (!controller.CanMoveToRight()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(1, 0, 0));
                    }
                }
                if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT) {
                    if (!controller.CanMoveToLeft())
                    {
                        if (!controller.CanMoveToRight()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(1, 0, 0));
                    }
                }
            }
            if (followerNextPosition == Domain.PuyoPosition.RIGHT)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT)
                {
                    if (!controller.CanMoveToRight())
                    {
                        if (!controller.CanMoveToLeft()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(-1, 0, 0));
                    }
                }
                if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT)
                {
                    if (!controller.CanMoveToRight())
                    {
                        if (!controller.CanMoveToLeft()) { return; }
                        controller.Stop();
                        follower.Stop();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(-1, 0, 0));
                    }
                }
            }
            if (followerNextPosition == Domain.PuyoPosition.LOWER)
            {
                if (rotateDirection == Domain.PuyoRotation.ROTATE_LEFT) {
                    // 競り上がりの処理
                    if (!controller.CanMoveToDown())
                    {
                        controller.Stop();
                        follower.Stop();
                        var y = controller.HeightToGround();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(0, y, 0));
                    }
                }
                else if (rotateDirection == Domain.PuyoRotation.ROTATE_RIGHT)
                {
                    // 競り上がりの処理
                    if (!controller.CanMoveToDown())
                    {
                        controller.Stop();
                        follower.Stop();
                        var y = controller.HeightToGround();
                        controller.ForceToMove(controller.GameObject.transform.position + new Vector3(0, y, 0));
                    }
                }
            }
            // ひとまず回転中、反発はなしにした
            follower.Rigidbody.isKinematic = true;
            controller.Rigidbody.isKinematic = true;
            isRotating = true;
            controller.ToCanceling();
            follower.ToCanceling();

        }

        private void UpdateRotate()
        {
            if (!isRotating) { return; }
            var startPos = follower.GameObject.transform.position;
            var endPos = Domain.PuyoRotation.GetNextPosition(controller.GameObject.transform.position, followerNextPosition);
            rotateProgress += rotateSpeed * Time.deltaTime;
            if (rotateProgress >= 1f) { rotateProgress = 1f; }
            follower.ForceToMove(Vector3.Lerp(startPos, endPos, rotateProgress));
            if (rotateProgress >= 1f) { FinishToRotate(); }
        }

        private void FinishToRotate()
        {
            rotateProgress = 0f;
            isRotating = false;
            followerNextPosition = null;
            controller.Rigidbody.isKinematic = false;
            follower.Rigidbody.isKinematic = false;
            controller.Restart();
            follower.Restart();
        }

        private void PropergateTouchEvent()
        {
            // 同期
            if (controller.State.IsJustTouch && follower.State.IsFalling)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
            }
            if (follower.State.IsJustTouch && controller.State.IsFalling)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
            }
            if (controller.State.IsJustTouch && follower.State.IsJustTouch)
            {
                controller.ToJustTouch();
                follower.ToJustTouch();
                controller.DoTouchAnimation();
                follower.DoTouchAnimation();
                controller.TryToKeepTouching();
                follower.TryToKeepTouching();
            }
        }

        private void PropergateCancelTouchEvent()
        {
            if (controller.State.IsTouching && follower.State.IsCanceling) {
                controller.ToCanceling();
                follower.ToCanceling();
            }
            if (controller.State.IsCanceling && follower.State.IsTouching) {
                controller.ToCanceling();
                follower.ToCanceling();
            }
            if (controller.State.IsCanceling && follower.State.IsCanceling)
            {
                controller.ToCanceling();
                follower.ToCanceling();
                controller.ToFall();
                follower.ToFall();
            }
        }

        private void PropergateStayEvent()
        {
            if (controller.State.IsJustStay && follower.State.IsTouching) {
                controller.ToJustStay();
                follower.ToJustStay();
            }
            if (follower.State.IsJustStay && controller.State.IsTouching) {
                controller.ToJustStay();
                follower.ToJustStay();
            }
            if (controller.State.IsJustStay && follower.State.IsJustStay)
            {
                controller.ToJustStay();
                follower.ToJustStay();
                controller.ToStay();
                follower.ToStay();
                controller.GameObject.layer = LayerMask.NameToLayer("Default");
                DisposeObservables();
            }
        }

        public void DisposeObservables()
        {
            controller = null;
            follower = null;
        }
    }
}