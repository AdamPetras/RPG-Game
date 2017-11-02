using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public class ModifyQuest : Quest
    {
        public ModifyQuest(ModifyQuest quest)
        {
            EQuest = quest.EQuest;
            EQuestState = quest.EQuestState;
            KillsToComplete = quest.KillsToComplete;
            MoneyReward = quest.MoneyReward;
            QuestMasterTalkedList = quest.QuestMasterTalkedList;
            Name = quest.Name;
            Description = quest.Description;
            QuestID = quest.QuestID;
            if (quest.QuestMasterList != null)
                QuestMasterList = quest.QuestMasterList;
            Level = quest.Level;
            Experiences = quest.Experiences;
            QuestMasterAsign = quest.QuestMasterAsign;
            QuestMasterSubmit = quest.QuestMasterSubmit;
            CurrentKills = quest.CurrentKills;
            TotalKills = quest.TotalKills;
            EnemyId = quest.EnemyId;
            ItemReward = quest.ItemReward;
            ItemToDeliver = quest.ItemToDeliver;
            ItemToDeliveryQuantity = quest.ItemToDeliveryQuantity;
        }


        public ModifyQuest(Dictionary<string, string> itemDictionary)
        {
            QuestMasterList = new List<int>();
            QuestMasterTalkedList = new List<int>();
            ItemReward = new List<QuestItem>();
            QuestID = int.Parse(itemDictionary["ID"]);
            Name = itemDictionary["Name"];
            Description = itemDictionary["Description"];
            Level = int.Parse(itemDictionary["Level"]);
            Experiences = int.Parse(itemDictionary["Experiences"]);
            QuestMasterAsign = int.Parse(itemDictionary["QuestMasterAsign"]);
            QuestMasterSubmit = int.Parse(itemDictionary["QuestMasterSubmit"]);
            EQuestState = EQuestState.None;
            KillsToComplete = 0;
            CurrentKills = 0;
            if (itemDictionary["EQuest"] != "")
                EQuest = (EQuest)Enum.Parse(typeof(EQuest), itemDictionary["EQuest"], true);
            else EQuest = EQuest.None;
            MoneyReward = uint.Parse(itemDictionary["Money"]);
            for (int i = 0; i < itemDictionary["ItemReward"].Split(',').Count(); i++)
            {
                if (itemDictionary["ItemReward"].Split(',')[i] != "" && NewItem.DoesItemExist(int.Parse(itemDictionary["ItemReward"].Split(',')[i])) && itemDictionary["ItemRewardQuantity"].Split(',')[i] != "")
                    ItemReward.Add(new QuestItem(new NewItem(NewItem.IdToItem(int.Parse(itemDictionary["ItemReward"].Split(',')[i]))), int.Parse(itemDictionary["ItemRewardQuantity"].Split(',')[i])));
            }
            if (NewItem.DoesItemExist(int.Parse(itemDictionary["ItemToDelivery"])))
                ItemToDeliver = new NewItem(NewItem.IdToItem(int.Parse(itemDictionary["ItemToDelivery"])));
            ItemToDeliveryQuantity = int.Parse(itemDictionary["ItemToDeliveryQuantity"]);
            EnemyId = int.Parse(itemDictionary["EnemyId"]);
            TotalKills = byte.Parse(itemDictionary["TotalKills"]);
            for (int i = 0; i < itemDictionary["QuestMastersID"].Split(',').Count(); i++)
            {
                if (itemDictionary["QuestMastersID"].Split(',')[i] != "")
                    QuestMasterList.Add(int.Parse(itemDictionary["QuestMastersID"].Split(',')[i]));
            }
        }


    }
    public class QuestSettings
    {
        private List<ModifyQuest> _questList;
        public QuestSettings(List<ModifyQuest> questList)
        {
            _questList = questList;
        }

        public void AddQuest(ModifyQuest modifyQuest)
        {
            _questList.Add(modifyQuest);
        }

        public void RemoveQuest(int id)
        {
            _questList.Remove(_questList.Find(s => s.QuestID == id));
        }

        public void RemoveQuest(ModifyQuest modifyQuest)
        {
            _questList.Remove(_questList.Find(s => s.Equals(modifyQuest)));
        }
    }
}
