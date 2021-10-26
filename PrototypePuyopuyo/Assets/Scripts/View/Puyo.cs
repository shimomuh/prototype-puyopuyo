using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class Puyo : MonoBehaviour
    {
        private float fallSpeed = 1f; // Factory時に外から指定された方がいい
        private int putOnFrame = 60; // 外から指定されるべき

        void OnUpdate()
        {
            transform.localPosition += new Vector3(0, fallSpeed, 0);
        }
    }
}