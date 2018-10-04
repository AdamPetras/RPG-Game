using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.Menu;
using UnityEngine;

namespace Assets.Script.InventoryFolder.ShopFolder
{
    public class ComponentSalesMan : MonoBehaviour
    {
        public SalesMan SalesMan;
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
            SalesMan = new SalesMan(Id, Name, transform.position, gameObject, ItemList);
            SalesManGenerate.SalesManList.Add(this);
        }

        void Update()
        {
            if (MainMenu.Visible)
                return;
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            }
            if (SalesMan != null)
            {
                SalesMan.TalkToSalesMan(transform, _playerComponent.transform);
                SalesMan.IfTalkingDistance(transform, _playerComponent.transform);
            }
        }
    }
}