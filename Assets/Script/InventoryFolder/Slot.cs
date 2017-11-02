using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.InventoryFolder
{
    public class Slot : MonoBehaviour, IDropHandler
    {
        private GameObject _obj;
        public bool Occupied;
        private void Awake()
        {
            _obj = gameObject;
        }

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

        public void OnDrop(PointerEventData eventData)
        {
            if (InventoryMouseHandler.itemBeingDragged == null)
                return;
            if (!ItemObj)   //přídání do inventáře
            {
                if (InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState == EItemState.Drop)
                {
                    Destroy(InventoryMouseHandler.itemBeingDragged.transform.parent.parent.gameObject);
                }
                InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState = EItemState.Inventory;    //nastavení je v inventáři
                Occupied = true;     //nastavení slotu že je okupován
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(transform);     //nastavení transformu
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Add(InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>()); // vložení itemu do databáze
            }
            else if (ItemObj.GetComponent<ComponentItem>().ID ==
                     InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().ID && ItemObj != InventoryMouseHandler.itemBeingDragged)    //stackování itemů v invu přetáhnutím
            {
                ItemStacking();
            }
        }

        private void ItemStacking()
        {
            if (InventoryMouseHandler.itemBeingDragged == null)
                return;
            if (!SlotManagement.CanWeStack(ItemObj.GetComponent<ComponentItem>(), InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>()))    //pokud neni stackovatelný
                return;
            ItemObj.GetComponent<ComponentItem>().ActualStack += InventoryMouseHandler.itemBeingDragged    //nastavení stacků u objektu
                .GetComponent<ComponentItem>().ActualStack;
            /*  InventorySettings.InventoryList.Find(s => s.ID == ItemObj.GetComponent<ComponentItem>().ID).ActualStack =
                  ItemObj.GetComponent<ComponentItem>().ActualStack;  //nastavení stacků v listu*/
            if (ItemObj.GetComponent<ComponentItem>().ActualStack > 1)  //zobrazení textu s číslem staků
                ItemObj.transform.Find("Stack").GetComponent<Text>().text =
                    ItemObj.GetComponent<ComponentItem>().ActualStack.ToString();
            if (InventoryMouseHandler.itemBeingDragged.transform.parent.GetComponent<Slot>() != null)
                InventoryMouseHandler.itemBeingDragged.transform.parent.GetComponent<Slot>().Occupied = false;
            DestroyObject(InventoryMouseHandler.itemBeingDragged); //zničení objektu který byl přetažen
        }
    }
}