using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class Information : MonoBehaviour
    {
        private static Text _infoText;

        private void Start()
        {
            _infoText = GetComponent<Text>();
        }

        public static void SetText(string txt)
        {
            _infoText.text = txt;
        }

    }
}