using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.QuestFolder
{
    public enum EQuestMasterState
    {
        Writen,
        Choise,
        Talking,
        None
    }

    public abstract class QuestMaster : IBasicAtribute, IQuestMaster
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int Level { get; set; }
        public GameObject Prefab { get; set; }
        public Vector3 Position { get; set; }
        protected const int LINE = 20;
        public List<ModifyQuest> QuestList { get; private set; }
        public Transform Mark { get; set; }
        private bool _reading;
        private int selected;
        public delegate void DelegateQuest(ModifyQuest quest);

        public bool Talking;
        private ModifyQuest _selectedQuest;
        private bool _instantiate;

        private event DelegateQuest EventAcceptQuest;
        private event DelegateQuest EventSubmitQuest;
        private event DelegateQuest EventCheckQuest;
        private GameObject _questList;
        private GameObject _questInfo;
        private GameObject _questInfoPanel;
        private GameObject _todoPanel;
        private List<GameObject> _questButtons;
        private List<GameObject> _questItemReward;
        private Transform _exitKey;
        private Transform _applyButton;
        private Transform _exitButton;
        private Transform _viewport;
        private Transform _background;
        private Text _applyButtonText;
        protected EQuestMasterState EQuestMasterState;
        protected QuestMaster()
        {
            QuestList = new List<ModifyQuest>();
            _questButtons = new List<GameObject>();
            _questItemReward = new List<GameObject>();
            EQuestMasterState = EQuestMasterState.Choise;
            _selectedQuest = null;
            _reading = false;
            //start úkolu
            EventAcceptQuest += Accept;
            //kontrola úkolu
            EventCheckQuest += Delivery;
            EventCheckQuest += Talk;
            EventCheckQuest += TalkComplete;
            //ukončení úkolu
            EventSubmitQuest += Submit;
            EventSubmitQuest += Reward;
            selected = 0;
            Talking = false;
            _instantiate = false;
        }

        private void OnBack()
        {
            if (_reading)
            {
                EQuestMasterState = EQuestMasterState.Choise;
                _reading = false;
                _exitButton.Find("Text").GetComponent<Text>().text = "Exit";
                _applyButton.gameObject.SetActive(false);
                GameObject.Destroy(_questInfo);
                _exitButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _exitButton.GetComponent<Button>().onClick.AddListener(OnExit);
            }
            else
                OnExit();
        }

        public void OnExit()
        {
            MainPanel.CloseWindow("QuestWindow");
            EQuestMasterState = EQuestMasterState.Choise;
            _applyButton.gameObject.SetActive(false);
            _exitButton.Find("Text").GetComponent<Text>().text = "Exit";
            QuestMasterObject.Visible = false;
            Talking = false;
            _exitButton.GetComponent<Button>().onClick.RemoveAllListeners();
            _exitButton.GetComponent<Button>().onClick.AddListener(OnExit);
            GameObject.Destroy(_questInfo);
            foreach (GameObject obj in _questButtons)
            {
                GameObject.Destroy(obj);
            }
            _questButtons.Clear();
            Utilities.DisableOrEnableAll(_questList);
            _questList.SetActive(false);
        }

        private void OnAccept()
        {
            if (_reading)
            {
                if (_selectedQuest.EQuestState == EQuestState.Progress)
                    OnCheckQuest(_selectedQuest);
                if (_selectedQuest.EQuestState == EQuestState.Complete &&
                    _applyButton.Find("Text").GetComponent<Text>().text == "Complete")
                {
                    if (_selectedQuest.ItemReward.Count > 0)
                    {
                        if (!SlotManagement.IsInventoryFull())
                            OnSubmitQuest(_selectedQuest);
                        else
                        {
                            Debug.LogWarning("#004 Inventory is full [ThisQuestMaster]");
                            MyDebug.LogWarning("#004 Inventory is full [ThisQuestMaster]");
                        }
                    }
                }
                if (_selectedQuest.EQuestState == EQuestState.None)
                    OnAcceptQuest(_selectedQuest);
                OnBack();
            }
        }

        protected virtual void Submit(ModifyQuest quest)
        {
            QuestList.Remove(quest);
        }

        protected virtual void Reward(ModifyQuest quest)
        {
            HUDQuestList.RemoveQuest(quest);
        }

        protected virtual void Accept(ModifyQuest quest)
        {
            HUDQuestList.AddQuest(quest);
            QuestList.Remove(quest);
            if (quest.EQuest == EQuest.Talk)
                foreach (int id in quest.QuestMasterList)
                {
                    if (!IdToQuestMaster(id).QuestList.Contains(quest))
                        IdToQuestMaster(id).QuestList.Add(quest);
                }
            else if (quest.EQuest == EQuest.Delivery || quest.EQuest == EQuest.Kill)
            {
                if (!IdToQuestMaster(quest.QuestMasterSubmit).QuestList.Contains(quest))
                    IdToQuestMaster(quest.QuestMasterSubmit).QuestList.Add(quest);
            }
        }

        public virtual void Delivery(ModifyQuest quest)
        {
            QuestCompleteAdd(quest);
        }

        private void Talk(ModifyQuest quest)
        {
            if (quest.EQuest == EQuest.Talk && quest.QuestMasterList.Any(s => s == ID))
            {
                quest.QuestMasterTalkedList.Add(ID);
                quest.QuestMasterList.Remove(ID);
                QuestList.Remove(quest);
            }
        }

        private void TalkComplete(ModifyQuest quest)
        {
            if (quest.EQuest == EQuest.Talk && quest.QuestMasterList.All(quest.QuestMasterTalkedList.Contains))
            {
                quest.EQuestState = EQuestState.Complete;
                QuestCompleteAdd(quest);
            }
        }

        private void QuestCompleteAdd(ModifyQuest quest)
        {
            //Debug.Log("add");
            if (!IdToQuestMaster(quest.QuestMasterSubmit).QuestList.Contains(quest))
                IdToQuestMaster(quest.QuestMasterSubmit).QuestList.Add(quest);
        }

        private void CanvasInstantiate()
        {
            _background = _questList.transform.Find("Background");
            _viewport = _background.Find("ScrollView").Find("Viewport");
            _exitKey = _background.Find("ExitKey");
            _exitButton = _background.Find("Exit");
            _applyButton = _background.Find("Apply");
            _applyButtonText = _applyButton.Find("Text").GetComponent<Text>();
            _background.Find("DragPanel").Find("Text").GetComponent<Text>().text = Name;
            _applyButton.gameObject.SetActive(false);
            _applyButton.GetComponent<Button>().onClick.AddListener(OnAccept);
            _exitButton.GetComponent<Button>().onClick.AddListener(OnExit);
            _exitKey.GetComponent<Button>().onClick.AddListener(OnExit);
            _questList.SetActive(false);
            Utilities.DisableOrEnableAll(_questList);
        }


        public void ShowAvailable()
        {
            if (Talking)
            {
                QuestMasterObject.Visible = true;
                if (!_instantiate)
                {
                    MainPanel.OpenWindow("QuestWindow");
                    _questList = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestListPrefab"), GameObject.Find("Graphics").transform);
                    _questList.name = "QuestListPrefab";
                    CanvasInstantiate();
                    _instantiate = true;
                }
                int i = 0;
                _questList.SetActive(true);
                Utilities.DisableOrEnableAll(_questList, true);
                if (_questButtons.Count == 0)
                    foreach (ModifyQuest quest in QuestList.ToList())
                    {
                        if (EQuestMasterState == EQuestMasterState.Choise && EQuestMasterState != EQuestMasterState.Writen)
                        {
                            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestListItem"));
                            button.transform.SetParent(_viewport);
                            button.transform.Find("Text").GetComponent<Text>().text = quest.Name;
                            button.GetComponent<Button>().onClick.AddListener(delegate
                            {
                                DrawQuest(quest);
                                EQuestMasterState = EQuestMasterState.Writen;
                                _reading = true;
                                selected = QuestList.IndexOf(quest);
                            });
                            _questButtons.Add(button);
                        }
                        if (EQuestMasterState == EQuestMasterState.Writen && quest.Equals(QuestList[selected]))
                        {
                            DrawQuest(quest);
                            _reading = true;
                        }
                        i++;
                    }
            }
        }

        protected virtual void DrawQuest(ModifyQuest quest)
        {
            if (_questInfo == null)
            {
                _selectedQuest = quest;
                foreach (GameObject obj in _questButtons)
                {
                    GameObject.Destroy(obj);
                }
                _questButtons.Clear();
                ButtonText(quest);
                _questInfo = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestInfo"), _background, false);
                _questInfo.name = "QuestInfo";
                _questInfo.transform.Find("ScrollView").Find("Viewport").Find("Text").GetComponent<Text>().text = _selectedQuest.Description;
                _questInfoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestItemPanel"), _questInfo.transform);
                _questInfoPanel.name = "QuestItemPanel";
                _todoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/TodoPanel"), _questInfo.transform.Find("ScrollView").Find("Viewport"), false);
                _todoPanel.transform.Find("TodoTextPre").GetComponent<Text>().text = SetTodoText(quest);
                Transform moneyExpPanel = _questInfoPanel.transform.Find("MoneyExpPanel");
                if (quest.ItemReward.Count > 0)
                {
                    foreach (QuestItem qItem in quest.ItemReward)
                    {
                        GameObject qItemObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestRewardItem"),
                            _questInfoPanel.transform, false);
                        qItemObject.GetComponent<Image>().sprite = qItem.Item.Icon;
                        if (qItem.Quantity > 1)
                            qItemObject.transform.Find("Stack").GetComponent<Text>().text = qItem.Quantity.ToString();
                        _questItemReward.Add(qItemObject);
                    }
                }
                moneyExpPanel.SetAsLastSibling();
                moneyExpPanel.Find("TextMoney").GetComponent<Text>().text = quest.MoneyReward.ToString();
                moneyExpPanel.Find("TextExperiences").GetComponent<Text>().text = quest.Experiences.ToString();
                Canvas.ForceUpdateCanvases();              
                _exitButton.Find("Text").GetComponent<Text>().text = "Back";
                _exitButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _exitButton.GetComponent<Button>().onClick.AddListener(OnBack);          
            }
        }

        private string SetTodoText(ModifyQuest quest)
        {
            string tmp = "You have to ";
            switch (quest.EQuest)
            {
                case EQuest.Delivery:
                    tmp += "deliver";
                    if (quest.ItemToDeliver != null)
                    {
                        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestItemNeed"),
                            _todoPanel.transform, false);
                        obj.GetComponent<Image>().sprite = quest.ItemToDeliver.Icon;
                        obj.transform.Find("Stack").GetComponent<Text>().text = quest.ItemToDeliveryQuantity.ToString();
                    }
                    else
                        tmp += " some information";
                    break;
                case EQuest.Talk:
                    tmp += "talk to ";
                    int counter = 1;
                    foreach (int id in quest.QuestMasterList)
                    {
                        tmp += IdToQuestMaster(id).Name;
                        if (quest.QuestMasterList.Count != counter)
                            tmp += ", ";
                        counter++;
                    }
                    break;
                case EQuest.Kill:
                    tmp += "kill "+quest.CurrentKills+"/"+quest.TotalKills+" "+quest.EnemyId;
                    break;
            }
            return tmp;
        }

        private void ButtonText(ModifyQuest quest)
        {
            if (quest.EQuestState == EQuestState.None)
            {
                _applyButton.gameObject.SetActive(true);
                _applyButtonText.text = "Accept";
                //ACCEPT
            }
            else if (quest.EQuestState == EQuestState.Complete)
            {
                _applyButton.gameObject.SetActive(true);
                _applyButtonText.text = "Complete";
                //complete
            }
            else if (quest.EQuestState == EQuestState.Progress)
            {
                if (quest.EQuest == EQuest.Talk)
                {
                    _applyButtonText.text = "Talk";
                    _applyButton.gameObject.SetActive(true);
                    //talk
                }
                else if (quest.EQuest == EQuest.Delivery)
                {
                    _applyButtonText.text = "Delivery";
                    _applyButton.gameObject.SetActive(true);
                    //delivery
                }
                else if (quest.EQuest == EQuest.Kill)
                {
                    _applyButtonText.text = "";
                    //kill
                }
            }
        }
        public virtual void OnAcceptQuest(ModifyQuest quest)
        {
            if (EventAcceptQuest != null)
                EventAcceptQuest.Invoke(quest);
        }

        public virtual void OnSubmitQuest(ModifyQuest quest)
        {
            if (EventSubmitQuest != null) EventSubmitQuest.Invoke(quest);
        }

        protected virtual void OnCheckQuest(ModifyQuest quest)
        {
            if (EventCheckQuest != null) EventCheckQuest.Invoke(quest);
        }

        public static QuestMaster IdToQuestMaster(int id)
        {
            return QuestMasterGenerate.QuestMasterList.Find(s => s.Id == id).ThisQuestMaster;
        }

    }
}
