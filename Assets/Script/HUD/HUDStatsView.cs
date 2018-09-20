using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.Interaction;
using Assets.Script.InventoryFolder;
using Assets.Script.InventoryFolder.CraftFolder;
using Assets.Script.Menu;
using Assets.Script.QuestFolder;
using Assets.Script.StatisticsFolder;
using Assets.Scripts;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class HUDStatsView : MonoBehaviour
    {
        private GameObject _statsView;
        private Transform _firstViewPort;
        private Transform _secondViewPort;
        private Transform _exitKey;
        private Transform _background;
        private Character _character;
        private Vector3 offset;
        public static bool Visible;
        public static bool CanIDeactive = true;
        private static readonly Vector3 defaultVect= new Vector3(-1000, -1000);
        private static Vector3 beforePosition;
        public GameObject GraphicsPanel;
        public Transform Char2DObject;
        private void Start()
        {
            _statsView = Instantiate(Resources.Load<GameObject>("Prefab/StatsViewPrefab"),GraphicsPanel.transform);
            _statsView.name = "StatsView";      
            _background = _statsView.transform.Find("Background");
            Char2DObject = _statsView.transform.Find("Camera").Find("Canvas").Find("human");             
            _firstViewPort = _background.Find("ScrollView").Find("Viewport");
            _secondViewPort = _background.Find("ScrollView1").Find("Viewport");
            _background.Find("DragPanel").Find("Text").GetComponent<Text>().text = "Stats view";
            _exitKey = _background.Find("Exit");
            _exitKey.GetComponent<Button>().onClick.AddListener(OnHide);
            Utilities.DisableOrEnableAll(_statsView);
            _background.GetComponent<RectTransform>().position = new Vector3(Screen.width, Screen.height, 0);
            _statsView.SetActive(true);
            beforePosition = _statsView.GetComponent<RectTransform>().position;
            _statsView.GetComponent<RectTransform>().position = defaultVect;
            Visible = false;
        }

        public void OnVisible()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            MainPanel.OpenWindow(_statsView.name);
            _statsView.transform.SetAsLastSibling();
            _statsView.GetComponent<RectTransform>().position = beforePosition;
            Utilities.DisableOrEnableAll(_statsView, true);
            Visible = true;
        }

        public void OnHide()
        {
            if (MainMenu.Visible || InGameTime.Visible)
                return;
            MainPanel.CloseWindow(_statsView.name);
            //_statsView.SetActive(false);
            beforePosition = _statsView.GetComponent<RectTransform>().position;
            _statsView.GetComponent<RectTransform>().position = defaultVect;
            Utilities.DisableOrEnableAll(_statsView);
            Visible = false;
        }

        private void Update()
        {
            if (_character == null)
            {
                _character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().character;
                if (_character != null)
                {
                    Initialize();
                }
            }            
            if (Input.GetKeyUp(KeyCode.C) && !Visible && !CraftSettings.SearchFocused)
            {
                OnVisible();
            }
            else if (Input.GetKeyUp(KeyCode.C) && Visible && !CraftSettings.SearchFocused)
            {
                OnHide();
            }
            if (SlotManagement.StatsChanged)
            {
                Initialize();
                SlotManagement.StatsChanged = false;
            }
            if (Visible)
            {
                Char2DObject.GetComponent<Transform>().Rotate(Vector3.up, 1f);
            }
        }

        private void Initialize()
        {
            int a = 1;
            for (int i = 0; i < _character.skillArray.Length - 3; i++)
            {
                _secondViewPort.Find("Text (" + (i * 2) + ")").GetComponent<Text>().text = ((ESkill)i).ToString();
                _secondViewPort.Find("Text (" + (a) + ")").GetComponent<Text>().text = _character.skillArray[i].ValuesTogether.ToString();
                a += 2;
            }
            a = 1;
            for (int i = 0; i < _character.damageStatsArray.Length; i++)
            {
                if (((EDamageStats) i) != EDamageStats.None)
                {
                    _firstViewPort.Find("Text (" + (i * 2) + ")").GetComponent<Text>().text =
                        ((EDamageStats) i).ToString();
                    _firstViewPort.Find("Text (" + (a) + ")").GetComponent<Text>().text =
                        _character.damageStatsArray[i].ValuesTogether.ToString();     
                }
                a += 2;
            }
        }
    }
}