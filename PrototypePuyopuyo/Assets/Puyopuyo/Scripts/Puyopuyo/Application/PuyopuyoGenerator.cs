using com.amabie.SingletonKit;
using UnityEngine;

namespace Puyopuyo.Application {
    public class PuyopuyoGenerator : SingletonMonoBehaviour<PuyopuyoGenerator>
    {
        public void Generate(Transform fieldTransform, Vector3 initialPosition, UI.PuyoMaterial cMaterial = UI.PuyoMaterial.Random, UI.PuyoMaterial fMaterial = UI.PuyoMaterial.Random)
        {
            var controller = PuyoGenerator.Instance.Generate(fieldTransform, initialPosition, cMaterial);
            var follower = PuyoGenerator.Instance.Generate(fieldTransform, initialPosition + new Vector3(0, 1, 0), fMaterial);
            PuyoController.Instance.Observe(controller, follower);
        }
    }
}
