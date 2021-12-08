using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Puyopuyo.UI {
    public interface IPuyo {
        Domain.IPuyoStateMachine State { get; }
        bool IsGrounded { get; }
        GameObject GameObject { get; }
        public IPuyo Partner { get; }
        Rigidbody Rigidbody { get; }
        void AdaptRandomMaterial();
        void RecognizePartner(IPuyo partner);
        void Stop();
        void Restart();
        void ToFall();
        void ToJustStay();
        void ToStay();
        void ToJustTouch();
        void ToCancelTouching();
        void TryToKeepTouching();
        bool IsVerticalWithPartner();
        void DoTouchAnimation();
        void DoPopAnimation();
        void ToLeft();
        void ToRight();
        void ToDown();
        void ForceMove(Vector3 position);
        void ForceChangeState();
    }
    public class Puyo : MonoBehaviour, IPuyo
    {
        private float moveFallAmount = -0.5f;
        private Domain.IPuyoBodyClock puyoBodyClock;
        public Domain.IPuyoStateMachine State { get; private set; }
        public bool IsGrounded { get; private set; }
        public GameObject GameObject => gameObject;
        public IPuyo Partner { get; private set; }
        private bool hasPartner => Partner != null;
        private new Collider collider;
        public Rigidbody Rigidbody { get; private set; }
        private bool isFreeFall;
        [SerializeField]
        private List<Material> materials;
        private Material[] adaptedMaterials;

        protected void Awake()
        {
            puyoBodyClock = new Domain.PuyoBodyClock();
            State = new Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            IsGrounded = false;
        }

        public void AdaptRandomMaterial()
        {
            var rand = UnityEngine.Random.Range(0, this.materials.Count);
            var materials = GetComponent<Renderer>().materials;
            materials[0] = this.materials[rand];
            GetComponent<Renderer>().materials = materials;
            adaptedMaterials = materials;
        }

        public void RecognizePartner(IPuyo partner)
        {
            this.Partner = partner;
        }

        private void Start()
        {
            State.ToFalling();
            puyoBodyClock.NotifyBeginToFall();
        }

        private void Update()
        {
            UpdateAboutFall();
            UpdateAboutTouch();
            UpdateAboutStay();
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

        public void Stop()
        {
            puyoBodyClock.Stop();
        }

        public void Restart()
        {
            puyoBodyClock.Restart();
        }

        public void ToFall()
        {
            if (State.IsFalling) { return; }
            State.ToFalling();
            puyoBodyClock.NotifyBeginToFall();
        }

        public void ToJustStay()
        {
            if (State.IsJustStay) { return; }
            State.ToJustStay();
            puyoBodyClock.NotifyFinishStayAction();
        }

        public void ToStay()
        {
            if (State.IsStaying) { return; }
            State.ToStaying();
            Rigidbody.isKinematic = false;
            isFreeFall = true;
            if (!IsGrounded) {
                if (IsVerticalWithPartner() && Partner.IsGrounded)
                {
                    IsGrounded = true;
                    return;
                }
                FreeFall();
            }
        }

        private void AutoDown()
        {
            if (!State.IsFalling) { return; }
            transform.Translate(0, moveFallAmount, 0);
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

        private GameObject Generate(Vector3 offset)
        {
            GameObject puyoSkelton = Resources.Load<GameObject>("Prefabs/Pop");
            GameObject puyoObj = Instantiate(puyoSkelton);
            puyoObj.name = puyoObj.name.Replace("(Clone)", "");
            puyoObj.transform.SetParent(transform);
            puyoObj.transform.position = transform.position + offset;
            puyoObj.transform.localScale = new Vector3(0f, 0f, 0f);
            puyoObj.GetComponent<Renderer>().materials = adaptedMaterials;
            return puyoObj;
        }

        public IEnumerator PopAnimation()
        {
            var pop1 = Generate(new Vector3(0.2f, 0.4f, 0f));
            var pop2 = Generate(new Vector3(0.4f, 0.2f, 0f));
            var pop3 = Generate(new Vector3(-0.3f, 0.3f, 0f));
            var pop4 = Generate(new Vector3(-0.5f, 0.1f, 0f));
            collider.enabled = false;
            // Lerp でやりたいけど、もっというとアニメーターでやりたいから一旦仮置き
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            yield return new WaitForSeconds(0.01f);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            yield return new WaitForSeconds(0.01f);
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            yield return new WaitForSeconds(0.01f);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            yield return new WaitForSeconds(0.1f);
            pop1.transform.localScale = new Vector3(1f, 1f, 1f);
            pop2.transform.localScale = new Vector3(1f, 1f, 1f);
            pop3.transform.localScale = new Vector3(1f, 1f, 1f);
            pop4.transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            yield return new WaitForSeconds(0.1f);
            pop1.transform.Translate(0.4f, 0.8f, 0f);
            pop2.transform.Translate(0.8f, 0.4f, 0f);
            pop3.transform.Translate(-0.7f, 0.5f, 0f);
            pop4.transform.Translate(-0.9f, 0.3f, 0f);
            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            yield return new WaitForSeconds(0.1f);
            pop1.transform.Translate(0.2f, 1.2f, 0f);
            pop2.transform.Translate(1.2f, 0.2f, 0f);
            pop3.transform.Translate(-1.1f, 0.3f, 0f);
            pop4.transform.Translate(-1.3f, 0.1f, 0f);
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            yield return new WaitForSeconds(0.1f);
            pop1.transform.localScale = new Vector3(0f, 0f, 0f);
            pop2.transform.localScale = new Vector3(0f, 0f, 0f);
            transform.localScale = new Vector3(0f, 0f, 0f);
            collider.enabled = true;
            yield return null;
            Destroy(this.GameObject);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var hitPosition = GetHitPoint(collision);
            if (!State.IsFalling) { return; }
            if (gameObject.transform.position.x != hitPosition.x) { return; }
            if (gameObject.transform.position.y < hitPosition.y) { return; }
            if (IsPartner(collision.gameObject)) { return; }
            if (isFreeFall)
            {
                DoTouchAnimation();
                State.ToStaying();
                return;
            }
            ToJustTouch();
            IsGrounded = true;
            Rigidbody.isKinematic = true; // 反発を防ぐ処理
            // パートナーがいる場合は PuyoController でアニメーションを同期すべきか判断させる
            if (!hasPartner) { DoTouchAnimation(); }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            var hitPosition = GetHitPoint(collision);
            if (!State.IsTouching) { return; }
            // collision.contacts の point が (0,0,0) になってしまうので床滑りしたときの対策
            if (gameObject.transform.position.y == collision.transform.position.y) { return; }
            if (IsPartner(collision.gameObject)) { return; } // そんなことないと思うけど
            ToCancelTouching();
            IsGrounded = false;
            Rigidbody.isKinematic = false;
        }

        private Vector3 GetHitPoint(Collision collision)
        {
            if (collision.contacts.Length > 1) { throw new Exception($"{collision.gameObject.name} が2点以上で交わっています"); }
            Vector3 hitPos = new Vector3();
            foreach (ContactPoint point in collision.contacts)
            {
                hitPos = point.point;
            }
            return hitPos;
        }

        private bool IsPartner(GameObject gameObj)
        {
            return ReferenceEquals(Partner.GameObject, gameObj);
        }

        public bool IsVerticalWithPartner()
        {
            // Skelton の場合 ToStay で実行時、Skelton は partner がいないので null を対処しておく
            if (Partner == null) { return false; }
            return gameObject.transform.position.x == Partner.GameObject.transform.position.x;
        }

        public void ToJustTouch()
        {
            if (State.IsJustTouch) { return; }
            State.ToJustTouch();
        }

        public void ToCancelTouching()
        {
            if (State.IsCancelTouching) { return; }
            puyoBodyClock.NotifyFinishStayAction();
            State.ToCancelTouching();
        }

        public void DoTouchAnimation()
        {
            StartCoroutine(TouchAnimation());
        }

        public void DoPopAnimation()
        {
            StartCoroutine(PopAnimation());
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
            transform.Translate(0, moveFallAmount, 0);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void ForceMove(Vector3 position)
        {
            transform.position = position;
        }

        private void FreeFall()
        {
            State.ToFalling();
            isFreeFall = true;
            moveFallAmount = -0.2f;
            puyoBodyClock.NotifyBeginToFreeFall();
        }

        /// <summary>
        /// 強制的に Raycast を使って状態を変化させる
        /// isKinematic = true にして回転したあとに false にした場合などは
        /// OnCollisionEnter や OnCollisionExit が実行されないのでその救済措置
        /// </summary>
        public void ForceChangeState()
        {
            var hasCollision = Physics.Raycast(transform.position, Vector3.down, 0.4f);
            if (hasCollision) {
                if (State.IsFalling) {
                    ToJustTouch();
                    TryToKeepTouching();
                    DoTouchAnimation();
                    return;
                }
                return;
            }
            if (State.IsTouching)
            {
                ToCancelTouching();
            }
        }
    }
}