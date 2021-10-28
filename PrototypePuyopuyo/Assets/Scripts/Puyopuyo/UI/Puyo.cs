using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Puyopuyo.UI {
    public class Puyo : MonoBehaviour
    {
        private float MOVE_FALL_AMOUNT = -0.5f;
        private float MOVE_FALL_WAITING_SECONDS = 1f;
        private float MOVE_TOUCH_WAITING_SECONDS = 1f;
        private Puyopuyo.Domain.IClock fallClock;
        private Puyopuyo.Domain.IClock touchClock;
        public Puyopuyo.Domain.IPuyoStateMachine State { get; private set; }
        private Collider collider;
        private string CONTROLLING_TAG = "ControllingPuyo";
        private string OBJECT_TAG = "ObjectPuyo";

        private void Awake()
        {
            gameObject.tag = CONTROLLING_TAG;
            fallClock = new Puyopuyo.Domain.Clock(MOVE_FALL_WAITING_SECONDS);
            fallClock.StartTime();
            touchClock = new Puyopuyo.Domain.Clock(MOVE_TOUCH_WAITING_SECONDS);
            touchClock.StopTime();
            State = new Puyopuyo.Domain.PuyoStateMachine();
            collider = gameObject.GetComponent<Collider>();
        }

        private void Update()
        {
            UpdateAboutFall();
            UpdateAboutTouch();
        }

        private void UpdateAboutFall()
        {
            if (!fallClock.CanTikTok) { return; }
            fallClock.TikTok();
            if (fallClock.IsRing) {
                fallClock.StopRing();
                AutoDown();
            }
        }

        private void UpdateAboutTouch()
        {
            if (!touchClock.CanTikTok) { return; }
            touchClock.TikTok();
            if (touchClock.IsRing) {
                touchClock.StopTime();
                touchClock.ResetAll();
                State.ToStay();
                gameObject.tag = OBJECT_TAG;
                StartCoroutine(StayAnimation());
            }
        }

        private void AutoDown()
        {
            if (!State.CanFall()) { return; }
            transform.Translate(0, MOVE_FALL_AMOUNT, 0);
        } 

        private IEnumerator StayAnimation()
        {
            collider.enabled = false;
            // Lerp でやりたいけど、もっというとアニメーターでやりたいから一旦仮置き
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.25f, 0.9f, 1.25f);
            yield return new WaitForSeconds(0.1f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);
            transform.localScale = new Vector3(1.5f, 0.8f, 1.5f);
            yield return new WaitForSeconds(0.2f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1.25f, 0.9f, 1.25f);
            yield return new WaitForSeconds(0.1f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z);            
            transform.localScale = new Vector3(1, 1, 1);
            collider.enabled = true;
            yield return null;
        }

        public void ToTouch()
        {
            if (State.IsTouch) { return; }
            State.ToTouch();
            fallClock.ResetAll();
            touchClock.ResetAll();
            touchClock.StartTime();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.tag == CONTROLLING_TAG) { return; }
            if (State.IsStay) { return; }
            ToTouch();
        }
    }
}