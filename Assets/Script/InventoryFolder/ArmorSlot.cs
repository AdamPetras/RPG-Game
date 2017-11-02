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
            if (!ItemObj && InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>().Subtype == ESubtype && !Occupied)
            {
                Occupied = true;
                ComponentItem armor = InventoryMouseHandler.itemBeingDragged.GetComponent<ComponentItem>();
                armor.EItemState = EItemState.Armor;
                SlotManagement.AddRemoveStats(armor);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList.Add(armor);
                InventoryMouseHandler.itemBeingDragged.transform.SetParent(transform);
            }
        }


    }
}