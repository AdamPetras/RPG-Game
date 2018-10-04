using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Script.InventoryFolder
{
    public class Drop : MonoBehaviour
    {
        [NonSerialized] public List<NewItem> DropItemList;
        [NonSerialized] public BoxCollider DropClickCollider;
        private int CONSTSIZE;
        private Transform _viewPortTransform;
        private Canvas _canvas;
        private GameObject _playerGameObject;
        public static bool Visible;
        private void Awake()
        {
            DropItemList = new List<NewItem>();         
            _canvas = GetComponent<Canvas>();
            CONSTSIZE = 0;
            _viewPortTransform = transform.Find("Background").Find("ScrollView").Find("Viewport");      
            transform.Find("Background").Find("ExitKey").GetComponent<Button>().onClick.AddListener(OnHide);
            gameObject.GetComponent<Canvas>().enabled = false;
            gameObject.GetComponent<GraphicRaycaster>().enabled = false;
            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        }

        private void LateUpdate()
        {
            if (DropClickCollider != null)
                if (!Visible &&Input.GetMouseButtonUp(1)&& Utilities.IsRayCastHit(DropClickCollider.transform) && Utilities.IsDistanceLess(gameObject.transform.parent, _playerGameObject.transform, 2f))
                {
                    Show();
                }
            if (_canvas.enabled)
            {
                DistanceExit();
                if (DropItemList.Count == 0)
                {
                    OnHide();
                    Destroy(gameObject);
                }
            }
        }

        public void Show()
        {
            Visible = true;
            gameObject.GetComponent<Canvas>().enabled = true;
            gameObject.GetComponent<GraphicRaycaster>().enabled = true;
            MainPanel.OpenWindow("DropWindow",gameObject);
        }

        public void DistanceExit()
        {
            
            if (Utilities.IsDistanceBigger(gameObject.transform.parent, _playerGameObject.transform, 2f))
            {
                OnHide();
            }
        }

        public void OnHide()
        {
            Visible = false;
            MainPanel.CloseWindow("DropWindow");
            gameObject.GetComponent<Canvas>().enabled = false;
            gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        }

        public void AddItems(DropItem[] items)
        {
            for (int i = 0; i <= items.Length; i++)
            {
                NewItem item = CONSTSIZE < items.Length ? SimulateDropChance(items[CONSTSIZE]) : null;
                if (item == null)
                {
                    CONSTSIZE++;
                    continue;
                }
                AddItemUsefullFunc(item);
                CONSTSIZE++;
            }
        }

        public void AddItems(DropMoney dropItem)
        {
            NewItem item = SimulateDropChance(dropItem);
            if (item == null)
                return;
            AddItemUsefullFunc(item);
        }

        private void AddItemUsefullFunc(NewItem item)
        {
            Debug.Log(item);
            if (DropItemList == null)
                return;
            
            if (DropItemList.Any(s => s.Name == item.Name && SlotManagement.CanWeStack(item, s.ActualStack)))
            {
                DropItemList.Find(s => s.Name == item.Name && SlotManagement.CanWeStack(item, s.ActualStack)).ActualStack += item.ActualStack;
            }
            else
            {
                GameObject dropSlot = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DropSlot"));
                Text dropslotText = dropSlot.transform.Find("Text").GetComponent<Text>();             
                dropSlot.transform.SetParent(_viewPortTransform);
                GameObject dropItem = dropSlot.transform.Find("DropItem").GetComponent<DropSlot>().ItemObj;
                dropItem = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Item"));
                dropItem.transform.SetParent(dropSlot.transform.Find("DropItem"));
                NewItem.SetStats(dropItem.gameObject, item);
                dropslotText.text = dropItem.GetComponent<ComponentItem>().Name + "\n";
                if (dropItem.GetComponent<ComponentItem>().ActualStack > 1)
                    dropItem.transform.Find("Stack").GetComponent<Text>().text = dropItem.GetComponent<ComponentItem>().ActualStack.ToString();
                dropItem.gameObject.GetComponent<ComponentItem>().EItemState = EItemState.Drop;
                DropItemList.Add(item);
            }
        }

        private NewItem SimulateDropChance(DropItem dropItem)
        {

            if (Random.Range(0, 100) < dropItem.Chance)
            {
                NewItem item = new NewItem(NewItem.IdToItem(dropItem.ItemID));
                if (dropItem.Quantity.QuantityTo > item.MaximumStack)
                {
                    item.ActualStack = Random.Range(dropItem.Quantity.QuantityFrom, item.MaximumStack + 1);
                }
                else
                    item.ActualStack = Random.Range(dropItem.Quantity.QuantityFrom, dropItem.Quantity.QuantityTo + 1);
                return item;
            }
            return null;
        }
        private NewItem SimulateDropChance(DropMoney dropItem)
        {
           
            if (Random.Range(0, 100) < dropItem.Chance)
            {
                NewItem item = new NewItem(NewItem.IdToItem(0));
                if (dropItem.Quantity.QuantityTo > item.MaximumStack)
                {
                    item.ActualStack = Random.Range(dropItem.Quantity.QuantityFrom, item.MaximumStack + 1);
                }
                else
                {
                    item.ActualStack = Random.Range(dropItem.Quantity.QuantityFrom, dropItem.Quantity.QuantityTo + 1);
                    //Debug.Log(item.ActualStack);
                }
                return item;
            }
            return null;
        }

        private void DeleteItem(NewItem item)
        {
            DropItemList.Remove(item);
        }

        public bool DeleteList()
        {
            if (DropItemList != null)
                if (DropItemList.FindAll(s => s == null).Count == 0)
                {
                    DropItemList = null;
                    return true;
                }
            return false;
        }
    }
}
