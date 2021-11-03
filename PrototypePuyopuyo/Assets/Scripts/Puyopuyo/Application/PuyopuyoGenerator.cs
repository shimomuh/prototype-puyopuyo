using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyopuyoGenerator : SingletonMonoBehaviour<PuyopuyoGenerator>
    {
        public void Generate(Transform fieldTransform, Vector3 initialPosition)
        {
            var controller = PuyoGenerator.Instance.Generate(fieldTransform, initialPosition);
            var follower = PuyoGenerator.Instance.Generate(fieldTransform, initialPosition + new Vector3(0, 1, 0));
            var skeltonCollection = SkeltonColliderCollectionGenerator.Instance.Generate(fieldTransform, initialPosition);
            PuyoController.Instance.Observe(controller, follower, skeltonCollection);
        }
    }
}
