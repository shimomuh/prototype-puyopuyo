using System.Collections.Generic;
using UnityEngine;

namespace Puyopuyo.Domain
{
    public class PuyoCollision
    {
        private Dictionary<string, Collider> dict;

        public PuyoCollision()
        {
            dict = new Dictionary<string, Collider>()
            {
                { Vector3.left.ToString(), null },
                { Vector3.right.ToString(), null },
                { new Vector3(0, UI.Puyo.AMOUNT_TO_FALL_UNDER_CONTROLL, 0).ToString(), null },
                { new Vector3(0, UI.Puyo.AMOUNT_TO_FREE_FALL, 0).ToString(), null }
            };
        }

        public Collider GetCollider(Vector3 vector)
        {
            if (!dict.ContainsKey(vector.ToString()))
            {
                Debug.Log(vector);
            }
            return dict[vector.ToString()];
        }

        public void SetCollider(Vector3 vector, Collider collider)
        {
            dict[vector.ToString()] = collider;
        }
    }
}