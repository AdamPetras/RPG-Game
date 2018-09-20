using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.Extension
{
    public class DragAndDropWindow : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _offset;
        private Rect _rect;
        private Rect _screenRect;
        public static bool WindowDrag;
        void Awake()
        {
            _screenRect = new Rect(0, 0, Screen.width, Screen.height);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            WindowDrag = true;
            _rect = new Rect(transform.position.x,
                   transform.position.y,
                   transform.GetComponent<RectTransform>().rect.width,
                   transform.GetComponent<RectTransform>().rect.height);
            _offset = new Vector3(Input.mousePosition.x - _rect.x, Input.mousePosition.y - _rect.y);
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                if (gameObject.transform.parent.GetChild(i) != transform)
                    gameObject.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_screenRect.Contains(Input.mousePosition))
                transform.parent.transform.position = Input.mousePosition - _offset + new Vector3(_rect.width, _rect.height) / 2;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            WindowDrag = false;
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                if (gameObject.transform.parent.GetChild(i) != transform)
                    gameObject.transform.parent.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.parent.parent.transform.SetAsLastSibling();
        }
    }
}