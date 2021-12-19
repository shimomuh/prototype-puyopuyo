using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puyopuyo.UI.DebugTool
{
    public class PuyopuyoFactory : MonoBehaviour
    {
        private float INITIAL_Y_POSITION = 12f;
        [SerializeField] private GameObject button;
        [SerializeField] private GameObject field;
        [SerializeField] private InputField inputField;
        [SerializeField] private GameObject blueButton;
        [SerializeField] private GameObject greenButton;
        [SerializeField] private GameObject purpleButton;
        [SerializeField] private GameObject redButton;
        [SerializeField] private GameObject yellowButton;
        [SerializeField] private GameObject randomButton;
        PuyoMaterial[] puyoMaterials;
        int puyoMaterialCounter = 0;

        private void Awake() {
            Application.PuyopuyoGenerator.Instance.Generate(field.transform, new Vector3(0, 0.5f, 0));
            AddEventTrigger(button, OnClick);
            AddEventTrigger(blueButton, () => OnClickPuyoColorButton(PuyoMaterial.Blue));
            AddEventTrigger(greenButton, () => OnClickPuyoColorButton(PuyoMaterial.Green));
            AddEventTrigger(purpleButton, () => OnClickPuyoColorButton(PuyoMaterial.Purple));
            AddEventTrigger(redButton, () => OnClickPuyoColorButton(PuyoMaterial.Red));
            AddEventTrigger(yellowButton, () => OnClickPuyoColorButton(PuyoMaterial.Yellow));
            AddEventTrigger(randomButton, () => OnClickPuyoColorButton(PuyoMaterial.Random));
            puyoMaterials = new PuyoMaterial[2];
            puyoMaterials[0] = PuyoMaterial.Random;
            puyoMaterials[1] = PuyoMaterial.Random;
        }

        public void AddEventTrigger(GameObject gameObj, Action action)
        {
            EventTrigger currentTrigger = gameObj.AddComponent<EventTrigger>();
            currentTrigger.triggers = new List<EventTrigger.Entry>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((x) => action());
            currentTrigger.triggers.Add(entry);
        }

        public void OnClickPuyoColorButton(PuyoMaterial puyoMaterial)
        {
            puyoMaterials[puyoMaterialCounter] = puyoMaterial;
            var puyo = (puyoMaterialCounter == 0) ? "controller" : "follower";
            Debug.Log($"{puyo} を {puyoMaterial} に変更しました");
            puyoMaterialCounter = (puyoMaterialCounter == 0) ? 1 : 0;
        }

        public void OnClick()
        {
            float y = INITIAL_Y_POSITION;
            if (inputField.text != "")
            {
                float.TryParse(inputField.text, out y);
            }
            Application.PuyopuyoGenerator.Instance.Generate(field.transform, new Vector3(0, y, 0), puyoMaterials[0], puyoMaterials[1]);
        }
    }
}