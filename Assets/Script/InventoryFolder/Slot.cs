using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.InventoryFolder
{
    public class Slot : MonoBehaviour,IDropHandler
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
                InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState = EItemState.Inventory;    //nastavení je v inventáři
                Occupied = true;     //nastavení slotu že je okupován                
                if(InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState != EItemState.Inventory)
                    GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Add(InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>()); // vložení itemu do databáze
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(transform);     //nastavení transformu
            }
            else if (ItemObj.GetComponent<ComponentItem>().ID ==
                     InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().ID && ItemObj != InventoryMouseHandler.itemBeingDragged)    //stackování itemů v invu přetáhnutím
            {
                ItemStacking();
            }
            else if (AreBothInInventory())
            {
                if (InventoryMouseHandler.startParent != null)
                {
                    SlotManagement.SwapItemsInInventory(InventoryMouseHandler.itemBeingDragged.transform, ItemObj.transform, InventoryMouseHandler.startParent, ItemObj.transform.parent, InventoryMouseHandler.startParent.GetComponent<Slot>(),this);
                }
            }
            else if (IsAnyInArmor())
            {
                Debug.Log("this");
                GameObject obj = RetWhichIsInArmor();
                obj.GetComponent<ArmorSlot>().Occupied = true;
                obj.transform.SetParent(InventoryMouseHandler.startParent);
                InventoryMouseHandler.startParent.GetComponent<ArmorSlot>().Occupied = true;

            }
        }

        private bool AreBothInInventory()
        {
            return ItemObj.GetComponent<ComponentItem>().EItemState == EItemState.Inventory &&
                   InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState ==
                   EItemState.Inventory;
        }

        private GameObject RetWhichIsInArmor()
        {
            if (ItemObj.GetComponent<ComponentItem>().EItemState == EItemState.Armor)
                return ItemObj;
            return InventoryMouseHandler.itemBeingDragged;
        }

        private bool IsAnyInArmor()
        {
            return ItemObj.GetComponent<ComponentItem>().EItemState == EItemState.Armor ||
                   InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState ==
                   EItemState.Armor;
        }

        private void ItemStacking()
        {
            if (InventoryMouseHandler.itemBeingDragged == null)
                return;
            ComponentItem itemComponentItem = ItemObj.GetComponent<ComponentItem>();
            if (itemComponentItem.ActualStack == itemComponentItem.MaximumStack)
            {
                SlotManagement.SwapItemsInInventory(InventoryMouseHandler.itemBeingDragged.transform, ItemObj.transform, InventoryMouseHandler.startParent, ItemObj.transform.parent, InventoryMouseHandler.startParent.GetComponent<Slot>(), this);
                return;
            }
            if (!SlotManagement.CanWeStack(ItemObj.GetComponent<ComponentItem>(),
                InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>())) //pokud neni stackovatelný
            {
                int difer = itemComponentItem.MaximumStack - itemComponentItem.ActualStack; //určení diference
                itemComponentItem.ActualStack = itemComponentItem.MaximumStack; //nastavení aktual stacku na max
                SlotManagement.ShowStack(itemComponentItem);    //aktualizace stacků
                InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().ActualStack -= difer;  //odečtení stacků z přetahovaného itemu
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(InventoryMouseHandler.startParent);
                InventoryMouseHandler.startParent.GetComponent<Slot>().Occupied = true;
                SlotManagement.ShowStack(InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>());
                return;
            }
            ItemObj.GetComponent<ComponentItem>().ActualStack += InventoryMouseHandler.itemBeingDragged    //nastavení stacků u objektu
                .GetComponent<ComponentItem>().ActualStack;
            /*  InventorySettings.InventoryList.Find(s => s.ID == ItemObj.GetComponent<ComponentItem>().ID).ActualStack =
                  ItemObj.GetComponent<ComponentItem>().ActualStack;  //nastavení stacků v listu*/
            if (ItemObj.GetComponent<ComponentItem>().ActualStack > 1)  //zobrazení textu s číslem staků
                ItemObj.transform.Find("Stack").GetComponent<Text>().text =
                    ItemObj.GetComponent<ComponentItem>().ActualStack.ToString();
            if (InventoryMouseHandler.itemBeingDragged.transform.parent.GetComponent<Slot>() != null)
                InventoryMouseHandler.itemBeingDragged.transform.parent.GetComponent<Slot>().Occupied = false;
            SlotManagement.RemoveFromInventory(InventoryMouseHandler.itemBeingDragged);
            DestroyObject(InventoryMouseHandler.itemBeingDragged); //zničení objektu který byl přetažen
            
        }
    }
}