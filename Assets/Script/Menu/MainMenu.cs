using System;
using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.Interaction;
using Assets.Script.InventoryFolder;
using Assets.Script.QuestFolder;
using Assets.Scripts.InventoryFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script.Menu
{
    public enum EMenuButtons
    {
        None,
        New_game,
        Save,
        Load,
        Settings,
        Back_to_main_menu,
        Quit
    }
    public class MainMenu : MonoBehaviour
    {
        public static bool Visible;
        public static bool Enabled;
        public bool CouldBeExited;
        public bool IsMainMenu;
        private bool isExitDialogVisible;
        public GameObject gameSettings;
        private Transform _backgroundTransform;
        private Transform _newGameTransform;
        private Transform _saveTransform;
        private Transform _loadTransform;
        private Transform _settingTransform;
        private Transform _backToMenuTransform;
        private Transform _resumeTransform;
        private Transform _quitGameTransform;
        private GameObject _dialog;
        void Awake()
        {
            _backgroundTransform = gameObject.transform.Find("Background");
            _newGameTransform = _backgroundTransform.Find("NewGame");
            _saveTransform = _backgroundTransform.Find("SaveGame");
            _loadTransform = _backgroundTransform.Find("LoadGame");
            _settingTransform = _backgroundTransform.Find("Settings");
            _backToMenuTransform = _backgroundTransform.Find("BackToMainMenu");
            _resumeTransform = _backgroundTransform.Find("Resume");
            _quitGameTransform = _backgroundTransform.Find("QuitGame");
            CouldBeExited = true;
        }

        // Use this for initialization
        void Start()
        {
            LoadSettings();
            _newGameTransform.GetComponent<Button>().onClick.AddListener(OnNewGame);
            if (IsMainMenu)
            {
                _saveTransform.gameObject.SetActive(false);
                _backToMenuTransform.gameObject.SetActive(false);
                _resumeTransform.gameObject.SetActive(false);
                _newGameTransform.gameObject.SetActive(true);
            }
            else
            {
                _newGameTransform.gameObject.SetActive(false);
                _resumeTransform.gameObject.SetActive(true);
                _saveTransform.gameObject.SetActive(true);
                _backToMenuTransform.gameObject.SetActive(true);
                _resumeTransform.GetComponent<Button>().onClick.AddListener(OnResume);
                _backToMenuTransform.GetComponent<Button>().onClick.AddListener(OnMainMenu);
                _saveTransform.GetComponent<Button>().onClick.AddListener(OnSave);
                _backgroundTransform.gameObject.SetActive(false);
                Visible = false;
            }
            _loadTransform.GetComponent<Button>().onClick.AddListener(OnLoad);
            _settingTransform.GetComponent<Button>().onClick.AddListener(OnSettings);
            _quitGameTransform.GetComponent<Button>().onClick.AddListener(OnQuit);
            Enabled = true;
        }



        // Update is called once per frame
        void Update()
        {
            if (!IsMainMenu)
                if (Input.GetKeyUp(KeyCode.Escape) && !Visible)
                {
                    if(!MainPanel.IsAnyWindowOpen())
                        OnVisible();
                    else
                        MainPanel.CloseAllWindows();
                }
                else if (Input.GetKeyUp(KeyCode.Escape) && Visible)
                {
                    OnResume();
                }
        }

        public void OnVisible()
        {
            if (BlackScreen.Visible || InGameTime.Visible)
                return;
            transform.SetAsLastSibling();
            MainPanel.OpenWindow(name);
            _backgroundTransform.gameObject.SetActive(true);
            Visible = true;
        }

        private void OnNewGame()
        {
            if (Enabled)
            {
                SceneManager.LoadScene("Character Generate");
            }
        }

        private void OnSave()
        {
            if (Enabled)
                new SaveLoadComponent(true, IsMainMenu, gameObject);
        }

        private void OnLoad()
        {
            if (Enabled)
                new SaveLoadComponent(false, IsMainMenu, gameObject);
        }

        private void OnSettings()
        {

        }

        public void OnResume()
        {
            if (Enabled && CouldBeExited)
            {
                MainPanel.CloseWindow(name);
                _backgroundTransform.gameObject.SetActive(false);
                Visible = false;
            }
        }

        private void OnQuit()
        {
            if (Enabled)
            {
                _dialog = Instantiate(Resources.Load<GameObject>("Prefab/DialogWindow"));
                Enabled = false;
                InitDialog(OnDialogQuitYes, "Do you really want to exit game?");
            }
        }

        private void OnDialogNo()
        {
            Destroy(_dialog);
            Enabled = true;
        }

        private void OnDialogMenuYes()
        {
            Destroy(_dialog);
            SceneManager.LoadScene("Menu");
            Enabled = true;
            OnResume();

        }

        private void OnDialogQuitYes()
        {
            Destroy(_dialog);
            Application.Quit();
            Enabled = true;
            OnResume();
        }

        private void OnMainMenu()
        {
            if (Enabled)
            {
                _dialog = Instantiate(Resources.Load<GameObject>("Prefab/DialogWindow"));
                Enabled = false;
                InitDialog(OnDialogMenuYes, "Do you really want to go to main menu?");
                Utilities.ClearStaticCaches();
            }
        }

        private void InitDialog(UnityAction dialogYes, string text)
        {
            Transform dialogBackground = _dialog.transform.Find("Background");
            dialogBackground.Find("Yes").GetComponent<Button>().onClick.AddListener(dialogYes);
            dialogBackground.Find("No").GetComponent<Button>().onClick.AddListener(OnDialogNo);
            dialogBackground.Find("Exit").GetComponent<Button>().onClick.AddListener(OnDialogNo);
            dialogBackground.Find("Text").GetComponent<Text>().text = text;
        }

        public void LoadSettings()
        {
            GameObject gs = GameObject.Find("GameSettings");
            if (gs == null)
            {
                GameObject gs1 = Instantiate(gameSettings, Vector3.zero, Quaternion.identity) as GameObject;
                gs1.name = "GameSettings";
            }
            GameObject.Find("GameSettings").GetComponent<GameSettings>();
        }
    }
}
