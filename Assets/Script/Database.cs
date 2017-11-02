using System;
using System.Collections.Generic;
using Assets.Script.QuestFolder;
using Assets.Script.SpellFolder;
using Assets.Scripts.Extension;
using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Script
{
    public class Database : MonoBehaviour
    {
        public TextAsset ItemInventory;
        public TextAsset QuestList;
        public TextAsset SpellList;
        public static List<NewItem> ItemDatabase = new List<NewItem>();
        public static List<ModifyQuest> QuestDatabase = new List<ModifyQuest>();
        public static List<Spell> SpellDatabase = new List<Spell>();
        void Awake()
        {
            DontDestroyOnLoad(this);
            Init(ItemInventory,"Item",ItemDatabase,l=> new NewItem(l), "ID", "ItemName", "Type", "Subtype", "Stats", "StatValues", "Chance",
                "CraftItem", "NumberOfItems", "Quantity", "Profession", "ProfessionLevelNeed", "ProfessionExperiences", "BuyPrice", "SellPrice", "Loot", "MaximumStack");
            Init(QuestList,"Quest", QuestDatabase, l => new ModifyQuest(l),"ID", "Name", "Description", "Level", "Experiences", "QuestMasterAsign", "QuestMasterSubmit",
                "EQuest", "Money", "ItemReward", "ItemRewardQuantity", "ItemToDelivery", "ItemToDeliveryQuantity", "EnemyId", "TotalKills", "QuestMastersID");
            Init(SpellList,"Spell",SpellDatabase,l=>new Spell(l),"ID","Name","Description","LevelToAccess","GoldToAccess","ManaCost","Range","Cooldown","SpellType","Duration","Skill", "PercentageIncrease", "PercentageDamage");
        }
        //ultra hnusná metoda
        private void  Init<T>(TextAsset asset,string firstElem,List<T> staticList, Func<Dictionary<string, string>,T> objNew,params string [] tabs)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            XmlDatabaseReader reader = new XmlDatabaseReader(asset);
            reader.ReadItemsFromDatabase(firstElem, list, tabs);
            if (staticList.Count == 0)    //po načtení scény se pořád vkládali do databáze jedny a ty samé položky
                foreach (Dictionary<string, string> db in list)
                {
                    staticList.Add(objNew(db));
                }
        }
    }
}
