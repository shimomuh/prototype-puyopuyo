using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Puyopuyo.UI.Debug {
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject field;
        private Dictionary<int, PuyoHUDObject> puyoHUDDict;
        private readonly Vector3 OFFSET = new Vector3(0, 0, 0);

        private void Awake()
        {
            puyoHUDDict = new Dictionary<int, PuyoHUDObject>();
        }

        private void Update() {
            if (field.transform.childCount == 0) { return; }
            AddPuyoHUDDict();
            UpdateHUDPosition();
            UpdateHUDText();
        }

        private void AddPuyoHUDDict()
        {
            foreach (Transform child in field.transform)
            {
                if (puyoHUDDict.ContainsKey(child.gameObject.GetInstanceID())) { continue; }
                var textObj = new GameObject("PuyoHUD");
                var text = textObj.AddComponent<Text>();
                textObj.transform.SetParent(gameObject.transform);
                text.fontSize = 20;
                text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text.alignment = TextAnchor.MiddleCenter;
                puyoHUDDict[child.gameObject.GetInstanceID()] = new PuyoHUDObject(child.gameObject, textObj);
            }
        }

        private void UpdateHUDPosition()
        {
            foreach(Transform child in field.transform)
            {
                var puyoHUDObject = puyoHUDDict[child.gameObject.GetInstanceID()];
                puyoHUDObject.HUD.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, puyoHUDObject.Puyo.transform.position + OFFSET);
            }
        }

        private void UpdateHUDText()
        {
            foreach (var kvp in puyoHUDDict)
            {
                var state = kvp.Value.Puyo.GetComponent<Puyo>().State.ToString();
                kvp.Value.HUD.GetComponent<Text>().text = state.Substring(0, 1);
            }
        }

        public struct PuyoHUDObject
        {
            public GameObject Puyo { get; private set; }
            public GameObject HUD { get; private set; }
            public PuyoHUDObject(GameObject puyo, GameObject hud)
            {
                Puyo = puyo;
                HUD = hud;
            }
        }
    }
}