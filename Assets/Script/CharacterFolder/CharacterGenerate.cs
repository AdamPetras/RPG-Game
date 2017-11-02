using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Camera;
using Assets.Script.Extension;
using Assets.Script.StatisticsFolder;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.CharacterFolder
{
    public class CharacterGenerate : MonoBehaviour
    {
        private PlayerComponent _playerComponent;
        private const int OFFSET = 5;
        private const int TABLE_HEIGHT = 25;
        private const int TABLE_WIDTH = 100;

        public GUIStyle Human;
        public GUIStyle Bull;
        public GUIStyle Elf;
        public GUIStyle Ork;
        public GUIStyle Label;
        public GameObject Prefab;
        private GameObject Player;
        public GameObject gameSettings;
        private List<string> _NotAbleNameList;
        // Use this for initialization
        void Awake()
        {
            Instance(ECharacter.Human);
            InitVulgarList();
            
        }

        // Update is called once per frame
        void Update()
        {
            LoadSettings();
            _playerComponent.transform.Rotate(Vector3.up, 1f);
        }

        void OnGUI()
        {
            ShowName();
            ShowSkills();
            ShowVitals();
            ShowDamageStats();
            ShowExperiences();
            if (_playerComponent.Name.Length < 4 || _NotAbleNameList.Any(_playerComponent.Name.ToLower().Contains))
                ShowCreateLabel();
            else
                CreateButton();
            SelectCharacter();
        }

        private void InitVulgarList()
        {
            _NotAbleNameList = new List<string>
            {
                "kokot",
                "pica",
                "curak",
                "zmrd",
                "mrdat",
                "kurva",
                "mrdka",
                "fuck",
                "f.u.c.k",
                "dick",
                " ",
                "%",
                "#",
                "&",
                "*",
                "$"
            };

        }

        private void ShowName()
        {
            _playerComponent.Name = GUI.TextField(new Rect(Screen.width / 2 - TABLE_WIDTH / 2, TABLE_HEIGHT + OFFSET, TABLE_WIDTH, 25), _playerComponent.Name);
            GUI.Label(new Rect(Screen.width / 2 - TABLE_WIDTH / 2, OFFSET, TABLE_WIDTH, TABLE_HEIGHT), "Enter your name", Label);
        }

        private void ShowExperiences()
        {
            GUI.Label(new Rect(Screen.width / 2 - TABLE_WIDTH / 2, Screen.height - TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), _playerComponent.ExpCurrent + "/" + _playerComponent.ExpToNextLevel, Label);
        }

        private void ShowSkills()
        {
            for (int i = 0; i < Enum.GetValues(typeof(ESkill)).Length; i++)
            {
                GUI.Label(new Rect(-OFFSET * 2 + Screen.width - TABLE_WIDTH * 2, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), ((int)(_playerComponent.character.GetSkill(i).ValuesTogether)).ToString(), Label);
                GUI.Label(new Rect(-OFFSET + Screen.width - TABLE_WIDTH, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), ((ESkill)i).ToString(), Label);
            }
        }

        private void ShowVitals()
        {
            for (int i = 0; i < Enum.GetValues(typeof(EVital)).Length; i++)
            {
                GUI.Label(new Rect(OFFSET, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), ((EVital)i).ToString(), Label);
                GUI.Label(new Rect(TABLE_WIDTH + OFFSET * 2, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), (int)(_playerComponent.character.GetVital(i).CurrentValue) + "/" + (int)_playerComponent.character.GetVital(i).ValuesTogether, Label);
            }
        }

        private void ShowDamageStats()
        {
            for (int i = 0; i < Enum.GetValues(typeof(EDamageStats)).Length; i++)
            {
                GUI.Label(new Rect(OFFSET, OFFSET + (i + Enum.GetValues(typeof(EVital)).Length) * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), ((EDamageStats)i).ToString(), Label);
                GUI.Label(new Rect(TABLE_WIDTH + OFFSET * 2, OFFSET + (i + Enum.GetValues(typeof(EVital)).Length) * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), Math.Round(_playerComponent.character.GetDamageStats(i).CurrentValue, 0).ToString(), Label);
            }
        }

        private void ShowCreateLabel()
        {
            GUI.Label(new Rect(Screen.width - (TABLE_WIDTH * 4) - OFFSET, Screen.height - TABLE_HEIGHT - OFFSET, TABLE_WIDTH * 4, TABLE_HEIGHT), "Name have to contain minimal 4 character without space");
        }

        private void CreateButton()
        {
            if (GUI.Button(
                new Rect(Screen.width - TABLE_WIDTH - OFFSET - TABLE_HEIGHT, Screen.height - TABLE_HEIGHT - OFFSET, TABLE_WIDTH + TABLE_HEIGHT,
                    TABLE_HEIGHT), "Create character"))
            {
                GameSettings.GameState = EGameState.NewGame;
                gameSettings.GetComponent<GameSettings>().SaveCharacter();
                SceneManager.LoadScene("Game");
            }
        }

        private void SelectCharacter()
        {
            GUI.Box(new Rect(OFFSET, Screen.height - TABLE_WIDTH - OFFSET, TABLE_WIDTH, TABLE_WIDTH), "");
            if (GUI.Button(new Rect(new Rect(OFFSET, Screen.height - TABLE_HEIGHT * 4 - OFFSET, TABLE_WIDTH / 2, TABLE_WIDTH / 2)),
                ECharacter.Human.ToString(), Human))
            {
                Instance(ECharacter.Human);
            }
            if (
                GUI.Button(
                    new Rect(new Rect(OFFSET + TABLE_WIDTH / 2, Screen.height - TABLE_HEIGHT * 4 - OFFSET, TABLE_WIDTH / 2, TABLE_WIDTH / 2)),
                    ECharacter.Ork.ToString(), Ork))
            {
                Instance(ECharacter.Ork);
            }
            if (GUI.Button(new Rect(new Rect(OFFSET, Screen.height - TABLE_HEIGHT * 2 - OFFSET, TABLE_WIDTH / 2, TABLE_WIDTH / 2)),
                ECharacter.Elf.ToString(), Elf))
            {
                StatsOperating(ECharacter.Elf);
            }
            if (
                GUI.Button(
                    new Rect(new Rect(OFFSET + TABLE_WIDTH / 2, Screen.height - TABLE_HEIGHT * 2 - OFFSET, TABLE_WIDTH / 2,
                        TABLE_WIDTH / 2)), ECharacter.Bull.ToString(), Bull))
            {
                StatsOperating(ECharacter.Bull);
            }
        }

        private void Instance(ECharacter eCharacter)
        {
            if (Prefab.name != eCharacter.ToString() || Player == null)
            {              
                string savedName = "";  //nerestartuje name field
                if (Player != null)
                {
                    savedName = _playerComponent.Name;
                    Destroy(Player);
                }
                Prefab = Resources.Load<GameObject>("Prefab/" + eCharacter);
                Prefab.name = eCharacter.ToString();
                Vector3 pos = new Vector3(0, Terrain.activeTerrain.SampleHeight(Vector3.zero), 0);
                Player = (GameObject)Instantiate(Prefab, pos, Quaternion.identity);
                Player.GetComponent<CharacterController>().enabled = false;
                Player.GetComponent<ThirdPersonCamera>().enabled = false;
                _playerComponent = Player.GetComponent<PlayerComponent>();
                _playerComponent.Prefab = Prefab;
                _playerComponent.Name = savedName;   //nastavení jména zpět
                StatsOperating(eCharacter);
            }
        }

        private void StatsOperating(ECharacter eCharacter)
        {
            _playerComponent.character.ECharacter = eCharacter;
            _playerComponent.LevelUp();
            _playerComponent.character.IncrementSkills(true);
            _playerComponent.character.UpdateStats();
        }
        public void LoadSettings()
        {
            GameObject gs = GameObject.Find("GameSettings");
            if (gs == null)
            {
                GameObject gs1 = Instantiate(gameSettings, Vector3.zero, Quaternion.identity);
                gs1.name = "GameSettings";
            }
            GameObject.Find("GameSettings").GetComponent<GameSettings>();
        }
    }
}
