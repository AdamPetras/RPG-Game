using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.Menu;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public class QuestMasterObject : MonoBehaviour
    {
        public QuestMasterSettings ThisQuestMaster;
        private PlayerComponent _playerComponent;
        public static bool Visible;
        public static bool CanIDeactive = true;
        public int Id;
        public string Name;
        public int Health;
        public int Mana;
        public int Level;
       // public int[] ListOfQuest;
        //private ObjectGenerate _objectGenerate;
        //private static bool _saveLoadOnceCall;
        // Use this for initialization
        void Start()
        {
            //_objectGenerate = QuestMasterGenerate.QuestMasterList.Find(s => s.Name == name);        
            gameObject.name = Name;
            ThisQuestMaster = new QuestMasterSettings(Id, Name, gameObject, transform.position);
            QuestMasterGenerate.QuestMasterList.Add(this);
            /*for (int i = 0; i < ListOfQuest.Length; i++)
            {
                if (ThisQuestMaster.QuestList.Any(s => s.QuestID != ListOfQuest[i]) || ThisQuestMaster.QuestList.Count == 0)
                    ThisQuestMaster.QuestList.Add(new ModifyQuest(Quest.IdToQuest(ListOfQuest[i])));
            }*/
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                ThisQuestMaster.FindPlayer(_playerComponent);
            }
            StartCoroutine(ThisQuestMaster.CheckMarks());
            StartCoroutine(ThisQuestMaster.MarkRotate());
        }

        // Update is called once per frame
        void Update()
        {
            if (MainMenu.Visible)
                return;
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                ThisQuestMaster.FindPlayer(_playerComponent);
            }
            if (ThisQuestMaster != null && _playerComponent != null)
            {
                ThisQuestMaster.IfNear();
            }
        }

        public void AddQuestToQuestMaster()
        {
            QuestObject obj = null;
            if (GameObject.Find("Quest") != null)
            {
                obj = GameObject.Find("Quest").GetComponent<QuestObject>();
            }
            if (obj != null)
            {               
                foreach (ModifyQuest quest in obj.QuestList.ToList().Where(s => s.EQuestState == EQuestState.None))
                {        
                    if (ThisQuestMaster.ID == quest.QuestMasterAsign)
                    {
                        if (!ThisQuestMaster.QuestList.Contains(quest))
                        {
                            obj.QuestList.Remove(obj.QuestList.Find(s => s.QuestID == quest.QuestID));
                            ThisQuestMaster.QuestList.Add(quest);
                        }
                    }
                }
            }
            foreach (ModifyQuest quest in _playerComponent.QuestList.Where(s => s.EQuestState == EQuestState.Complete))
            {
                if (ThisQuestMaster.ID == quest.QuestMasterSubmit)
                {
                    if (!ThisQuestMaster.QuestList.Contains(quest))
                    {
                        ThisQuestMaster.QuestList.Add(quest);
                    }
                }
            }
            foreach (ModifyQuest quest in _playerComponent.QuestList.Where(s => s.EQuestState == EQuestState.Progress))
            {
                if (quest.EQuest == EQuest.Talk)
                    if (quest.QuestMasterList.Any(a => a == ThisQuestMaster.ID))
                    {
                        if (!ThisQuestMaster.QuestList.Contains(quest))
                            ThisQuestMaster.QuestList.Add(quest);
                    }
                    else { }
                else if (quest.EQuest == EQuest.Delivery || quest.EQuest == EQuest.Kill)
                {
                    if (ThisQuestMaster.ID == quest.QuestMasterSubmit)
                    {
                        if (!ThisQuestMaster.QuestList.Contains(quest))
                        {
                            ThisQuestMaster.QuestList.Add(quest);
                        }
                    }
                }
            }
        }
    }
}
