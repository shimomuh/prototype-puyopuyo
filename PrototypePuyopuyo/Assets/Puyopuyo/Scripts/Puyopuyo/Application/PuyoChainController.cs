using System.Collections.Generic;
using com.amabie.SingletonKit;
using Puyopuyo.UI;
using UnityEngine;

namespace Puyopuyo.Application
{
    public class PuyoChainController : SingletonMonoBehaviour<PuyoChainController>
    {
        private Transform field;
        private IPuyo[][] puyoMatrix;
        private int[][] checkMatrix;
        private static readonly int VISIBLE_ROW_COUNT = 12;
        private static readonly int UNVISIBLE_ROW_COUNT = 2;
        private static readonly int ROW_COUNT = VISIBLE_ROW_COUNT + UNVISIBLE_ROW_COUNT; // 縦
        private static readonly int COLUMN_COUNT = 6; // 横
        private static readonly int OFFSET_X = 2;

        private void Start()
        {
            field = GameObject.FindGameObjectWithTag("Field").transform;
        }

        private void Initialize()
        {
            puyoMatrix = new IPuyo[VISIBLE_ROW_COUNT][];
            checkMatrix = new int[VISIBLE_ROW_COUNT][];
            for (var i = 0; i < VISIBLE_ROW_COUNT; i++)
            {
                puyoMatrix[i] = new IPuyo[COLUMN_COUNT];
                checkMatrix[i] = new int[COLUMN_COUNT];
                for (var j = 0; j < COLUMN_COUNT; j++)
                {
                    puyoMatrix[i][j] = null;
                    checkMatrix[i][j] = -1;
                }
            }
        }

        public void Update()
        {
            if (!ShouldCheckState()) { return; }
            Check();
            Check();
        }

        private bool ShouldCheckState()
        {
            foreach (Transform puyo in field)
            {
                if (puyo.GetComponent<IPuyo>() == null) { continue; }
                if (puyo.GetComponent<SkeltonCollider>() != null) { continue; }
                if (!puyo.GetComponent<IPuyo>().State.IsStaying) { return false; }
            }
            return true;
        }

        private void Check()
        {
            int deleteColorIndex = -1;
            Initialize();
            foreach (Transform puyo in field)
            {
                if (puyo.GetComponent<IPuyo>() == null) { continue; }
                if (puyo.GetComponent<SkeltonCollider>() != null) { continue; }
                puyoMatrix[(int)puyo.transform.position.y][(int)puyo.transform.position.x + OFFSET_X] = puyo.GetComponent<IPuyo>();
            }
            
            for (var i = 0; i < VISIBLE_ROW_COUNT; i++)
            {
                for (var j = 0; j < COLUMN_COUNT; j++)
                {
                    int count = 0;
                    RecursiveCheck(i, j, ref count);
                    if (count >= 4)
                    {
                        deleteColorIndex = puyoMatrix[i][j].MaterialIndex;
                        break;
                    }
                }
            }

            if (deleteColorIndex == -1) { return; }
            for (var i = 0; i < VISIBLE_ROW_COUNT; i++)
            {
                for (var j = 0; j < COLUMN_COUNT; j++)
                {
                    if (checkMatrix[i][j] != deleteColorIndex) { continue; }
                    puyoMatrix[i][j].DoPopAnimation();
                }
            }
        }

        private void RecursiveCheck(int y, int x, ref int count)
        {
            if (puyoMatrix[y][x] == null) { return; }
            if (checkMatrix[y][x] == puyoMatrix[y][x].MaterialIndex) { return; }
            checkMatrix[y][x] = puyoMatrix[y][x].MaterialIndex; count++;
            var colorIndex = puyoMatrix[y][x].MaterialIndex;
            if (x - 1 >= 0 && puyoMatrix[y][x - 1] != null && puyoMatrix[y][x - 1].MaterialIndex == colorIndex) { RecursiveCheck(y, x - 1, ref count); }
            if (x + 1 < COLUMN_COUNT && puyoMatrix[y][x + 1] != null && puyoMatrix[y][x + 1].MaterialIndex == colorIndex) { RecursiveCheck(y, x + 1, ref count); }
            if (y - 1 >= 0 && puyoMatrix[y - 1][x] != null && puyoMatrix[y - 1][x].MaterialIndex == colorIndex) { RecursiveCheck(y - 1, x, ref count); }
            if (y + 1 < VISIBLE_ROW_COUNT && puyoMatrix[y + 1][x] != null && puyoMatrix[y + 1][x].MaterialIndex == colorIndex) { RecursiveCheck(y + 1, x, ref count); }
        }
    }
}