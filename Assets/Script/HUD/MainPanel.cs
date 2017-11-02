using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder.CraftFolder;
using Assets.Script.Menu;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class MainPanel : MonoBehaviour
    {
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
        void Awake()
        {
            _backGround = gameObject.transform.Find("Background");
            _expCharge = _backGround.Find("ExpCharge");
            _expTextPanel = _backGround.Find("TouchPanel").Find("ExpPanel");
            _buttonPanel = _backGround.Find("ButtonPanel");
            _expTextPanel.gameObject.SetActive(false);
            InitButtons();
        }

        void Start()
        {
            _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerComponent.ExperiencesPooling)
            {
                _expCharge.GetComponent<RectTransform>().sizeDelta = new Vector2(CurrBarLenght(_playerComponent.ExpCurrent, _playerComponent.ExpToNextLevel),
                    _expCharge.GetComponent<RectTransform>().sizeDelta.y);
                _expTextPanel.Find("Text").GetComponent<Text>().text = _playerComponent.ExpCurrent + "/" + _playerComponent.ExpToNextLevel + " Experiences";
                PlayerComponent.ExperiencesPooling = false;
            }
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
            GameObject menu = Utilities.ListOfAllObjects.Find(s => s.name == "Menu");
            if (!MainMenu.Visible)
            {
                menu.transform.Find("Background").gameObject.SetActive(true);
                MainMenu.Visible = true;
            }
            else
            {
                menu.transform.Find("Background").gameObject.SetActive(false);
                MainMenu.Visible = false;
            }
        }

        private void OnShowQuestList()
        {
            HUDQuestList.Visible = ShowHide(Utilities.ListOfAllObjects.Find(s => s.name == "QuestList"),HUDQuestList.CanIDeactive);
        }

        private void OnShowSkill()
        {
            HUDSpellBook.Visible = ShowHide(Utilities.ListOfAllObjects.Find(s => s.name == "SpellBook"),HUDSpellBook.CanIDeactive);
        }

        private void OnShowCrafting()
        {
            ComponentCraftMenu.Visible = ShowHide(Utilities.ListOfAllObjects.Find(s => s.name == "CraftMenu"),ComponentCraftMenu.CanIDeactive);
        }

        private void OnShowProfesion()
        {
            HUDProfessionView.Visible = ShowHide(Utilities.ListOfAllObjects.Find(s => s.name == "SkillView"),HUDProfessionView.CanIDeactive);
        }

        private void OnShowStats()
        {
            HUDStatsView.Visible = ShowHide(Utilities.ListOfAllObjects.Find(s => s.name == "StatsView"),HUDStatsView.CanIDeactive);
        }

        private bool ShowHide(GameObject obj, bool canIDeactive)
        {
            if (!obj.activeSelf || !obj.GetComponent<Canvas>().enabled)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);

                }
                if (!obj.GetComponent<Canvas>().enabled)
                {
                    obj.GetComponent<Canvas>().enabled = true;
                }
                Utilities.DisableOrEnableAll(obj, true);
                return true;
            }
            if (obj.activeSelf || obj.GetComponent<Canvas>().enabled)
            {
                if (canIDeactive)   //ošetření kvůli tomu že pokud jsem spustil spell z knihy a knihu poté deaktivoval tak cooldown se zasekl
                {
                    obj.SetActive(false);  //deaktivace celého komponentu
                }
                else
                {
                    obj.GetComponent<Canvas>().enabled = false;    //deaktivace pouze kanvasu              
                }
                Utilities.DisableOrEnableAll(obj);
                return false;
            }
            return false;
        }
        private int CurrBarLenght(float currSize, float maxSize)
        {
            return (int)((currSize / maxSize) * MAXBARLENGHT);
        }
    }
}
