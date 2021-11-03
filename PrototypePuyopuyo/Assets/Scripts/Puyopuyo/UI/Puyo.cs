using System.Collections;
using UnityEngine;
using System;

namespace Puyopuyo.UI {
    public interface IPuyo {
        Domain.IPuyoStateMachine State { get; }
        bool IsGrounded { get; }
        GameObject GameObject { get; }
        void RecognizePartner(GameObject gameObj);
        void ToFall();
        void ToJustStay();
        void ToStay();
        void ToJustTouch();
        void TryToKeepTouching();
        bool IsVerticalWithPartner();
        void DoTouchAnimation();
        void ToLeft();
        void ToRight();
        void ToDown();
    }
    public class Puyo : MonoBehaviour, IPuyo
    {
        private float MOVE_FALL_AMOUNT = -0.5f;
        private Domain.IPuyoBodyClock puyoBodyClock;
        public Domain.IPuyoStateMachine State { get; private set; }
        public bool IsGrounded { get; private set; }
        public GameObject GameObject => gameObject;
        private GameObject partner;
        private bool hasPartner => partner != null;
        private new Collider collider;
        private new Rigidbody rigidbody;

        private void Awake()
        {
            puyoBodyClock = new Domain.PuyoBodyClock();
            State = new Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
            rigidbody = gameObject.GetComponent<Rigidbody>();
            IsGrounded = false;
        }

        public void RecognizePartner(GameObject partner)
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
            UpdateAboutTouch();
            UpdateAboutStay();
        }

        /// <summary>
        /// 基本ぷよは落ちようとする
        /// </summary>
        private void FreeFall()
        {
            if (IsGrounded) { return; }
            if (!State.IsJustStay) { return; }
            ToFall();
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

        public void ToStay()
        {
            State.ToStaying();
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

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var hitPosition = GetHitPoint(collision);
            if (!State.IsFalling) { return; }
            if (gameObject.transform.position.x != hitPosition.x) { return; }
            if (gameObject.transform.position.y < hitPosition.y) { return; }
            if (IsPartner(collision.gameObject)) { return; }
            ToJustTouch();
            IsGrounded = true;
            rigidbody.isKinematic = true; // 反発を防ぐ処理
            // パートナーがいる場合は PuyoController でアニメーションを同期すべきか判断させる
            if (!hasPartner) { DoTouchAnimation(); }
        }

        private Vector3 GetHitPoint(Collision collision)
        {
            if (collision.contacts.Length != 1) { throw new Exception("1点以上で交わっています"); }
            Vector3 hitPos = new Vector3();
            foreach (ContactPoint point in collision.contacts)
            {
                hitPos = point.point;
            }
            return hitPos;
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

        public void DoTouchAnimation()
        {
            StartCoroutine(TouchAnimation());
        }

        public void TryToKeepTouching()
        {
            puyoBodyClock.NotifyBeginToTouch();
            State.ToTouching();
        }

        public void ToLeft()
        {
            transform.Translate(-1, 0, 0);
        }

        public void ToRight()
        {
            transform.Translate(1, 0, 0);
        }

        public void ToDown()
        {
            transform.Translate(0, MOVE_FALL_AMOUNT, 0);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}