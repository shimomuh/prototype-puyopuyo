using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyopuyoGenerator : SingletonMonoBehaviour<PuyopuyoGenerator>
    {

        public void Generate(Transform parentTransform, Vector3 initialPosition)
        {
            var controller = Puyopuyo.Application.PuyoGenerator.Instance.Generate(parentTransform, initialPosition);
            var follower = Puyopuyo.Application.PuyoGenerator.Instance.Generate(parentTransform, initialPosition + new Vector3(0, 1, 0));
            //Puyopuyo.Application.SkeltonColliderCollectionGenerator.Instance.Generate(parentTransform, initialPosition);
            Puyopuyo.Application.PuyoController.Instance.Observe(controller, follower);
        }
    }
}
