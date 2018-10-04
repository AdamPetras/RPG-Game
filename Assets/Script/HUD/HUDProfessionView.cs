using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.Interaction;
using Assets.Script.InventoryFolder.CraftFolder;
using Assets.Script.Menu;
using Assets.Scripts.InventoryFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class HUDProfessionView : MonoBehaviour
    {
        private Transform _background;
        private GameObject _skillView;
        private Transform _fishingTransform;
        private Transform _cookingTransform;
        private Transform _craftingTransform;
        private Transform _tailoringTransform;
        private Transform _smithingTransform;
        private PlayerComponent _playerComponent;
        public static bool Visible;
        public static bool CanIDeactive = true;
        public GameObject GraphicsPanel;

        private void Awake()
        {
            _skillView = Instantiate(Resources.Load<GameObject>("Prefab/SkillView"), GraphicsPanel.transform);
            _background = _skillView.transform.Find("Background");
            _fishingTransform = _background.Find("Fishing");
            _cookingTransform = _background.Find("Cooking");
            _craftingTransform = _background.Find("Crafting");
            _tailoringTransform = _background.Find("Tailoring");
            _smithingTransform = _background.Find("Smithing");
        }

        private void Start()
        {
            _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            _background.Find("DragPanel").Find("Text").GetComponent<Text>().text = "Profession view";
            _background.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnHide);
            _skillView.name = "SkillView";
            _skillView.SetActive(false);
            Utilities.DisableOrEnableAll(_skillView);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.B) && !_skillView.activeSelf && !CraftSettings.SearchFocused)
            {
                OnVisible();
            }
            else if (Input.GetKeyUp(KeyCode.B) && _skillView.activeSelf && !CraftSettings.SearchFocused)
            {
                OnHide();
            }
            if (CraftDataOperating.ExpChanged) //update poling
            {
                UpdateStats();
                CraftDataOperating.ExpChanged = false;
            }
        }

        public void OnVisible()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            MainPanel.OpenWindow(_skillView.name,gameObject);
            _skillView.transform.SetAsLastSibling();
            _skillView.SetActive(true);
            Utilities.DisableOrEnableAll(_skillView, true);
            Visible = true;
        }

        public void OnHide()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            MainPanel.CloseWindow(_skillView.name);
            Visible = false;
            _skillView.SetActive(false);
        }

        private void UpdateStats()
        {
            ProfesionUpdate(_fishingTransform, EProfession.Fishing);
            ProfesionUpdate(_cookingTransform, EProfession.Cooking);
            ProfesionUpdate(_craftingTransform, EProfession.Crafting);
            ProfesionUpdate(_smithingTransform, EProfession.Smithing);
            ProfesionUpdate(_tailoringTransform, EProfession.Tailoring);
        }

        private void ProfesionUpdate(Transform stat, EProfession prof)
        {
            stat.Find("CurrentExp").GetComponent<Text>().text = _playerComponent.SelectProfession(prof).Experience.ToString();
            stat.Find("Level").GetComponent<Text>().text = "Level: " + _playerComponent.SelectProfession(prof).Level;
            stat.Find("MaxExp").GetComponent<Text>().text = _playerComponent.SelectProfession(prof).NextLevelExp.ToString();
            stat.Find("MinExp").GetComponent<Text>().text = _playerComponent.SelectProfession(prof).Experience.ToString();
            float currBarLenght = (int)(((float)_playerComponent.SelectProfession(prof).Experience / _playerComponent.SelectProfession(prof).NextLevelExp) * 190);
            stat.Find("Charge").GetComponent<RectTransform>().sizeDelta = new Vector2(currBarLenght, stat.Find("Charge").GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}