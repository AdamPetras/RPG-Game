using System;
using Assets.Scripts.InventoryFolder;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.InventoryFolder
{
    public class DropSlot : MonoBehaviour
    {
        private GameObject _obj;

        private void Awake()
        {
            _obj = gameObject;
        }

        [NotNull]
        public GameObject ItemObj
        {
            get
            {
                if (_obj.transform.childCount > 0)
                    return _obj.transform.GetChild(0).gameObject;
                return null;
            }
            set { _obj = value; }
        }
    }
}