using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.SpellFolder
{
    public class SpellSlot : MonoBehaviour, IDropHandler
    {
        public bool Occupied;
        private GameObject _obj;
        public bool IsBook;
        private KeyCode keyCode;
        private void Awake()
        {
            _obj = gameObject;
            Occupied = false;
        }

        private void Start()
        {
            if (!IsBook)
                SetEnum();
        }

        private void Update()
        {
            if (Input.GetKeyUp(keyCode))
            {
                if (_obj.transform.Find("Spell") != null)
                {
                    _obj.transform.Find("Spell").GetComponent<Button>().onClick.Invoke();
                }
            }
        }

        private void SetEnum()
        {
            if (gameObject.transform.Find("Text") == null)
                return;
            string text = gameObject.transform.Find("Text").GetComponent<Text>().text;
            if (text.All(char.IsDigit))
            {
                string s = "Alpha" + text;
                text = s;
            }
            keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, true);
        }

        [NotNull]
        public GameObject SpellObj
        {
            get
            {
                if (_obj.transform.childCount > 0)
                {
                    return _obj.transform.GetChild(0).gameObject;
                }
                return null;
            }
            set
            {
                _obj = value;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_obj != null && SpellDragAndDrop.SpellDraged != null && !IsBook)
            {
                //Debug.Log("drop");
                if (Occupied)
                {
                    if (_obj.transform.GetChild(0).GetComponent<ComponentSpell>().Spell.CooldownEnable)
                    {
                        Destroy(_obj.transform.GetChild(0).gameObject);
                        ComponentSpell.SpellList.Remove(_obj.transform.GetChild(0).GetComponent<ComponentSpell>());
                    }
                    else
                    {
                        _obj.transform.GetChild(0).GetComponent<ComponentSpell>().Spell.IWantToDestroy = true;
                        _obj.transform.GetChild(0).SetParent(GameObject.Find("DragPanel").transform.Find("Background"));
                    }
                }
                Occupied = true;
                SpellDragAndDrop.SpellDraged.transform.SetParent(transform);
                SpellDragAndDrop.SpellDraged.transform.position = transform.position;
                gameObject.transform.Find("Text").SetAsLastSibling();
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}