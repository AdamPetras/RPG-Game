using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.InventoryFolder
{
    public class ArmorSlot : MonoBehaviour, IDropHandler
    {
        public ESubtype ESubtype;
        public bool Occupied;
        private PlayerComponent _playerComponent;
        private void Start()
        {
            _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
        }

        public GameObject ItemObj
        {
            get
            {
                if (transform.childCount > 0)
                    return transform.GetChild(0).gameObject;
                return null;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (InventoryMouseHandler.itemBeingDragged == null)
                return;          
            ComponentItem item = InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>();
            if (item.Subtype == ESubtype && item.EItemState != EItemState.Armor)
            {
                if (Occupied)
                {
                    SlotManagement.AddRemoveStats(ItemObj.GetComponent<ComponentItem>(), false);
                    SlotManagement.AddToInventory(ItemObj.GetComponent<ComponentItem>());
                    _playerComponent.ArmorList.Remove(ItemObj.GetComponent<ComponentItem>());
                    ItemObj.GetComponent<ComponentItem>().EItemState = item.EItemState;
                }
                item.EItemState = EItemState.Armor;
                Occupied = true;
                SlotManagement.AddRemoveStats(item);
                _playerComponent.ArmorList.Add(item);
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(transform);
            }
            else
            {                
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(InventoryMouseHandler.startParent);
                if (InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().EItemState == EItemState.Armor)
                {
                    InventoryMouseHandler.startParent.GetComponent<ArmorSlot>().Occupied = true;
                }
            }
        }
    }
}