using System;
using System.Collections.Generic;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public struct QuestItem
    {
        public NewItem Item;
        public int Quantity;

        public QuestItem(NewItem item, int quantity)
        {
            Item = item;
            if (item.MaximumStack < quantity)
            {
                Debug.LogWarning("#005 Repaired maximum stack of item in quest cause it is exceeded [QuestObject-QuestItem]");
                MyDebug.LogWarning("#005 Repaired maximum stack of item in quest cause it is exceeded [QuestObject-QuestItem]");
            }

            if (SlotManagement.CanWeStack(item, quantity))
            {
                item.ActualStack = quantity;
            }
            Quantity = quantity;
        }
    }

    public class QuestObject : MonoBehaviour
    {
        public QuestSettings QuestSettings;
        public List<ModifyQuest> QuestList;

        // Use this for initialization
        void Awake()
        {          
            QuestList = new List<ModifyQuest>();
            QuestSettings = new QuestSettings(QuestList);
            foreach (ModifyQuest quest in Database.QuestDatabase)
            {
                QuestSettings.AddQuest(new ModifyQuest(quest));
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}