using System;
using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using Assets.Script.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.InventoryFolder.CraftFolder
{
    public struct CraftItem
    {
        public int ID;
        public int NumberOfItems;

        public CraftItem(int id, int numberOfItems)
        {
            ID = id;
            NumberOfItems = numberOfItems;
        }
    }

    public class CraftDataOperating
    {
        public NewItem SelectedItem { get; set; }
        private GetCraftComponent _getCraftComponent;
        private List<GameObject> _objectList;
        private PlayerComponent _playerComponent;
        public static bool ExpChanged = false;
        public CraftDataOperating(GetCraftComponent getCraftComponent)
        {
            _getCraftComponent = getCraftComponent;
            _getCraftComponent.GetCraftButton().onClick.AddListener(AddToInventory);
            _objectList = new List<GameObject>();

        }

        public void ItemClicked(PlayerComponent playerComponent)
        {           
            int index = 0;
            _playerComponent = playerComponent;
            foreach (GameObject obj in _objectList.ToArray())
            {
                RemoveFromList(_objectList, obj);
            }
            foreach (CraftItem craftItem in SelectedItem.CraftItems)
            {
                _objectList.Add(Utilities.InstatiateItem(NewItem.IdToItem(craftItem.ID), SelectedItem.CraftItems[index].NumberOfItems.ToString(), _getCraftComponent.GetCraftNeedPanel()));
                index++;
            }
            _objectList.Add(Utilities.InstatiateItem(NewItem.IdToItem(SelectedItem.ID), SelectedItem.Quantity.ToString(), _getCraftComponent.GetProductPanel()));
            _getCraftComponent.GetTextNeedLevel().GetComponent<Text>().text =SelectedItem.EProfession+" level need : "+
                SelectedItem.ProfesionLevelNeed;
            _getCraftComponent.GetCraftItemName().GetComponent<Text>().color = Utilities.ColorByItemRank(SelectedItem.ERank);
            _getCraftComponent.GetCraftItemName().GetComponent<Text>().text = SelectedItem.Name;
            string s = "";
            foreach (ItemStats stats in SelectedItem.ItemStats)
            {
                s += stats.ESkill + " " + stats.Value + "\n";
            }
            _getCraftComponent.GetCraftItemInfo().GetComponent<Text>().text =
                s + "Sell price: " + SelectedItem.SellPrice;
        }



        private void RemoveFromList(List<GameObject> viewCardList, GameObject gameObject)
        {
            if (viewCardList.Contains(gameObject))
            {
                viewCardList.Remove(gameObject);  //odstranění vybraného objektu z listu
                Object.Destroy(gameObject);
            }
        }
        private bool IsLevelAvailable(NewItem item)
        {
            return _playerComponent.SelectProfession(item.EProfession).Level >= item.ProfesionLevelNeed;
        }
        private bool IsInventoryContains()
        {
            foreach (CraftItem craftItem in SelectedItem.CraftItems)
            {
                if (!SlotManagement.IfAnySlotListContains(GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList, craftItem.ID, craftItem.NumberOfItems))
                {
                    return false;
                }
            }
            return true;
        }

        private bool YouNeedToBeOnPlace(EProfession eProfession)
        {
            if (eProfession == EProfession.Smithing && ComponentCraftMenu.IsNearAnvil)
                return true;
            if (eProfession == EProfession.Crafting && ComponentCraftMenu.IsNearCraftTable)
                return true;
            if (eProfession == EProfession.Cooking && ComponentCraftMenu.IsNearRange)
                return true;
            if (eProfession == EProfession.Tailoring && ComponentCraftMenu.IsNearTailorKit)
                return true;
            return false;
        }

        public void AddToInventory()
        {
            if (SelectedItem != null)
                if (IsInventoryContains()&& IsLevelAvailable(SelectedItem))
                {
                    if (!YouNeedToBeOnPlace(SelectedItem.EProfession))
                    {
                        if(SelectedItem.EProfession == EProfession.Tailoring)
                            MyDebug.LogWarning("You have to be near tailoring kit");
                        if (SelectedItem.EProfession == EProfession.Smithing)
                            MyDebug.LogWarning("You have to be near anvil and furnace");
                        if (SelectedItem.EProfession == EProfession.Crafting)
                            MyDebug.LogWarning("You have to be near crafting table");
                        if (SelectedItem.EProfession == EProfession.Cooking)
                            MyDebug.LogWarning("You have to be near range");
                        return;
                    }
                    SelectedItem.ActualStack = SelectedItem.Quantity;
                    if (SlotManagement.AddToInventory(SelectedItem))
                    {
                        if (_playerComponent.SelectProfession(SelectedItem.EProfession).Level -
                            SelectedItem.ProfesionLevelNeed < 5)
                        {
                            _playerComponent.SelectProfession(SelectedItem.EProfession)
                                .AddExperiences(Convert.ToUInt32(SelectedItem.ProfesionExperiences));
                            ExpChanged = true;
                        }
                        RemoveItemsFromInv();
                    }
                }
        }

        private void RemoveItemsFromInv()
        {
            foreach (CraftItem craftItem in SelectedItem.CraftItems)
            {
                SlotManagement.DeleteItemByStacks(SlotManagement.FindItemInInventory(craftItem.ID), craftItem.NumberOfItems);
            }
        }
    }
}
