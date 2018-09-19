using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.Menu;
using Assets.Scripts;
using Assets.Scripts.InventoryFolder;
using Boo.Lang.Environments;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.InventoryFolder.ShopFolder
{
    [Serializable]
    public struct SaleItem
    {
        public int Id;
        public int Stack;

        public SaleItem(int id)
        {
            Id = id;
            Stack = Int32.MaxValue;
        }
    }

    public class SalesMan //: IBasicAtribute
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public GameObject Prefab;
        public Vector3 Position;
        public int Id;
        private PlayerComponent _playerComponent;
        private GameObject _salesmanHud;
        private Transform _background;
        private Transform _acceptButton;
        private Transform _exitButton;
        private Transform _exitKey;
        private Transform _viewport;
        private List<SaleItem> _itemList;
        public SalesMan(int id, string name, Vector3 position, GameObject prefab, List<SaleItem> itemList)
        {
            Id = id;
            Name = name;
            Position = position;
            Prefab = prefab;
            _itemList = itemList;
            _salesmanHud = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/SalesmanPrefab"), GameObject.Find("Graphics").transform);
            _salesmanHud.name = "SalesmanPrefab";
            InitGUI();
        }

        private void InitGUI()
        {
            _background = _salesmanHud.transform.Find("Background");
            _acceptButton = _background.Find("Apply");
            _exitButton = _background.Find("Exit");
            _exitKey = _background.Find("ExitKey");
            _background.Find("DragPanel").Find("Text").GetComponent<Text>().text = Name;
            _exitButton.GetComponent<Button>().onClick.AddListener(OnExit);
            _exitKey.GetComponent<Button>().onClick.AddListener(OnExit);
            _viewport = _background.Find("ScrollView").Find("Viewport");
            _acceptButton.gameObject.SetActive(false);
            _salesmanHud.SetActive(false);
            Utilities.DisableOrEnableAll(_salesmanHud);
            Init();
        }

        private void OnExit()
        {
            _salesmanHud.SetActive(false);
            Utilities.DisableOrEnableAll(_salesmanHud);
            ComponentSalesMan.Visible = false;
            MainPanel.CloseWindow(_salesmanHud.name);
        }

        public void TalkToSalesMan(Transform transform, Transform playerTransform)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                OnExit();
            if (transform != null && playerTransform != null)
                if (Input.GetMouseButtonUp(1) && Utilities.IsDistanceLess(transform,playerTransform,2f))
                    if (Utilities.IsRayCastHit(transform))
                    {
                        _salesmanHud.SetActive(true);
                        Utilities.DisableOrEnableAll(_salesmanHud, true);
                        ComponentSalesMan.Visible = true;
                        MainPanel.OpenWindow(_salesmanHud.name);
                    }
        }

        public void IfTalkingDistance(Transform transform, Transform playerTransform)
        {
            if (_salesmanHud.activeSelf)
            {
                if (Utilities.IsDistanceBigger(transform, playerTransform, 2f))
                {
                    OnExit();
                }
            }
        }

        private void Init()
        {
            foreach (SaleItem item in _itemList)
            {
                NewItem newItem = new NewItem(NewItem.IdToItem(item.Id));
                newItem.ActualStack = item.Stack;
                GameObject itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/SaleSlot"), _viewport, false);
                Transform saleItemTransform = itemObj.transform.Find("SaleItem");
                saleItemTransform.GetComponent<ComponentItem>().ID = item.Id;
                saleItemTransform.GetComponent<ComponentItem>().ItemStats = newItem.ItemStats;
                saleItemTransform.GetComponent<ComponentItem>().Name = newItem.Name;
                saleItemTransform.GetComponent<ComponentItem>().Icon = newItem.Icon;
                saleItemTransform.GetComponent<ComponentItem>().EItemState = EItemState.Shop;
                saleItemTransform.GetComponent<ComponentItem>().ActualStack = item.Stack;
                saleItemTransform.GetComponent<ComponentItem>().SellPrice = newItem.SellPrice;
                saleItemTransform.GetComponent<InventoryMouseHandler>().CanIMove = false;
                saleItemTransform.GetComponent<Image>().sprite = newItem.Icon;
                itemObj.transform.Find("Name").GetComponent<Text>().text = newItem.Name;
                itemObj.transform.Find("MoneyText").GetComponent<Text>().text = newItem.BuyPrice.ToString();
            }
        }
    }
}