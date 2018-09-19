using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class PanelPointerEvent:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private Color _defaultColor;
        public Color OnHoverColor;
        private Image _image;
        void Start()
        {
            _image = GetComponent<Image>();
            _defaultColor = _image.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.color = OnHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.color = _defaultColor;
        }
    }
}