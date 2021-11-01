using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Puyopuyo.UI {
    public interface IPuyo {
        Puyopuyo.Domain.IPuyoStateMachine State { get; }
        bool IsGrounded { get; }
        void ToFall();
        void ToJustStay();
        void ToJustTouch();
        void TryToKeepTouching();
        bool IsVerticalWithPartner();
        void AnimateTouch();
    }
    public class Puyo : MonoBehaviour, IPuyo
    {
        private float MOVE_FALL_AMOUNT = -0.5f;
        private Puyopuyo.Domain.IPuyoBodyClock puyoBodyClock;
        public Puyopuyo.Domain.IPuyoStateMachine State { get; private set; }
        public bool IsGrounded { get; private set; }
        public GameObject GameObject => gameObject;
        public GameObject partner;
        private bool hasPartner => partner != null;
        private Collider collider;

        private void Awake()
        {
            puyoBodyClock = new Puyopuyo.Domain.PuyoBodyClock();
            State = new Puyopuyo.Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
            IsGrounded = false;
        }

        public void KnowPartner(GameObject partner)
        {
            if (partner.GetComponent<Puyo>() == null) { throw new Exception("ぷよはぷよしかパートナーに選べません"); }
            this.partner = partner;
        }

        private void Start()
        {
            puyoBodyClock.NotifyBeginToFall();
        }

        public void ToFall()
        {
            State.ToFalling();
        }

        private void Update()
        {
            //FreeFall();
            UpdateAboutFall();
            //UpdateAboutTouch();
            //UpdateAboutStay();
        }

        /// <summary>
        /// 基本ぷよは落ちようとする
        /// </summary>
        private void FreeFall()
        {
            if (!IsGrounded && (State.IsJustTouch || State.IsTouching)) { ToFall(); }
        }

        private void UpdateAboutFall()
        {
            if (!State.IsFalling) { return; }
            puyoBodyClock.UpdateAboutFall();
            if (!puyoBodyClock.ShouldFallAction) { return; }
            AutoDown();
            puyoBodyClock.NotifyFinishFallAction();
        }

        private void UpdateAboutTouch()
        {
            if (!State.IsTouching) { return; }
            puyoBodyClock.UpdateAboutTouch();
        }

        private void UpdateAboutStay()
        {
            if (!State.IsTouching) { return; }
            if (!puyoBodyClock.ShouldStayAction) { return; }
            ToJustStay();
        }

        public void ToJustStay()
        {
            State.ToJustStay();
            puyoBodyClock.NotifyFinishStayAction();
        }

        private void AutoDown()
        {
            if (!State.IsFalling) { return; }
            transform.Translate(0, MOVE_FALL_AMOUNT, 0);
        } 

        public IEnumerator TouchAnimation()
        {
            collider.enabled = false;
            // Lerp でやりたいけど、もっというとアニメーターでやりたいから一旦仮置き
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.1f, 0.9f, 1.1f);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);
            transform.localScale = new Vector3(1.2f, 0.8f, 1.1f);
            yield return new WaitForSeconds(0.02f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.1f, 0.9f, 1.1f);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.1f, 0.9f, 1.1f);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);
            transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);
            yield return new WaitForSeconds(0.02f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.1f, 0.9f, 1.1f);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1, 1, 1);
            collider.enabled = true;
            yield return null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!State.IsFalling) { return; }
            if (gameObject.transform.position.y < collision.transform.position.y) { return; }
            if (IsPartner(collision.gameObject)) { return; }
            ToJustTouch();
            IsGrounded = true;
            // パートナーがいる場合は PuyoController でアニメーションを同期すべきか判断させる
            if (!hasPartner) { AnimateTouch(); }
        }

        private bool ShouldToJustTouch(GameObject gameObj)
        {
            if (!IsPartner(gameObj)) { return true; }
            if (IsVerticalWithPartner() && gameObj.GetComponent<Puyo>().IsGrounded) { return true; }
            return false;
        }

        private bool IsPartner(GameObject gameObj)
        {
            return ReferenceEquals(partner, gameObj);
        }

        public bool IsVerticalWithPartner()
        {
            return gameObject.transform.position.x == partner.transform.position.x;
        }

        public void ToJustTouch()
        {
            if (State.IsJustTouch) { return; }
            State.ToJustTouch();
        }

        public void AnimateTouch()
        {
            StartCoroutine(TouchAnimation());
        }

        public void TryToKeepTouching()
        {
            puyoBodyClock.NotifyBeginToTouch();
            State.ToTouching();
        }
    }
}