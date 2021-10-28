using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puyopuyo.UI {
    public class PuyopuyoFactory : MonoBehaviour
    {
        [SerializeField] private GameObject button;
        private Vector3 INITIAL_POSITION = new Vector3(0.5f, 12f, 0f);
        [SerializeField] private GameObject field;

        private void Awake() {
            AddEventTrigger(button);
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
            Puyopuyo.Application.PuyopuyoGenerator.Instance.Generate(field.transform, new Vector3(-0.5f, 4, 0));
        }
    }
}