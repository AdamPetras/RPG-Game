using Assets.Script.Camera;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.QuestFolder;
using Assets.Script.TargetFolder;
using UnityEngine;

namespace Assets.Script
{
    public class GameMaster : MonoBehaviour
    {
        public GameObject PlayerCharacter;
        public GameObject gameSettings;

        private GameObject _playerCharacter;
        private PlayerComponent _playerComponentScript;

        private Vector3 _spawnPosition;

        void Awake()
        {
            if (GameSettings.GameState != EGameState.MenuLoad)
            {
                _spawnPosition = new Vector3(24, 2f, 27);
                _playerCharacter = ObjectGenerate.CharacterInstantiate(PlayerCharacter = Resources.Load<GameObject>("Prefab/Human"), _spawnPosition);
            }
            else
                _playerCharacter = ObjectGenerate.CharacterInstantiate(Resources.Load<GameObject>("Prefab/Human"),Vector3.zero);          
            LoadSettings();
            LoadDebug();
            LoadBlackScreen();
            _playerComponentScript = _playerCharacter.GetComponent<PlayerComponent>();           
            _playerComponentScript.Prefab = Resources.Load<GameObject>("Prefab/Human");
            _playerCharacter.GetComponent<CharacterController>().enabled = true;
            _playerCharacter.GetComponent<ThirdPersonCamera>().enabled = true;
            _playerCharacter.GetComponent<Animator>().enabled = true;
            _playerComponentScript.Created = true;
        }


        // Use this for initialization
        void Start()
        {
            if (GameSettings.GameState == EGameState.MenuLoad)
            {
                gameSettings.GetComponent<GameSettings>().LoadCharacter(GameSettings.SavePosition);
                _playerCharacter.transform.position = _playerComponentScript.SavedPosition;
                //_playerCharacter.transform.rotation = _playerComponentScript.SavedRotation;
            }
            else
            {
                gameSettings.GetComponent<GameSettings>().LoadCharacter();
                _playerComponentScript = _playerCharacter.GetComponent<PlayerComponent>();               
            }
            _playerCharacter.GetComponent<Target>().TargetPrefab.GetComponent<Canvas>().enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
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
        public void LoadDebug()
        {
            GameObject gs = GameObject.Find("Debug");
            if (gs == null)
            {
                GameObject debug = Instantiate(Resources.Load<GameObject>("Prefab/Debug"));
                debug.name = "Debug";
            }
        }
        public void LoadBlackScreen()
        {
            GameObject gs = GameObject.Find("BlackScreen");
            if (gs == null)
            {
                GameObject debug = Instantiate(Resources.Load<GameObject>("Prefab/BlackScreen"));
                debug.name = "BlackScreen";
            }
        }
    }
}
