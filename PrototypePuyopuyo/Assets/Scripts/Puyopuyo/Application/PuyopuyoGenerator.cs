using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyopuyoGenerator : SingletonMonoBehaviour<PuyopuyoGenerator>
    {
        public void Generate(Transform fieldTransform, Vector3 initialPosition)
        {
            var controller = PuyoWithSkeltonColliderCollectionGenerator.Instance.Generate(fieldTransform, initialPosition);
            var follower = PuyoWithSkeltonColliderCollectionGenerator.Instance.Generate(fieldTransform, initialPosition + new Vector3(0, 1, 0));
            PuyoController.Instance.Observe(controller, follower);
        }
    }
}
