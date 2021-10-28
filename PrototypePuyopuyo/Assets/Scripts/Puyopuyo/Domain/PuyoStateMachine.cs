using System;

namespace Puyopuyo.Domain {
    public interface IPuyoStateMachine {
        bool IsStay { get; }
        bool IsTouch { get; }
        bool CanFall();
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
        public bool IsStay => currentState == State.Stay;
        public bool IsTouch => currentState == State.Touch;

        public PuyoStateMachine ()
        {
            ToFall();
        }

        public bool CanFall()
        {
            return currentState == State.Fall;
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