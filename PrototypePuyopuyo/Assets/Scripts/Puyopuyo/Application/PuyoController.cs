using com.amabie.SingletonKit;

namespace Puyopuyo.Application {
    /// <summary>
    /// パートナーやスケルトンとの同期はこの子が行う
    /// </summary>
    public class PuyoController : SingletonMonoBehaviour<PuyoController>
    {
        Puyopuyo.UI.Puyo controller;
        Puyopuyo.UI.Puyo follower;

        public void Observe(Puyopuyo.UI.Puyo controller, Puyopuyo.UI.Puyo follower)
        {
            this.controller = controller;
            this.follower = follower;
            controller.KnowPartner(follower.GameObject);
            follower.KnowPartner(controller.GameObject);
        }
        
        public void Update()
        {
            CheckInputEvent();
            PropergateTouchEvent();
            //PropergateStayEvent();
        }

        private void CheckInputEvent()
        {
            // Input.GetAxis は後で考える
            /*if (Input.GetKeyDown("left")) {
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
            }*/
        }

        private void PropergateTouchEvent()
        {
            if (controller == null || follower == null) { return; }
            // 同期
            if (controller.State.IsJustTouch && follower.State.IsFalling) {
                follower.ToJustTouch();
                // TODO: 仮に ToTouch 時点で Dispose してみる
                //SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
            if (follower.State.IsJustTouch && controller.State.IsFalling) {
                controller.ToJustTouch();
                //SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
            if (controller.State.IsJustTouch && follower.State.IsJustTouch) {
                if (controller.IsVerticalWithPartner()) {
                    controller.DoTouchAnimation();
                    follower.DoTouchAnimation();
                } else {
                    if (controller.IsGrounded) { controller.DoTouchAnimation(); }
                    if (follower.IsGrounded) { follower.TouchAnimation(); }
                }
                controller.TryToKeepTouching();
                follower.TryToKeepTouching();
            }
        }

        private void PropergateStayEvent()
        {
            /*// TODO: なんか機能してなさそう...
            if (controller == null || follower == null) { return; }
            // 本当はこれだけだとダメだが一旦これで。
            if (controller.State.IsStay && follower.State.IsTouch) {
                follower.ToStay();
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }
            if (follower.State.IsStay && controller.State.IsTouch) {
                controller.ToStay();
                SkeltonColliderCollectionGenerator.Instance.DisposeCollection();
            }*/
        }

        public void DisposeObservables()
        {
            this.controller = null;
            this.follower = null;
        }
    }
}