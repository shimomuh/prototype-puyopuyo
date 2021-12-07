using System.Collections.Generic;
using com.amabie.SingletonKit;
using Puyopuyo.UI;
using UnityEngine;

namespace Puyopuyo.Application
{
    public class PuyoChainController : SingletonMonoBehaviour<PuyoChainController>
    {
        private Transform field;
        private List<IPuyo> puyoes;

        private void Start()
        {
            field = GameObject.FindGameObjectWithTag("Field").transform;
        }

        public void Check()
        {
            int puyoCount = 0;
            puyoes = new List<IPuyo>();
            foreach (Transform puyo in field)
            {
                if (puyo.GetComponent<IPuyo>() == null) { continue; }
                if (puyo.GetComponent<SkeltonCollider>() != null) { continue; }
                puyoes.Add(puyo.GetComponent<IPuyo>());
                puyoCount++;
            }

            if (puyoCount >= 4) {
                foreach (var puyo in puyoes)
                {
                    puyo.DoPopAnimation();
                }
            }
        }
    }
}