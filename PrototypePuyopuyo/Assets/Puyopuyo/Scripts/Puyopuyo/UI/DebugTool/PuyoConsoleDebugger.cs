using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puyopuyo.UI.DebugTool
{
    public class PuyoConsoleDebugger : MonoBehaviour
    {
        [SerializeField] private GameObject field;

        private void Awake()
        {
            AddEventTrigger(field);
        }

        public void AddEventTrigger(GameObject gameObj)
        {
            EventTrigger currentTrigger = gameObj.AddComponent<EventTrigger>();
            currentTrigger.triggers = new List<EventTrigger.Entry>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((x) => OnClick());
            currentTrigger.triggers.Add(entry);
        }

        public void OnClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                var gameObj = hit.collider.gameObject;
                var puyo = gameObj.GetComponent<IPuyo>();
                if (puyo == null) { return; }
                UnityEngine.Debug.Log($"position : {puyo.GameObject.transform.position}");
                UnityEngine.Debug.Log($"state : {puyo.State}");
                UnityEngine.Debug.Log($"isKinematic? : {puyo.Rigidbody.isKinematic}");
                foreach (var dict in puyo.PuyoCollision.Dict) {
                    UnityEngine.Debug.Log($"colliders: {dict.Key}, {dict.Value}");
                }
            }
        }
    }
}