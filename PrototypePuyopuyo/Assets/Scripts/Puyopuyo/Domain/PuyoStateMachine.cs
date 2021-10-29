using System;

namespace Puyopuyo.Domain {
    public interface IPuyoStateMachine {
        bool CanFall { get; }
        bool IsFall { get; }
        bool IsTouch { get; }
        bool IsStay { get; }
        void ToFall();
        void ToTouch();
        void ToStay();
    }
    public class PuyoStateMachine : IPuyoStateMachine {
        private enum State {
            Fall,
            Touch,
            Stay
        }
        private State currentState;
        public bool CanFall => currentState == State.Fall;
        public bool IsFall => currentState == State.Fall;
        public bool IsTouch => currentState == State.Touch;
        public bool IsStay => currentState == State.Stay;

        public PuyoStateMachine ()
        {
            ToFall();
        }

        public void ToFall()
        {
            currentState = State.Fall;
        }

        public void ToTouch()
        {
            if (currentState == State.Stay) { throw new Exception("静止状態から触れている状態にいきなり遷移はできません！"); }
            currentState = State.Touch;
        }

        public void ToStay()
        {
            currentState = State.Stay;
        }
    }
}