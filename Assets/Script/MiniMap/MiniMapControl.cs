using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.MiniMap
{
    public class MiniMapControl:MonoBehaviour
    {
        private static Button _plus;
        private static Button _minus;
        public int MaximumScope;
        public int OriginalScope;
        public int MinimumScope;
        private static UnityEngine.Camera _miniMapCamera;
        private void Start()
        {
            _minus = gameObject.transform.Find("Minus").GetComponent<Button>();
            _plus = gameObject.transform.Find("Plus").GetComponent<Button>();
            _minus.onClick.AddListener(OnMinus);
            _plus.onClick.AddListener(OnPlus);
            _miniMapCamera = GameObject.FindGameObjectWithTag("Player").transform.Find("MiniMapCamera")
                .GetComponent<UnityEngine.Camera>();
            _miniMapCamera.orthographicSize = OriginalScope;
            
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                {
                    OnPlus();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
                {
                    OnMinus();
                }
            }
        }

        public void OnPlus()
        {
            if (_miniMapCamera.orthographicSize > MinimumScope)
                _miniMapCamera.orthographicSize--;           
        }
        public void OnMinus()
        {
            if (_miniMapCamera.orthographicSize < MaximumScope)
                _miniMapCamera.orthographicSize++;
        }
    }
}