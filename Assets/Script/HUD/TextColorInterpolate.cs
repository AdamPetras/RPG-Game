using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Script.HUD
{
    public class TextColorInterpolate : MonoBehaviour
    {
        private readonly Vector3 _scale = new Vector3(1.5f, 1.5f, 1.5f);
        private readonly Vector3 _defaultScale = new Vector3(1, 1, 1);
        private Vector3 _position;
        private RectTransform _rectTransform;
        private Color _defaultColor;
        private bool _stopInterpolate;
        private void Start()
        {
            _rectTransform = transform.GetComponent<RectTransform>();
            _defaultColor = transform.GetComponent<Text>().color;
            Init();
            InvokeRepeating("Interpolate", 2f, 0.05f);
        }

        private void Init()
        {
            transform.localScale = _defaultScale;
            _rectTransform.anchoredPosition = new Vector3(Random.Range(-Screen.width / 8, Screen.width / 8), Random.Range(-Screen.height / 8, Screen.height / 8));
            _position = new Vector3(_rectTransform.anchoredPosition.x, Math.Abs(_rectTransform.anchoredPosition.y) + Screen.height / 8, 0);
            _rectTransform.GetComponent<Text>().color = _defaultColor;
        }

        private void Interpolate()
        {
            if (!_stopInterpolate && Vector2.Distance(_rectTransform.anchoredPosition, _position) < 0.5f)
            {
                Color color = _rectTransform.GetComponent<Text>().color;
                color.a = Mathf.Lerp(color.a, 0, 0.1f);
                _rectTransform.GetComponent<Text>().color = color;
                if (_rectTransform.GetComponent<Text>().color.a <= 0.01f)
                {
                    //Init();
                    CancelInvoke("Interpolate");
                    Destroy(gameObject);
                }
            }
            else if (!_stopInterpolate)
            {
                _rectTransform.anchoredPosition = Vector3.Lerp(_rectTransform.anchoredPosition, _position, 0.1f);
                transform.localScale = Vector3.Lerp(transform.localScale, _scale, Time.deltaTime);
            }
        }
    }
}