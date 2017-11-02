using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.StatisticsFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.InventoryFolder
{
    public class SlotManagement
    {
        private static Character _character;
        public static bool StatsChanged;

        public SlotManagement(Character character)
        {
            _character = character;
            StatsChanged = false;
        }
        public static bool CanWeStack(NewItem firstItem, NewItem secondItem)
        {
            return (firstItem.Name == secondItem.Name && firstItem.MaximumStack > 1 &&
                    firstItem.ActualStack + secondItem.ActualStack <= firstItem.MaximumStack &&
                    firstItem.ActualStack < firstItem.MaximumStack);

        }

        public static bool CanWeStack(NewItem firstItem, int quantity)
        {
            return (firstItem.MaximumStack > 1 && quantity + firstItem.ActualStack <= firstItem.MaximumStack);
        }

        public static bool CanWeStack(ComponentItem firstItem, ComponentItem secondItem)
        {
            return (firstItem.Name == secondItem.Name && firstItem.MaximumStack > 1 &&
                    firstItem.ActualStack + secondItem.ActualStack <= firstItem.MaximumStack &&
                    firstItem.ActualStack < firstItem.MaximumStack);

        }

        public static bool CanWeStack(ComponentItem firstItem, int quantity)
        {
            return (firstItem.MaximumStack > 1 && quantity + firstItem.ActualStack <= firstItem.MaximumStack);
        }

        public static bool IfAnySlotListContains(List<ComponentItem> slotList, int itemId, int numberOfStack)
        {
            int allStacks = slotList.Where(s => s != null).Where(s => s.ID == itemId)   //zápis nahoře akorát použití linq
                    .Sum(slotItem => slotItem.ActualStack);
            return numberOfStack <= allStacks;
        }

        public static Transform FindEmptySlot(EItemState eItemState, ESubtype typeOfSlot = ESubtype.None)
        {
            Transform transform = null;
            try
            {
                Debug.Log(GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventorySlotList.Count);
                if (eItemState == EItemState.Inventory)
                {
                    transform = GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventorySlotList.Find(s => !s.GetComponent<Slot>().Occupied).transform;
                }
                else if (eItemState == EItemState.Armor)
                {
                    if (typeOfSlot == ESubtype.Axe || typeOfSlot == ESubtype.Sword || typeOfSlot == ESubtype.Knife)
                        typeOfSlot = ESubtype.Weapon;
                    transform = GameObject.Find("StatsView").transform.Find("Background").Find(typeOfSlot + "Slot");
                    Debug.Log(typeOfSlot);
                    if (transform.GetComponent<ArmorSlot>().Occupied)
                        return null;
                }
            }
            catch (NullReferenceException e)
            {
                if (typeOfSlot == ESubtype.None)
                {
                    Debug.LogWarning("#002 Inventory is full or inventory isnt created [SlotManagement]");
                    MyDebug.LogWarning("#002 Inventory is full or inventory isnt created [SlotManagement]");
                }
                else
                {
                    Debug.LogWarning("#003 Armor gear is full or armor slots isnt created [SlotManagement]");
                    MyDebug.LogWarning("#003 Armor gear is full or armor slots isnt created [SlotManagement]");
                }
                return null;
            }
            return transform;
        }



        private static void AddItemByStack(ComponentItem item)
        {
            int differ = item.ActualStack;
            List<ComponentItem> foundItems;
            if ((foundItems = FindAllItemInInventory(item.ID)).Count > 0)
            {
                foreach (ComponentItem foundItem in foundItems)
                {

                    int difference = foundItem.MaximumStack - foundItem.ActualStack;
                    if (differ > difference)
                    {
                        foundItem.ActualStack = foundItem.MaximumStack;
                        differ -= difference;
                    }
                    else
                    {
                        foundItem.ActualStack += differ;
                        ShowStack(foundItem);
                        GameObject.Destroy(item.gameObject);
                        return;
                    }

                    ShowStack(foundItem);
                }
            }
            do
            {
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Item"));
                ComponentItem mainItem = obj.GetComponent<ComponentItem>();
                Transform transform = FindEmptySlot(EItemState.Inventory);
                NewItem.SetStats(obj, new NewItem(NewItem.IdToItem(item.ID)));
                if (transform == null)
                    return;
                transform.GetComponent<Slot>().Occupied = true;
                mainItem.EItemState = EItemState.Inventory;
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Add(mainItem);
                if (differ > mainItem.MaximumStack)
                {
                    mainItem.ActualStack = mainItem.MaximumStack;
                    differ -= mainItem.MaximumStack;
                }
                else
                {
                    mainItem.ActualStack = differ;
                    differ = 0;
                }
                mainItem.transform.SetParent(transform);
                ShowStack(mainItem);
            } while (differ > 0);
            GameObject.Destroy(item.gameObject);
        }

        public static bool IsInventoryFull()
        {
            return GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventorySlotList.All(s => s.GetComponent<Slot>().Occupied);
        }

        public static bool AddToInventory(GameObject itemObject)
        {
            AddItemByStack(itemObject.GetComponent<ComponentItem>());
            return true;
        }

        public static bool AddToInventory(ComponentItem componentItem)
        {
            AddItemByStack(componentItem);
            return true;
        }

        public static bool AddToArmor(ComponentItem componentItem)
        {
            if (componentItem.Type != EType.Armour &&
                componentItem.Type != EType.Weapon)
            {
                return false;
            }
            //nalezení prázdného slotu v gearu
            Transform obj =
                SlotManagement.FindEmptySlot(EItemState.Armor,
                    componentItem.Subtype);
            if (obj == null)
                return false;
            //nastavení všech potřebných věcí například okupace u slotu  následně rodiče a item state a okupaci u gearu
            componentItem.transform.parent.GetComponent<Slot>().Occupied = false;
            componentItem.transform.SetParent(obj);
            componentItem.EItemState = EItemState.Armor;
            obj.GetComponent<ArmorSlot>().Occupied = true;
            //přidání statů
            SlotManagement.AddRemoveStats(componentItem);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList.Add(componentItem);
            //skrytí info (nemusí být ale pak je prodleva než se to skryje)
            return true;
        }

        public static bool AddToArmor(NewItem item)
        {

            GameObject itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Item"));
            NewItem.SetStats(itemObj, item);
            ComponentItem componentItem = itemObj.GetComponent<ComponentItem>();
            if (componentItem.Type != EType.Armour &&
                componentItem.Type != EType.Weapon)
            {
                return false;
            }
            //nalezení prázdného slotu v gearu
            Transform obj =
                SlotManagement.FindEmptySlot(EItemState.Armor,
                    componentItem.Subtype);
            if (obj == null)
                return false;
            //nastavení všech potřebných věcí například rodiče a item state a okupaci u gearu
            componentItem.transform.SetParent(obj);
            componentItem.EItemState = EItemState.Armor;
            obj.GetComponent<ArmorSlot>().Occupied = true;
            //přidání statů
            SlotManagement.AddRemoveStats(componentItem);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList.Add(componentItem);
            //skrytí info (nemusí být ale pak je prodleva než se to skryje)
            return true;
        }

        public static bool ClearList(List<ComponentItem> list)
        {
            if (list != null)
            {
                foreach (ComponentItem obj in list)
                {
                    if (obj != null)
                    {
                        if (obj.transform.parent.GetComponent<Slot>() != null)
                            obj.transform.parent.GetComponent<Slot>().Occupied = false;
                        else if (obj.transform.parent.GetComponent<ArmorSlot>() != null)
                            obj.transform.parent.GetComponent<ArmorSlot>().Occupied = false;
                        GameObject.Destroy(obj.gameObject);

                    }
                }
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Clear();
                return true;
            }
            return false;
        }

        public static bool ClearList(List<GameObject> list)
        {
            if (list != null)
            {
                foreach (GameObject obj in list)
                {
                    if (obj != null)
                    {
                        GameObject.Destroy(obj.gameObject);
                    }
                }
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Clear();
                return true;
            }
            return false;
        }

        public static bool AddToInventory(NewItem componentItem)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Item"));
            NewItem.SetStats(obj, componentItem);
            AddItemByStack(obj.GetComponent<ComponentItem>());
            return true;
        }

        public static void ShowStack(ComponentItem itemObj)
        {
            if (itemObj != null)
                if (itemObj.ActualStack > 1)  //zobrazení textu s číslem staků
                    itemObj.transform.Find("Stack").GetComponent<Text>().text =
                        itemObj.ActualStack.ToString();
                else
                {
                    itemObj.transform.Find("Stack").GetComponent<Text>().text = "";
                }
        }


        public static void DeleteItemByStacks(ComponentItem item, int numberOfStack)
        {
            if (!StackUnderflow(item, numberOfStack))
            {
                item.ActualStack -= numberOfStack;
                FindItemInInventory(item.ID).ActualStack = item.ActualStack;
                ShowStack(item);
            }
            else
            {
                item.transform.parent.GetComponent<Slot>().Occupied = false;
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Remove(item);
                GameObject.Destroy(item.gameObject);
            }
        }

        public static ComponentItem FindItemInInventory(int id)
        {
            return GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Find(s => s.ID == id);
        }

        public static List<ComponentItem> FindAllItemInInventory(int id)
        {
            return GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.FindAll(s => s.ID == id);
        }

        public static bool StackOverflow(ComponentItem firstItem, ComponentItem secondItem)
        {
            return firstItem.ActualStack + secondItem.ActualStack > firstItem.MaximumStack;
        }

        public static bool StackUnderflow(ComponentItem firstItem, ComponentItem secondItem)
        {
            return firstItem.ActualStack - secondItem.ActualStack < 1;
        }

        public static bool StackUnderflow(ComponentItem firstItem, int secondItemStack)
        {
            return firstItem.ActualStack - secondItemStack < 1;
        }

        public static bool StackOverflow(ComponentItem firstItem, int secondItemStack)
        {
            return firstItem.ActualStack + secondItemStack > firstItem.MaximumStack;
        }

        public static void AddRemoveStats(ComponentItem item, bool add = true)
        {
            if (item == null)
                return;
            if (item.Type == EType.Armour)
            {
                StatsChanged = true;
                foreach (ItemStats itemStats in item.ItemStats)
                {
                    switch (itemStats.ESkill)
                    {
                        case ESkill.Constitution:
                            AddOrRemove(ESkill.Constitution, itemStats.Value, add);
                            break;
                        case ESkill.Agility:
                            AddOrRemove(ESkill.Agility, itemStats.Value, add);
                            break;
                        case ESkill.Attack:
                            AddOrRemove(ESkill.Attack, itemStats.Value, add);
                            break;
                        case ESkill.Defence:
                            AddOrRemove(ESkill.Defence, itemStats.Value, add);
                            break;
                        case ESkill.Dexterity:
                            AddOrRemove(ESkill.Dexterity, itemStats.Value, add);
                            break;
                        case ESkill.Intelect:
                            AddOrRemove(ESkill.Intelect, itemStats.Value, add);
                            break;
                        case ESkill.Magic:
                            AddOrRemove(ESkill.Magic, itemStats.Value, add);
                            break;
                        case ESkill.Ranged:
                            AddOrRemove(ESkill.Ranged, itemStats.Value, add);
                            break;
                        case ESkill.Regenerate:
                            AddOrRemove(ESkill.Regenerate, itemStats.Value, add);
                            break;
                        case ESkill.Strenght:
                            AddOrRemove(ESkill.Strenght, itemStats.Value, add);
                            break;
                    }
                }
            }
        }
        public static void AddRemoveStats(NewItem item, bool add = true)
        {
            if (item == null)
            {
                Debug.LogWarning("#006 Armor item is null i cant add stats [SlotManagement]");
                MyDebug.LogWarning("#006 Armor item is null i cant add stats [SlotManagement]");
                return;
            }
            if (item.Type != EType.Armour && item.Type != EType.Weapon)
            {
                Debug.LogWarning("#007 Item is not type of armor or weapon [SlotManagement]");
                MyDebug.LogWarning("#007 Item is not type of armor or weapon [SlotManagement]");
            }
            StatsChanged = true;
            foreach (ItemStats itemStats in item.ItemStats)
            {
                switch (itemStats.ESkill)
                {
                    case ESkill.Constitution:
                        AddOrRemove(ESkill.Constitution, itemStats.Value, add);
                        break;
                    case ESkill.Agility:
                        AddOrRemove(ESkill.Agility, itemStats.Value, add);
                        break;
                    case ESkill.Attack:
                        AddOrRemove(ESkill.Attack, itemStats.Value, add);
                        break;
                    case ESkill.Defence:
                        AddOrRemove(ESkill.Defence, itemStats.Value, add);
                        break;
                    case ESkill.Dexterity:
                        AddOrRemove(ESkill.Dexterity, itemStats.Value, add);
                        break;
                    case ESkill.Intelect:
                        AddOrRemove(ESkill.Intelect, itemStats.Value, add);
                        break;
                    case ESkill.Magic:
                        AddOrRemove(ESkill.Magic, itemStats.Value, add);
                        break;
                    case ESkill.Ranged:
                        AddOrRemove(ESkill.Ranged, itemStats.Value, add);
                        break;
                    case ESkill.Regenerate:
                        AddOrRemove(ESkill.Regenerate, itemStats.Value, add);
                        break;
                    case ESkill.Strenght:
                        AddOrRemove(ESkill.Strenght, itemStats.Value, add);
                        break;
                }
            }
        }

        private static void AddOrRemove(ESkill eSkill, int bonus, bool add)
        {
            if (add)
                _character.AddStats(new CharacterSkill(_character.GetSkill((int)eSkill), bonus));
            else
                _character.AddStats(new CharacterSkill(_character.GetSkill((int)eSkill), -bonus));
        }
    }
}
