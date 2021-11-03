using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puyopuyo.UI.Debug {
    public class PuyopuyoFactory : MonoBehaviour
    {
        private float INITIAL_Y_POSITION = 12f;
        [SerializeField] private GameObject button;
        [SerializeField] private GameObject field;
        [SerializeField] private InputField inputField;

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
            float y = INITIAL_Y_POSITION;
            if (inputField.text != "")
            {
                float.TryParse(inputField.text, out y);
            }
            Application.PuyopuyoGenerator.Instance.Generate(field.transform, new Vector3(0, y, 0));
        }
    }
}