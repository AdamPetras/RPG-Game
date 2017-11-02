using System;
using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.InventoryFolder;
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
        private GameObject _craftItemPrefab;
        private List<GameObject> _objectList;
        private PlayerComponent _playerComponent;
        public static bool ExpChanged = false;
        public CraftDataOperating(GetCraftComponent getCraftComponent)
        {
            _getCraftComponent = getCraftComponent;
            _getCraftComponent.GetCraftButton().onClick.AddListener(AddToInventory);
            _objectList = new List<GameObject>();
            _craftItemPrefab = Resources.Load("Prefab/CraftItem") as GameObject;
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
                InstatiateItem(NewItem.IdToItem(craftItem.ID).Icon, SelectedItem.CraftItems[index].NumberOfItems.ToString(), _getCraftComponent.GetCraftNeedPanel());
                index++;
            }
            InstatiateItem(NewItem.IdToItem(SelectedItem.ID).Icon, SelectedItem.Quantity.ToString(), _getCraftComponent.GetProductPanel());
            _getCraftComponent.GetTextNeedLevel().GetComponent<Text>().text =SelectedItem.EProfession+" level need : "+
                SelectedItem.ProfesionLevelNeed;
        }

        private void InstatiateItem(Sprite icon, string text, Transform parent)
        {
            GameObject itemObject = Object.Instantiate(_craftItemPrefab);
            _objectList.Add(itemObject);
            if (icon != null)
                itemObject.GetComponent<Image>().sprite = icon;
            itemObject.transform.Find("Text").GetComponent<Text>().text = text;
            itemObject.transform.SetParent(parent, true);
            itemObject.transform.localScale = Vector3.one;
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

        public void AddToInventory()
        {
            if (SelectedItem != null)
                if (IsInventoryContains()&& IsLevelAvailable(SelectedItem))
                {
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
