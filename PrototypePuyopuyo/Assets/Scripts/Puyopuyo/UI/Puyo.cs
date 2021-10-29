using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Puyopuyo.UI {
    public class Puyo : MonoBehaviour
    {
        private float MOVE_FALL_AMOUNT = -0.5f;
        private Puyopuyo.Domain.IPuyoBodyClock puyoBodyClock;
        public Puyopuyo.Domain.IPuyoStateMachine State { get; private set; }
        private Collider collider;
        private bool IsControllable;
        public List<string> IgnoreTouchObjectTags { get; set; }

        private void Awake()
        {
            puyoBodyClock = new Puyopuyo.Domain.PuyoBodyClock();
            State = new Puyopuyo.Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
            IgnoreTouchObjectTags = new List<string>() {};
        }

        public void ToBeControllable()
        {
            IsControllable = true;
        }

        private void Start()
        {
            puyoBodyClock.NotifyBeginToFall();
            State.ToFall();
        }

        private void Update()
        {
            UpdateAboutFall();
            UpdateAboutTouch();
            UpdateAboutStay();
        }

        private void UpdateAboutFall()
        {
            puyoBodyClock.UpdateAboutFall();
            if (!puyoBodyClock.ShouldFallAction) { return; }
            AutoDown();
            puyoBodyClock.NotifyFinishFallAction();
        }

        private void UpdateAboutTouch()
        {
            puyoBodyClock.UpdateAboutTouch();
            // TODO: 操作によるキャンセル時の処理はここでやる
            // TODO: 操作によって controller または follower が ToStay の状態になりたいとき
            // 直下にものがないのを確認したらまた fall する
            // その時は fall の速度を変えるインターフェースを PuyoBodyClock に実装する
        }

        private void UpdateAboutStay()
        {
            if (!puyoBodyClock.ShouldStayAction) { return; }
            ToStay();
        }

        public void ToStay()
        {
            State.ToStay();
            // NOTE: 一旦アニメーションを切る
            //StartCoroutine(StayAnimation());
            puyoBodyClock.NotifyFinishStayAction(); // 手前の処理が非同期だけどまぁいっか
        }

        private void AutoDown()
        {
            if (!State.CanFall) { return; }
            transform.Translate(0, MOVE_FALL_AMOUNT, 0);
        } 

        private IEnumerator StayAnimation()
        {
            collider.enabled = false;
            // Lerp でやりたいけど、もっというとアニメーターでやりたいから一旦仮置き
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.25f, 0.9f, 1.25f);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);
            transform.localScale = new Vector3(1.5f, 0.8f, 1.5f);
            yield return new WaitForSeconds(0.1f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.25f, 0.9f, 1.25f);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1, 1, 1);
            collider.enabled = true;
            yield return null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (State.IsStay) { return; }
            if (IgnoreTouchObjectTags.Contains(collision.collider.gameObject.tag)) { return; }
            if (CanIgnoreAboutAdjoinedPuyo(collision.collider.gameObject)) { return; }
            ToTouch();
        }

        private bool CanIgnoreAboutAdjoinedPuyo(GameObject gameObj)
        {
            var adjoinedPuyo = gameObj.GetComponent<Puyopuyo.UI.Puyo>();
            if (adjoinedPuyo == null) { return false; }
            return adjoinedPuyo.IsControllable;
        }

        public void ToTouch()
        {
            State.ToTouch();
            puyoBodyClock.NotifyBeginToTouch();
        }


        public void Left()
        {
            transform.Translate(-1, 0, 0);
        }

        public void Right()
        {
            transform.Translate(1, 0, 0);
        }

        public void Down()
        {
            transform.Translate(0, MOVE_FALL_AMOUNT, 0);
        }
    }
}