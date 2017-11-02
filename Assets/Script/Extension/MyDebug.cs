using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Extension
{
    public class MyDebug:MonoBehaviour
    {
        private static Text _text;
        void Awake()
        {
            DontDestroyOnLoad(this);
            _text = transform.Find("ScrollView").Find("Viewport").Find("Text").GetComponent<Text>();
           
        }

        public static void LogError(string text)
        {
            _text.color = Color.red;
            _text.text += text+ "\n\n";
        }
        public static void LogWarning(string text)
        {
            _text.color = Color.yellow;
            _text.text += text + "\n\n";
        }
        public static void Log(string text)
        {
            _text.color = Color.white;
            _text.text += text + "\n\n";
        }
    }
}