using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application
{
    public class PuyoWithSkeltonColliderCollectionGenerator: Singleton<PuyoWithSkeltonColliderCollectionGenerator>
    {
        public UI.IPuyoWithSkeltonColliderCollection Generate(Transform fieldTransform, Vector3 initialPosition)
        {
            var puyo = PuyoGenerator.Instance.Generate(fieldTransform, initialPosition);
            var skeltonColliderCollection = SkeltonColliderCollectionGenerator.Instance.Generate(fieldTransform, initialPosition, puyo);
            return new UI.PuyoWithSkeltonColliderCollection(puyo, skeltonColliderCollection);
        }
    }
}