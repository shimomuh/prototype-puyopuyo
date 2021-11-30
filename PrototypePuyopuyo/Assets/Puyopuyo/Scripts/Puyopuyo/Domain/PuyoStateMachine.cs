using System;

namespace Puyopuyo.Domain {
    public interface IPuyoStateMachine {
        bool IsFalling { get; }
        bool IsJustTouch { get; }
        bool IsTouching { get; }
        bool IsJustStay { get; }
        bool IsStaying { get; }
        bool IsCancelTouching { get; }
        void ToFalling();
        void ToJustTouch();
        void ToTouching();
        void ToJustStay();
        void ToStaying();
        void ToCancelTouching();
    }
    public class PuyoStateMachine : IPuyoStateMachine {
        public enum State {
            Falling,
            JustTouch,
            Touching,
            JustStay,
            Staying,
            CancelTouching
        }
        private State currentState;
        public bool IsFalling => currentState == State.Falling;
        public bool IsJustTouch => currentState == State.JustTouch;
        public bool IsTouching => currentState == State.Touching;
        public bool IsJustStay => currentState == State.JustStay;
        public bool IsStaying => currentState == State.Staying;
        public bool IsCancelTouching => currentState == State.CancelTouching;

        public PuyoStateMachine ()
        {
            ToFalling();
        }

        public void ToFalling()
        {
            currentState = State.Falling;
        }

        public void ToJustTouch()
        {
            if (IsJustStay || IsStaying) {
                throw new Exception("「静止状態」から「ちょうど触れた状態」にいきなり遷移はできません！");
            }
            currentState = State.JustTouch;
        }

        public void ToTouching()
        {
            if (!IsJustTouch) {
                throw new Exception("「ちょうど触れた状態」でないと「触れている状態」にいきなり遷移はできません！");
            }
            currentState = State.Touching;
        }

        public void ToJustStay()
        {
            if (!IsTouching) {
                throw new Exception("「触れている状態」でないと「ちょうど留まっている状態」にいきなり遷移はできません！");
            }
            currentState = State.JustStay;
        }

        public void ToStaying()
        {
            // IsFalling は FreeFall 時
            if (!(IsJustStay || IsFalling)) {
                throw new Exception("「ちょうど留まっている/落ちている状態」でないと「留まっている状態」にいきなり遷移はできません！");
            }
            currentState = State.Staying;
        }

        public void ToCancelTouching()
        {
            if (!IsTouching) {
                throw new Exception("「触れている状態」でないと「触れているをキャンセルする状態」にいきなり遷移はできません！");
            }
            currentState = State.CancelTouching;
        }

        public override string ToString()
        {
            return currentState.ToString();
        }
    }
}