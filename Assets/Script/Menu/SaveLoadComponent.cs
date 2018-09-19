using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script.Menu
{
    public class SaveLoadComponent
    {
        public static bool Visible;
        private bool _isSave;
        private bool _isMainMenu;
        private GameSettings _gameSettings;
        private GameObject _saveLoadObj;
        private GameObject _menu;
        private Transform[] _buttonList;

        private const int NumOfSaves = 5;
        public SaveLoadComponent(bool isSave, bool isMainMenu, GameObject menu)
        {
            _gameSettings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
            _buttonList = new Transform[NumOfSaves];
            _menu = menu;
            _isMainMenu = isMainMenu;
            _isSave = isSave;
            menu.SetActive(false);
            _saveLoadObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/SaveLoad"));
            Transform background = _saveLoadObj.transform.Find("Background");
            for (int i = 0; i < NumOfSaves; i++)
            {
                _buttonList[i] = background.Find("Position" + i);
                if (GameSettings.IsSaveExist("Save_" + (i+1)).ToString() == "")
                    _buttonList[i].Find("Text").GetComponent<Text>().text = "Empty";
                else
                    _buttonList[i].Find("Text").GetComponent<Text>().text =
                        GameSettings.IsSaveExist("Save_" + (i+1)).ToString();
            }
            _buttonList[0].GetComponent<Button>().onClick.AddListener(OnSaveLoad1);
            _buttonList[1].GetComponent<Button>().onClick.AddListener(OnSaveLoad2);
            _buttonList[2].GetComponent<Button>().onClick.AddListener(OnSaveLoad3);
            _buttonList[3].GetComponent<Button>().onClick.AddListener(OnSaveLoad4);
            _buttonList[4].GetComponent<Button>().onClick.AddListener(OnSaveLoad5);
            background.Find("Back").GetComponent<Button>().onClick.AddListener(OnExit);
        }

        private void Load(string save)
        {
            if (_isMainMenu)
            {
                GameSettings.GameState = EGameState.MenuLoad;
                SceneManager.LoadScene("Game");
                GameSettings.SavePosition = save;
            }
            else
            {
                GameSettings.GameState = EGameState.InGameLoad;
                _gameSettings.LoadCharacter(save);
                _menu.GetComponent<MainMenu>().CouldBeExited = true;
                _menu.GetComponent<MainMenu>().OnResume();   
            }     
            OnExit();
        }

        private void OnSaveLoad1()
        {
            if (_isSave)
            {
                _buttonList[0].Find("Text").GetComponent<Text>().text = DateTime.Now.ToString();
                _gameSettings.SaveCharacter("Save_1");
            }
            else
            {
                if(_buttonList[0].Find("Text").GetComponent<Text>().text!= "Empty")
                    Load("Save_1");
            }
        }
        private void OnSaveLoad2()
        {
            if (_isSave)
            {
                _buttonList[1].Find("Text").GetComponent<Text>().text = DateTime.Now.ToString();
                _gameSettings.SaveCharacter("Save_2");
            }
            else
            {
                if (_buttonList[1].Find("Text").GetComponent<Text>().text != "Empty")
                    Load("Save_2");
            }
        }
        private void OnSaveLoad3()
        {
            if (_isSave)
            {
                _buttonList[2].Find("Text").GetComponent<Text>().text = DateTime.Now.ToString();
                _gameSettings.SaveCharacter("Save_3");
            }
            else
            {
                if (_buttonList[2].Find("Text").GetComponent<Text>().text != "Empty")
                    Load("Save_3");
            }
        }
        private void OnSaveLoad4()
        {
            if (_isSave)
            {
                _buttonList[3].Find("Text").GetComponent<Text>().text = DateTime.Now.ToString();
                _gameSettings.SaveCharacter("Save_4");
            }
            else
            {
                if (_buttonList[3].Find("Text").GetComponent<Text>().text != "Empty")
                    Load("Save_4");
            }
        }
        private void OnSaveLoad5()
        {
            if (_isSave)
            {
                _buttonList[4].Find("Text").GetComponent<Text>().text = DateTime.Now.ToString();
                _gameSettings.SaveCharacter("Save_5");
            }
            else
            {
                if (_buttonList[4].Find("Text").GetComponent<Text>().text != "Empty")
                    Load("Save_5");
            }
        }


        private void OnExit()
        {
            GameObject.Destroy(_saveLoadObj);
            _menu.SetActive(true);
        }
    }
}