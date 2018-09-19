using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.Interaction;
using Assets.Script.Menu;
using Assets.Script.QuestFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class HUDQuestList : MonoBehaviour
    {
        internal class ButtonQuest
        {
            public int QuestId;
            public GameObject GameObject;

            public ButtonQuest(int questId, GameObject gameObject)
            {
                QuestId = questId;
                GameObject = gameObject;
            }
        }
        private PlayerComponent _playerComponent;
        public static bool Visible;
        public static bool CanIDeactive = true;
        private static GameObject _questListObject;
        private static Text _exitButtText;
        private static Transform _exitButtTransform;
        private static Transform _backgroundTransform;
        private static Transform _viewPort;
        private static GameObject _questItemPrefab;
        private static List<GameObject> _questItemReward = new List<GameObject>();
        private static List<ButtonQuest> _objects;
        private static GameObject _questInfo;
        private static GameObject _todoPanel;
        private static GameObject _questInfoPanel;
        private static Transform _exitKeyTransform;
        public GameObject GraphicsPanel;
        void Start()
        {
            _questListObject = Instantiate(Resources.Load<GameObject>("Prefab/QuestListPrefab"), GraphicsPanel.transform);
            _questListObject.name = "QuestList";
            _backgroundTransform = _questListObject.transform.Find("Background");
            _viewPort = _backgroundTransform.Find("ScrollView").Find("Viewport");
            _exitButtTransform = _backgroundTransform.transform.Find("Exit");
            _exitButtTransform.GetComponent<Button>().onClick.AddListener(OnHide);
            _exitButtText = _exitButtTransform.Find("Text").GetComponent<Text>();
            _backgroundTransform.transform.Find("Apply").gameObject.SetActive(false);
            _backgroundTransform.Find("DragPanel").Find("Text").GetComponent<Text>().text = "QuestList";
            _questItemPrefab = Resources.Load<GameObject>("Prefab/QuestListItem");
            _exitKeyTransform = _backgroundTransform.Find("ExitKey");
            _exitKeyTransform.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnBack();
                OnHide();
            });
            _objects = new List<ButtonQuest>();
            _questListObject.SetActive(false);
            Utilities.DisableOrEnableAll(_questListObject);
        }

        void Update()
        {
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            }

            if (Input.GetKeyUp(KeyCode.U) && !_questListObject.activeSelf)
            {
                OnVisible();
                _backgroundTransform.transform.Find("Apply").gameObject.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.U) && _questListObject.activeSelf)
            {
                OnBack();
                OnHide();
            }
        }

        public static void AddQuest(ModifyQuest quest)
        {
            GameObject questItem = Instantiate(_questItemPrefab);
            _objects.Add(new ButtonQuest(quest.QuestID, questItem));
            questItem.transform.Find("Text").GetComponent<Text>().text = quest.Name;
            questItem.name = quest.QuestID.ToString();
            questItem.transform.SetParent(_viewPort, true);
            questItem.transform.localScale = Vector3.one;
            questItem.GetComponent<Button>().onClick.AddListener(delegate { OnShowQuestInfo(quest); });
            questItem.GetComponent<Button>().enabled = true;
        }

        public static void OnShowQuestInfo(ModifyQuest quest)
        {
            foreach (ButtonQuest obj in _objects)
            {
                obj.GameObject.SetActive(false);
            }
            _questInfo = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestInfo"), _backgroundTransform, false);
            _questInfo.name = "QuestInfo";
            _questInfo.transform.Find("ScrollView").Find("Viewport").Find("Text").GetComponent<Text>().text = quest.Description;
            _questInfoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/QuestItemPanel"), _questInfo.transform);
            _questInfoPanel.name = "QuestItemPanel";
            _todoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/TodoPanel"), _questInfo.transform.Find("ScrollView").Find("Viewport"), false);
            _todoPanel.transform.Find("TodoTextPre").GetComponent<Text>().text = SetTodoText(quest);
            Transform moneyExpPanel = _questInfoPanel.transform.Find("MoneyExpPanel");
            if (quest.ItemReward.Count > 0)
            {
                foreach (QuestItem qItem in quest.ItemReward)
                {
                    _questItemReward.Add(Utilities.InstatiateItem(qItem.Item, qItem.Quantity.ToString(), _questInfoPanel.transform));
                }
            }
            moneyExpPanel.SetAsLastSibling();
            moneyExpPanel.Find("TextMoney").GetComponent<Text>().text = quest.MoneyReward.ToString();
            moneyExpPanel.Find("TextExperiences").GetComponent<Text>().text = quest.Experiences.ToString();
            Canvas.ForceUpdateCanvases();
            _exitButtTransform.Find("Text").GetComponent<Text>().text = "Back";
            _exitButtTransform.GetComponent<Button>().onClick.RemoveAllListeners();
            _exitButtTransform.GetComponent<Button>().onClick.AddListener(OnBack);
        }

        private static string SetTodoText(ModifyQuest quest)
        {
            string tmp = "You have to ";
            switch (quest.EQuest)
            {
                case EQuest.Delivery:
                    tmp += "deliver";
                    if (quest.ItemToDeliver != null)
                    {
                        _questItemReward.Add(Utilities.InstatiateItem(quest.ItemToDeliver, quest.ItemToDeliveryQuantity.ToString(), _todoPanel.transform));
                    }
                    else
                        tmp += " some information";
                    break;
                case EQuest.Talk:
                    tmp += "talk to ";
                    int counter = 1;
                    foreach (int id in quest.QuestMasterList)
                    {
                        tmp += QuestMaster.IdToQuestMaster(id).Name;
                        if (quest.QuestMasterList.Count != counter)
                            tmp += ", ";
                        counter++;
                    }
                    break;
                case EQuest.Kill:
                    tmp += "kill " + quest.CurrentKills + "/" + quest.TotalKills + " " + quest.EnemyId;
                    break;
            }
            return tmp;
        }

        private static void OnBack()
        {
            foreach (ButtonQuest obj in _objects)
            {
                obj.GameObject.SetActive(true);
            }
            _exitButtText.text = "Exit";
            DestroyObject(_questInfo);
            _exitButtTransform.GetComponent<Button>().onClick.RemoveAllListeners();
            _exitButtTransform.GetComponent<Button>().onClick.AddListener(OnHide);
        }

        public static void OnHide()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            MainPanel.CloseWindow(_questListObject.name);
            Visible = false;
            _questListObject.SetActive(false);
            Utilities.DisableOrEnableAll(_questListObject);
        }

        public static void OnVisible()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            _questListObject.transform.SetAsLastSibling();
            MainPanel.OpenWindow(_questListObject.name);
            _questListObject.SetActive(true);
            Utilities.DisableOrEnableAll(_questListObject, true);
            Visible = true;
        }

        public static void RemoveQuest(ModifyQuest quest)
        {
            //Debug.Log();
            ButtonQuest buttonQuest = _objects.Find(s => s.QuestId == quest.QuestID);
            if (buttonQuest != null)
            {
                _objects.Remove(buttonQuest);
                DestroyObject(buttonQuest.GameObject);
            }
        }

        public void OnBecameInvisible()
        {
            enabled = false;
        }

        public void OnBecameVisible()
        {
            enabled = true;
        }
    }
}