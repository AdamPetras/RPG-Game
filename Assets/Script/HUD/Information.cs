using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class Information : MonoBehaviour
    {
        private static Text _infoText;
        private static Image _background;
        private static Information _instance;
        void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _infoText = GetComponent<Text>();
            _background = GetComponentInParent<Image>();
        }

        private static void SetTextComponent(string txt)
        {
            if (txt == "")
                _background.enabled = false;
            else
                _background.enabled = true;
            _infoText.text = txt;
            
        }

        public static void SetText(string txt)
        {
            SetTextComponent(txt);
            _infoText.color = Color.green;
        }
        public static void SetText(string txt,Color color)
        {
            SetTextComponent(txt);
            _infoText.color = color;
        }

        public static void SetText(string txt, Color color,float timetolive)
        {
            SetTextComponent(txt);
            _infoText.color = color;
            _instance.StartCoroutine(TimeToLive(timetolive));
        }

        private static IEnumerator TimeToLive(float time)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                SetText("");
                yield break;
            }
        }

    }
}