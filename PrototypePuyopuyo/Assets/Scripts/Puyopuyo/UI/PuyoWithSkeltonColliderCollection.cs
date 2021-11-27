using UnityEngine;

namespace Puyopuyo.UI
{
    public interface IPuyoWithSkeltonColliderCollection
    {
        IPuyo Puyo { get; }
        bool IsFalling { get; }
        bool IsJustTouch { get; }
        bool IsTouching { get; }
        bool IsCancelTouching { get; }
        bool IsJustStay { get; }
        void RecognizePartner(IPuyoWithSkeltonColliderCollection partner);
        bool CanToLeft();
        bool CanToRight();
        bool CanToDown();
        void ToLeft();
        void ToRight();
        void ToDown();
        void LerpRotate(Vector3 position);
        void ToJustTouch();
        void TryToKeepTouching();
        void ToCancelTouching();
        void ToFall();
        void ToJustStay();
        void ToStay();
        void Dispose();
    }

    /// <summary>
    /// ぷよと透明コライダー集合体をまとめるクラス
    /// ぷよだけを操作したい場合は Puyo プロパティに委譲
    /// 透明コライダーは単体で操作させない
    /// 基本はぷよと透明コライダー、もしくは透明コライダーにだけ操作したいインターフェースを持つ
    /// </summary>
    public class PuyoWithSkeltonColliderCollection : IPuyoWithSkeltonColliderCollection
    {
        public IPuyo Puyo => puyo;
        public bool IsFalling => puyo.State.IsFalling;
        public bool IsJustTouch => puyo.State.IsJustTouch;
        public bool IsTouching => puyo.State.IsTouching;
        public bool IsCancelTouching => puyo.State.IsCancelTouching;
        public bool IsJustStay => puyo.State.IsJustStay;

        private IPuyo puyo;
        private ISkeltonColliderCollection skeltonColliderCollection;

        public PuyoWithSkeltonColliderCollection(IPuyo puyo, ISkeltonColliderCollection skeltonColliderCollection)
        {
            this.puyo = puyo;
            this.skeltonColliderCollection = skeltonColliderCollection;
        }

        public void RecognizePartner(IPuyoWithSkeltonColliderCollection partner)
        {
            puyo.RecognizePartner(partner.Puyo);
        }

        public bool CanToLeft()
        {
            return skeltonColliderCollection.CanToLeft();
        }

        public bool CanToRight()
        {
            return skeltonColliderCollection.CanToRight();
        }

        public bool CanToDown()
        {
            return skeltonColliderCollection.CanToDown();
        }

        public void ToLeft()
        {
            puyo.ToLeft();
            skeltonColliderCollection.ToLeft();
        }

        public void ToRight()
        {
            puyo.ToRight();
            skeltonColliderCollection.ToRight();
        }

        public void ToDown()
        {
            puyo.ToDown();
            skeltonColliderCollection.ToDown();
        }

        public void LerpRotate(Vector3 position)
        {
            puyo.ForceMove(position);
            skeltonColliderCollection.LerpRotate(position);
        }

        public void ToJustTouch()
        {
            puyo.ToJustTouch();
            skeltonColliderCollection.ToJustTouch();
        }

        public void TryToKeepTouching()
        {
            puyo.TryToKeepTouching();
            skeltonColliderCollection.TryToKeepTouching();
        }

        public void ToCancelTouching()
        {
            puyo.ToCancelTouching();
            skeltonColliderCollection.ToCancelTouching();
        }

        public void ToFall()
        {
            puyo.ToFall();
            skeltonColliderCollection.ToFall();
        }

        public void ToJustStay()
        {
            puyo.ToJustStay();
            skeltonColliderCollection.ToJustStay();
        }

        public void ToStay()
        {
            puyo.ToStay();
            skeltonColliderCollection.ToStay();
        }

        public void Dispose()
        {
            skeltonColliderCollection.Dispose();
            puyo = null;
            skeltonColliderCollection = null;
        }
    }
}