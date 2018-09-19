using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder.CraftFolder;
using Assets.Script.Menu;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class MainPanel : MonoBehaviour
    {
        protected struct WindowOpen
        {
            public string Name;
            public int Index;

            public WindowOpen(string name, int index)
            {
                Name = name;
                Index = index;
            }
        }

        private Transform _expCharge;
        private Transform _expTextPanel;
        private Transform _backGround;
        private Transform _buttonPanel;
        private PlayerComponent _playerComponent;
        private const int MAXBARLENGHT = 648;
        private GameObject _questListObj;
        private GameObject _statsViewObj;
        private GameObject _professionObj;
        private GameObject _menuObj;
        private GameObject _craftMenuObj;
        private GameObject _skillViewObj;
        private static GameObject _gameMaster;
        private static MainMenu _menu;
        private RectTransform _expRect;
        private Text _expText;
        private static List<WindowOpen> OpenedWindows;
        private static int NumberOfWindows;
        private bool startRefresh;
        void Awake()
        {
            OpenedWindows = new List<WindowOpen>();
            _backGround = gameObject.transform.Find("Background");
            _expCharge = _backGround.Find("ExpCharge");
            _expTextPanel = _backGround.Find("TouchPanel").Find("ExpPanel");
            _buttonPanel = _backGround.Find("ButtonPanel");
            _expTextPanel.gameObject.SetActive(false);
            _expRect = _expCharge.GetComponent<RectTransform>();
            _expText = _expTextPanel.Find("Text").GetComponent<Text>();
            InitButtons();
        }

        void Start()
        {
            _gameMaster = GameObject.Find("GameMaster");
            _menu = GameObject.Find("Graphics").transform.Find("Menu").GetComponent<MainMenu>();
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            }
            startRefresh = false;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(OpenedWindows.Count);
            if (!startRefresh || PlayerComponent.ExperiencesPooling)
            {
                _expRect.sizeDelta = new Vector2(CurrBarLenght(_playerComponent.ExpCurrent, _playerComponent.ExpToNextLevel),
                    _expRect.sizeDelta.y);
                _expText.text = _playerComponent.ExpCurrent + "/" + _playerComponent.ExpToNextLevel + " Experiences";
                PlayerComponent.ExperiencesPooling = false;
                startRefresh = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("The cursor entered the selectable UI element.");
        }

        private void InitButtons()
        {
            _buttonPanel.Find("Stats").GetComponent<Button>().onClick.RemoveAllListeners();     //vyčistení všech předcházejících listneru
            _buttonPanel.Find("Stats").GetComponent<Button>().onClick.AddListener(OnShowStats);     //BUG metoda start a awake je volána 2x (vytvořilo 2x listner a funkce se volala 2x poprvé okno otevřela podruhé zavřela)
            _buttonPanel.Find("Profesion").GetComponent<Button>().onClick.RemoveAllListeners();
            _buttonPanel.Find("Profesion").GetComponent<Button>().onClick.AddListener(OnShowProfesion);
            _buttonPanel.Find("Crafting").GetComponent<Button>().onClick.RemoveAllListeners();
            _buttonPanel.Find("Crafting").GetComponent<Button>().onClick.AddListener(OnShowCrafting);
            _buttonPanel.Find("Skill").GetComponent<Button>().onClick.RemoveAllListeners();
            _buttonPanel.Find("Skill").GetComponent<Button>().onClick.AddListener(OnShowSkill);
            _buttonPanel.Find("Quest").GetComponent<Button>().onClick.RemoveAllListeners();
            _buttonPanel.Find("Quest").GetComponent<Button>().onClick.AddListener(OnShowQuestList);
            _buttonPanel.Find("Menu").GetComponent<Button>().onClick.RemoveAllListeners();
            _buttonPanel.Find("Menu").GetComponent<Button>().onClick.AddListener(OnShowMenu);
        }

        private void OnShowMenu()
        {
            if (!MainMenu.Visible)
            {
                CloseAllWindows();
                _menu.OnVisible(); 
            }
            else
            {
                _menu.OnResume();
            }
        }

        private void OnShowQuestList()
        {
            if (!HUDQuestList.Visible)
                HUDQuestList.OnVisible();
            else HUDQuestList.OnHide();
        }

        private void OnShowSkill()
        {
            if (!HUDSpellBook.Visible)
                _gameMaster.GetComponent<HUDSpellBook>().OnVisible();
            else _gameMaster.GetComponent<HUDSpellBook>().OnHide();
        }

        private void OnShowCrafting()
        {
            if (!ComponentCraftMenu.Visible)
                _gameMaster.GetComponent<ComponentCraftMenu>().OnVisible();
            else _gameMaster.GetComponent<ComponentCraftMenu>().OnHide();
        }

        private void OnShowProfesion()
        {
            if (!HUDProfessionView.Visible)
                _gameMaster.GetComponent<HUDProfessionView>().OnVisible();
            else _gameMaster.GetComponent<HUDProfessionView>().OnHide();
        }

        private void OnShowStats()
        {
            if (!HUDStatsView.Visible)
                _gameMaster.GetComponent<HUDStatsView>().OnVisible();
            else _gameMaster.GetComponent<HUDStatsView>().OnHide();
        }

        private int CurrBarLenght(float currSize, float maxSize)
        {
            return (int)((currSize / maxSize) * MAXBARLENGHT);
        }

        public static void OpenWindow(string objName)
        {
            OpenedWindows.Add(new WindowOpen(objName, NumberOfWindows));
            NumberOfWindows++;
            if (NumberOfWindows > 2)
            {
                Selector(OpenedWindows.First().Name);
            }
        }

        public static void CloseAllWindows()
        {
            if(HUDStatsView.Visible)
            _gameMaster.GetComponent<HUDStatsView>().OnHide();
            if(HUDSpellBook.Visible)
            _gameMaster.GetComponent<HUDSpellBook>().OnHide();
            if(HUDQuestList.Visible)
            HUDQuestList.OnHide();
            if(HUDProfessionView.Visible)
            _gameMaster.GetComponent<HUDProfessionView>().OnHide();
            if(ComponentCraftMenu.Visible)
            _gameMaster.GetComponent<ComponentCraftMenu>().OnHide();
        }

        public static void CloseWindow(string objName)
        {
            for (int i = 0; i < OpenedWindows.Count; i++)
            {
                if (OpenedWindows[i].Name == objName)
                    OpenedWindows.RemoveAt(i);
            }
            NumberOfWindows--;
        }

        public static bool IsAnyWindowOpen()
        {
            return OpenedWindows.Count != 0;
        }

        private static void Selector(string window)
        {
            
            switch (window)
            {
                case "StatsView":
                    _gameMaster.GetComponent<HUDStatsView>().OnHide();
                    break;
                case "MainMenu":
                    _menu.OnResume();
                    break;
                case "SpellBook":
                    _gameMaster.GetComponent<HUDSpellBook>().OnHide();
                    break;
                case "QuestList":
                    HUDQuestList.OnHide();
                    break;
                case "SkillView":
                    _gameMaster.GetComponent<HUDProfessionView>().OnHide();
                    break;
                case "CraftMenu":
                    _gameMaster.GetComponent<ComponentCraftMenu>().OnHide();
                    break;
            }
        }
    }
}
