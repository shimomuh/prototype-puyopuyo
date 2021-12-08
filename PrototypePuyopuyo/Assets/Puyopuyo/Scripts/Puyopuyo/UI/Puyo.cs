using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Puyopuyo.UI {
    public interface IPuyo {
        Vector3 VectorToFallDown { get; }

        Domain.IPuyoStateMachine State { get; }

        GameObject GameObject { get; }
        public IPuyo Partner { get; }
        Rigidbody Rigidbody { get; }
        int MaterialIndex { get; }

        void AdaptRandomMaterial();
        void UnderControllWith(IPuyo partner);

        void UpdatePerFrame();

        void Stop();
        void Restart();
        void ToFall();
        void ToJustStay();
        void ToStay();
        void ToJustTouch();
        void ToCanceling();
        void TryToKeepTouching();

        void DoTouchAnimation();
        void DoPopAnimation();

        void ToLeft();
        void ToRight();
        void ToDown();
        void ForceToMove(Vector3 position);

        bool CanMoveToLeft();
        bool CanMoveToRight();
        bool CanMoveToDown();

        float HeightToGround();
    }
    public class Puyo : MonoBehaviour, IPuyo
    {
        /// <summary>
        /// 落ちる量
        /// その時の状態によって落ちる量は違うので常に amountToFall を参照する
        /// </summary>
        public Vector3 VectorToFallDown
        {
            get
            {
                return new Vector3(0f, amountToFall, 0f);
            }
        }
        private float amountToFall;
        public static float AMOUNT_TO_FALL_UNDER_CONTROLL = -0.5f;
        public static float AMOUNT_TO_FREE_FALL = -0.2f;

        private Domain.IPuyoBodyClock puyoBodyClock;
        public Domain.IPuyoStateMachine State { get; private set; }

        public GameObject GameObject => gameObject;
        public Rigidbody Rigidbody { get; private set; }
        private new Collider collider;
        public IPuyo Partner { get; private set; }
        private bool hasPartner => Partner != null;
        public int MaterialIndex { get; private set; }

        private bool isFreeFall;
        private Domain.PuyoCollision puyoCollision;

        [SerializeField]
        private List<Material> materials;
        private Material[] adaptedMaterials;

        protected void Awake()
        {
            puyoBodyClock = new Domain.PuyoBodyClock();
            State = new Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            isFreeFall = true;
            puyoCollision = new Domain.PuyoCollision();
            amountToFall = AMOUNT_TO_FREE_FALL;
        }

        public void AdaptRandomMaterial()
        {
            MaterialIndex = UnityEngine.Random.Range(0, this.materials.Count);
            var materials = GetComponent<Renderer>().materials;
            materials[0] = this.materials[MaterialIndex];
            GetComponent<Renderer>().materials = materials;
            adaptedMaterials = materials;
        }

        public void UnderControllWith(IPuyo partner)
        {
            amountToFall = AMOUNT_TO_FALL_UNDER_CONTROLL;
            isFreeFall = false;
            this.Partner = partner;
        }

        private void Start()
        {
            State.ToFalling();
            puyoBodyClock.NotifyBeginToFall();
        }

        private void Update()
        {
            if (!isFreeFall) { return; }
            UpdatePerFrame();
        }

        public void UpdatePerFrame()
        {
            UpdateBodyClockIfNeeded();
            UpdateCollision();
            ChangeState();
        }

        /// <summary>
        /// 体内時計の時間を更新する
        /// お約束: State の更新はしない
        /// </summary>
        private void UpdateBodyClockIfNeeded()
        {
            UpdateFallClockAndActionIfNeeded();
            UpdateTouchClock();
        }

        #region UpdateBodyClockIfNeeded

        private void UpdateFallClockAndActionIfNeeded()
        {
            if (!State.IsFalling) { return; }
            puyoBodyClock.UpdateAboutFall();
            if (!puyoBodyClock.ShouldFallAction) { return; }
            AutoDown();
            puyoBodyClock.NotifyFinishFallAction();
        }

        private void UpdateTouchClock()
        {
            if (!State.IsTouching) { return; }
            puyoBodyClock.UpdateAboutTouch();
        }

        #endregion

        /// <summary>
        /// State を変更する
        /// 1フレームでこのメソッド内では1以上のステートには変化しない
        /// </summary>
        private void ChangeState()
        {
            if (isFreeFall)
            {
                ChangeStateForFreeFall();
                return;
            }
            ChangeStateUnderControll();
        }

        #region ChangeState

        /// 自由落下時
        ///   Falling -> JustTouch -> Staying
        ///   -> (collisionがなくなったら) -> Falling
        private void ChangeStateForFreeFall()
        {
            if (State.IsFalling)
            {
                if (!CanMoveToDown())
                {
                    ToJustTouch();
                    DoTouchAnimation();
                }
                return;
            }
            if (State.IsJustStay)
            {
                ToStay();
                return;
            }
            if (State.IsStaying)
            {
                if (CanMoveToDown())
                {
                    FreeFall();
                }
            }
        }

        /// 操作時
        ///   Falling -> JustTouch -> (Controller 側で同期して ToTouching) -> JustStay
        ///   -> (Controller 側で同期して ToStaying) -> Staying
        ///
        /// CancelTouching を伴う場合は
        ///   Touching -> (Controller 側で同期して CancelTouching)
        ///   -> Touching or Falling
        private void ChangeStateUnderControll()
        {
            if (State.IsFalling)
            {
                if (!CanMoveToDown())
                {
                    ToJustTouch();
                    DoTouchAnimation();
                }
                return;
            }
            if (State.IsJustTouch)
            {
                return;
            }
            if (State.IsTouching)
            {
                ToJustStayIfNeeded();
                return;
            }
            if (State.IsCanceling)
            {
                return;
            }
            if (State.IsJustStay)
            {
                return;
            }
            if (State.IsStaying)
            {
                ToStay();
            }
        }

        private void ToJustStayIfNeeded()
        {
            if (!puyoBodyClock.ShouldStayAction) { return; }
            ToJustStay();
        }

        #endregion

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
            // 保留。反発を防ぐためにつけてたけどいらなくなるかもしれないので。
            Rigidbody.isKinematic = false;
            isFreeFall = true;
            amountToFall = AMOUNT_TO_FREE_FALL;
            Partner = null;
        }

        private void AutoDown()
        {
            if (!State.IsFalling) { return; }
            transform.Translate(0, amountToFall, 0);
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

        public void ToJustTouch()
        {
            if (State.IsJustTouch) { return; }
            State.ToJustTouch();
        }

        public void ToCanceling()
        {
            if (State.IsCanceling) { return; }
            puyoBodyClock.NotifyFinishStayAction();
            State.ToCanceling();
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

        public void ToLeft() => MoveTo(Vector3.left);
        public void ToRight() => MoveTo(Vector3.right);
        public void ToDown() => MoveTo(new Vector3(0, amountToFall, 0));
        private void MoveTo(Vector3 vector) => transform.Translate(vector);
        public void ForceToMove(Vector3 position)
        {
            transform.position = position;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void FreeFall()
        {
            State.ToFalling();
            puyoBodyClock.NotifyBeginToFreeFall();
        }

        /// <summary>
        /// 強制的に Raycast を使って状態を変化させる
        /// isKinematic = true にして回転したあとに false にした場合などは
        /// OnCollisionEnter や OnCollisionExit が実行されないのでその救済措置
        /// </summary>
        private void ForceChangeState()
        {
            var hasCollision = Physics.Raycast(transform.position, Vector3.down, VectorToFallDown.magnitude - 0.1f);
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
                ToCanceling();
            }
        }

        private void UpdateCollision()
        {
            var feelVectors = new Vector3[] {
                Vector3.left,
                Vector3.right,
                VectorToFallDown
            };
            foreach (var vector in feelVectors)
            {
                FeelAroundFor(vector);
            }
        }

        private void FeelAroundFor(Vector3 direction)
        {
            var hasCollision = Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.9f);
            if (!hasCollision)
            {
                puyoCollision.SetCollider(direction, null);
                return;
            }
            if (IsPartner(hit.collider.gameObject)) { return; }
            puyoCollision.SetCollider(direction, hit.collider);
        }


        public bool CanMoveToLeft() => CanMoveTo(Vector3.left);
        public bool CanMoveToRight() => CanMoveTo(Vector3.right);
        public bool CanMoveToDown() => CanMoveTo(VectorToFallDown);

        /// <summary>
        /// 指定方向に移動できるかどうか
        /// </summary>
        private bool CanMoveTo(Vector3 vector)
        {
            var collider = puyoCollision.GetCollider(vector);
            return collider == null || IsPartner(collider.gameObject);
        }

        private bool IsPartner(GameObject gameObj)
        {
            if (Partner == null) { return false; }
            return ReferenceEquals(Partner.GameObject, gameObj);
        }

        public float HeightToGround()
        {
            var collider = puyoCollision.GetCollider(Vector3.down);
            UnityEngine.Debug.Log("衝突物のpos: " + collider.gameObject.transform.position);
            UnityEngine.Debug.Log("自分のpos: " + transform.position);
            UnityEngine.Debug.Log("closestPoint: " + collider.ClosestPointOnBounds(transform.position));
            UnityEngine.Debug.Log("closestPoint2: " + collider.ClosestPointOnBounds(collider.gameObject.transform.position));
            return 0.5f;
        }
    }
}