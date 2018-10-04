using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Script.InventoryFolder
{
    public class StackEnterComponent : MonoBehaviour
    {
        public static GameObject StackPrefab;
        private static Transform _stackBackground;
        private static Transform _stackExitKey;
        private static InputField _stackInput;
        private static Button _stackButton;
        private static int _buttonListenerCounter;
        private static int _inputFieldListenerCounter;

        private void Awake()
        {
            StackPrefab = gameObject;
            StackPrefab.SetActive(false);
            _stackBackground = StackPrefab.transform.Find("Background");
            _stackExitKey = _stackBackground.Find("ExitKey");
            _stackInput = _stackBackground.Find("InputField").GetComponent<InputField>();
            _stackButton = _stackBackground.Find("Button").GetComponent<Button>();
            _buttonListenerCounter = 0;
            _inputFieldListenerCounter = 0;
            _stackExitKey.GetComponent<Button>().onClick.AddListener(delegate
            {
                StackPrefab.SetActive(false);
            });
        }

        private void Update()
        {
            Debug.Log(_buttonListenerCounter);
        }

        public static void SetPosition(Vector3 position)
        {
            _stackBackground.transform.position = position;
        }

        public static Vector3 GetPosition()
        {
            return _stackBackground.transform.position;
        }

        public static RectTransform GetBackgroundRect()
        {
            return _stackBackground.GetComponent<RectTransform>();
        }

        

        public static void SetText(string text)
        {
            _stackInput.text = text;
        }

        public static string GetText()
        {
            return _stackInput.text;
        }

        public static void SetOnValueChanged(UnityAction<string> action)
        {
            if (_inputFieldListenerCounter == 0)
            {
                _stackInput.onValueChanged.AddListener(action);
                _inputFieldListenerCounter++;
            }
        }

        public static void SetOnButtonClick(UnityAction action)
        {
            if (_buttonListenerCounter == 0)
            {
                _stackButton.onClick.AddListener(action);
                _buttonListenerCounter++;
            }
        }

    }
}