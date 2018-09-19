using System.Collections.Generic;
using Assets.Script.Extension;
using Assets.Script.Interaction;
using Assets.Script.Menu;
using Assets.Script.SpellFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class HUDSpellBook : MonoBehaviour
    {
        private GameObject _skillBookObject;
        private Transform _background;
        private Transform _viewPort;
        public static bool Visible;
        public static bool CanIDeactive;
        public GameObject GraphicsPanel;
        private Vector3 _defaultPosition;
        private bool _shown;
        void Awake()
        {
            _skillBookObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/SalesManPrefab"), GraphicsPanel.transform);
            _skillBookObject.name = "SpellBook";
            _background = _skillBookObject.transform.Find("Background");
            _viewPort = _background.Find("ScrollView").Find("Viewport");
            _background.Find("DragPanel").Find("Text").GetComponent<Text>().text = "Spell book";
            _background.Find("Exit").GetComponent<Button>().onClick.AddListener(OnHide);
            _background.Find("ExitKey").GetComponent<Button>().onClick.AddListener(OnHide);
            _background.Find("Apply").gameObject.SetActive(false);
            _skillBookObject.SetActive(false);
            Utilities.DisableOrEnableAll(_skillBookObject);
            Visible = false;
            CanIDeactive = true;
        }

        void Start()
        {
            foreach (Spell spell in Database.SpellDatabase)
            {
                GameObject spellObj = Instantiate(Resources.Load<GameObject>("Prefab/SpellItem"));
                spellObj.transform.Find("SpellSlot").GetComponent<Image>().sprite = spell.Icon;
                spellObj.transform.Find("Money").GetComponent<Text>().text = spell.GoldToAccess.ToString();
                spellObj.transform.Find("Level").GetComponent<Text>().text = spell.LevelToAccess.ToString();
                GameObject spellInstantiate = Instantiate(Resources.Load<GameObject>("Prefab/Spell"), spellObj.transform.Find("SpellSlot"));
                spellInstantiate.name = "Spell";
                spellInstantiate.GetComponent<ComponentSpell>().Spell = new Spell(spell);
                GameObject spellInfo = Instantiate(Resources.Load<GameObject>("Prefab/Info"), transform);
                spellInfo.transform.Find("Background").Find("Name").GetComponent<Text>().text = spell.Name;
                spellInfo.transform.Find("Background").Find("Desc").GetComponent<Text>().text = "You need level " + spell.LevelToAccess + " and " + spell.GoldToAccess + " gold to unlock.\n" + spell.Description;
                spellInfo.SetActive(false);
                spellInstantiate.GetComponent<ComponentSpell>().SpellInfo = spellInfo;
                spellInstantiate.GetComponent<Image>().sprite = spell.Icon;
                spellInstantiate.transform.Find("Cooldown").GetComponent<Image>().sprite = spell.Icon;
                spellObj.transform.Find("SpellSlot").GetComponent<SpellSlot>().IsBook = true;
                spellObj.transform.Find("Text").GetComponent<Text>().text = spell.Name;
                spellObj.transform.SetParent(_viewPort);
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.L) && !Visible)
            {
                OnVisible();
            }
            else if (Input.GetKeyUp(KeyCode.L) && Visible)
                OnHide();
        }

        public void OnVisible()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            Utilities.DisableOrEnableAll(_skillBookObject, true);
            MainPanel.OpenWindow(_skillBookObject.name);
            _skillBookObject.transform.SetAsLastSibling();
            Visible = true;
            if (_shown)
            {
                _background.transform.position = _defaultPosition;
            }
            if (!_skillBookObject.activeSelf)
                _skillBookObject.SetActive(true);
        }

        public void OnHide()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            if (CanIDeactive)   //ošetření kvůli tomu že pokud jsem spustil spell z knihy a knihu poté deaktivoval tak cooldown se zasekl
            {
                _skillBookObject.SetActive(false);  //deaktivace celého komponentu
            }
            else
            {
                _defaultPosition = _background.transform.position;
                _background.transform.position = Vector3.zero;
                _shown = true;
                //_canvas.enabled = false;    //deaktivace pouze kanvasu              
            }
            MainPanel.CloseWindow(_skillBookObject.name);
            Utilities.DisableOrEnableAll(_skillBookObject);
            Visible = false;
        }
    }
}