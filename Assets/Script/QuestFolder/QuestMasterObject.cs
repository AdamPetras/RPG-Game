using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public class QuestMasterObject : MonoBehaviour
    {
        public QuestMasterSettings QuestMaster;
        private PlayerComponent _playerComponent;
        public EGenerateState EGenerateState;
        public static bool Visible;
        public static bool CanIDeactive = true;
        public int Health;
        public int Mana;
        public int Level;
        private ObjectGenerate _objectGenerate;

        private static bool _saveLoadOnceCall;
        // Use this for initialization
        void Start()
        {
            _objectGenerate = QuestMasterGenerate.QuestMasterList.Find(s => s.Name == name);
            QuestMaster = new QuestMasterSettings(_objectGenerate.Id, _objectGenerate.Name, _objectGenerate.Prefab, _objectGenerate.Position);
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                QuestMaster.FindPlayer(_playerComponent);
            }
            AddQuestToQuestMaster();
            StartCoroutine(QuestMaster.CheckMarks());
            StartCoroutine(QuestMaster.MarkRotate());
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                QuestMaster.FindPlayer(_playerComponent);
            }
            if (QuestMaster != null && _playerComponent != null)
            {                
                QuestMaster.IfNear();
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
                    if (QuestMaster.ID == quest.QuestMasterAsign)
                    {
                        if (!QuestMaster.QuestList.Contains(quest))
                        {
                            obj.QuestList.Remove(obj.QuestList.Find(s => s.QuestID == quest.QuestID));
                            QuestMaster.QuestList.Add(quest);
                        }
                    }
                }
            }
            foreach (ModifyQuest quest in _playerComponent.QuestList.Where(s => s.EQuestState == EQuestState.Complete))
            {
                if (QuestMaster.ID == quest.QuestMasterSubmit)
                {
                    if (!QuestMaster.QuestList.Contains(quest))
                    {
                        QuestMaster.QuestList.Add(quest);
                    }
                }
            }
            foreach (ModifyQuest quest in _playerComponent.QuestList.Where(s => s.EQuestState == EQuestState.Progress))
            {
                if (quest.EQuest == EQuest.Talk)
                    if (quest.QuestMasterList.Any(a => a == QuestMaster.ID))
                    {
                        if (!QuestMaster.QuestList.Contains(quest))
                            QuestMaster.QuestList.Add(quest);
                    }
                    else { }
                else if (quest.EQuest == EQuest.Delivery)
                {
                    if (QuestMaster.ID == quest.QuestMasterSubmit)
                    {
                        if (!QuestMaster.QuestList.Contains(quest))
                        {
                            QuestMaster.QuestList.Add(quest);
                        }
                    }
                }
                else if (quest.EQuest == EQuest.Kill)
                {

                }
            }
        }
    }
}
