using System;
using System.Collections.Generic;
using Assets.Script.Enemy;
using Assets.Scripts.InventoryFolder;

namespace Assets.Script.QuestFolder
{
    public enum EQuest
    {
        Delivery,
        Kill,
        Talk,
        None
    }

    public enum EQuestState
    {
        Complete,
        Progress,
        None
    }

    public abstract class Quest : IQuestKill, IQuestDelivery, IQuestTalk
    {
        public int QuestID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experiences { get; set; }
        public int QuestMasterAsign { get; set; }
        public uint MoneyReward { get; set; }
        public int QuestMasterSubmit { get; set; }
        public EQuest EQuest { get; set; }
        public EQuestState EQuestState { get; set; }
        public int EnemyId { get; set; }
        public byte CurrentKills { get; set; }
        public byte KillsToComplete { get; set; }
        public byte TotalKills { get; set; }
        public List<int> QuestMasterList { get; set; }
        public List<int> QuestMasterTalkedList { get; set; }
        public List<QuestItem> ItemReward { get; set; }
        public NewItem ItemToDeliver { get; set; }
        public int ItemToDeliveryQuantity { get; set; }

        public virtual void AddQuest(ModifyQuest modifyQuest)
        {

        }

        public virtual void RemoveQuest(int id)
        {

        }

        public virtual void RemoveQuest(ModifyQuest modifyQuest)
        {

        }
        public static ModifyQuest IdToQuest(int id)
        {
            return Database.QuestDatabase.Find(s => s.QuestID == id);
        }
    }
}
