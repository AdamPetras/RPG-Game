using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using UnityEngine;

namespace Assets.Script.InventoryFolder.ShopFolder
{
    public class ComponentSalesMan : MonoBehaviour
    {
        private SalesMan _salesMan;
        private PlayerComponent _playerComponent;
        private List<ObjectGenerate> _salesManList;
        public int Id;
        public string Name;
        public int Health;
        public int Mana;
        public int Level;
        public static bool Visible;
        public static bool CanIDeactive = true;
        public List<SaleItem> ItemList;
        void Start()
        {
            name = Name;
            _salesMan = new SalesMan(Id, Name, transform.position, gameObject, ItemList);
            SalesManGenerate.SalesManList.Add(this);
        }

        void Update()
        {
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            }
            if (_salesMan != null)
            {
                _salesMan.TalkToSalesMan(transform, _playerComponent.transform);
                _salesMan.IfTalkingDistance(transform, _playerComponent.transform);
            }
        }
    }
}