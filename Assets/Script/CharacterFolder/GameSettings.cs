using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Script.Camera;
using Assets.Script.CombatFolder;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.Interaction;
using Assets.Script.InventoryFolder;
using Assets.Script.QuestFolder;
using Assets.Script.StatisticsFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.CharacterFolder
{
    [Serializable]
    public class ItemSave
    {
        public int Id;
        public int ActualStack;
        public bool Wearing;
        public ItemSave(int id, int actualStack, bool wearing)
        {
            Id = id;
            ActualStack = actualStack;
            Wearing = wearing;
        }
    }

    [Serializable]
    public class QuestSave
    {
        public int Id;
        public int QuestMasterID;
        public byte CurrentKills;
        public List<int> QuestMasterTalkedList;
        public List<int> QuestMasterList;
        public EQuestState EQuestState;

        public QuestSave(int id, int questMasterID, byte currentKills, List<int> questMasterTalkedList, List<int> questMasterList, EQuestState eQuestState)
        {
            Id = id;
            CurrentKills = currentKills;
            QuestMasterID = questMasterID;
            QuestMasterTalkedList = questMasterTalkedList;
            QuestMasterList = questMasterList;
            EQuestState = eQuestState;
        }
    }
    [Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public SerializableVector3(Vector3 position)
        {
            X = position.x;
            Y = position.y;
            Z = position.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
    [Serializable]
    public class SerializableQuaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        public SerializableQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public SerializableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }
    }

    public enum EGameState
    {
        None,
        NewGame,
        MenuLoad,
        InGameLoad,
    }

    [Serializable]
    public class PlayerData
    {
        public int ExpCurrent;
        public int ExpToNextLevel;
        public float ExpMultiplier;
        public int Level;
        public string Name;
        public uint Money;
        public List<QuestSave> CollectedQuestList;
        public List<QuestSave> UnAssignedQuestList;
        public List<Profession> ProfessionList;
        public List<ItemSave> InventoryList;
        public Skill[] SkillArray;
        public DamageStats[] DamageStatsArray;
        public Vital[] VitalArray;
        public DateTime Time;
        public SerializableVector3 PlayerPosition;
        public SerializableQuaternion PlayerRotation;
        public SerializableVector3 CameraPosition;
        public SerializableQuaternion CameraRotation;
        public string PrefabName;
        public InGameTime.MyTime InGameTime;

        public PlayerData()
        {
            UnAssignedQuestList = new List<QuestSave>();
            CollectedQuestList = new List<QuestSave>();
            InventoryList = new List<ItemSave>();
        }
    }

    public class GameSettings : MonoBehaviour
    {
        public static EGameState GameState;
        public static string SavePosition;
        public const string PLAYERSPAWN = "PlayerSpawn";
        private PlayerComponent _playerComponent;
        private GameObject _playerGameObject;
        private GameObject _gameMaster;
        private InGameTime _inGameTime;
        private ComponentInventory _componentInventory;
        private GameObject _questGameObject;
        void Awake()
        {

        }

        // Use this for initialization
        private void Start()
        {
            DontDestroyOnLoad(this);
            LoadInstances();
        }

        public void DestroyAllGameObjects()
        {
            GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

            for (int i = 0; i < GameObjects.Length; i++)
            {
                Destroy(GameObjects[i]);
            }
        }

        private void LoadInstances()
        {
            if (_gameMaster == null)
            {
                _gameMaster = GameObject.Find("GameMaster");
            }
            if (_componentInventory == null)
            {
                if (_gameMaster != null)
                    _componentInventory = _gameMaster.GetComponent<ComponentInventory>();
            }
            if (_playerGameObject == null)
            {
                _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            }
            if (_playerComponent == null)
            {
                if (_playerGameObject != null)
                    _playerComponent = _playerGameObject.GetComponent<PlayerComponent>();
            }
            if (_questGameObject == null)
            {
                _questGameObject = GameObject.Find("Quest");
            }
            if (_inGameTime == null)
            {
                if(GameObject.Find("Graphics") != null)
                    _inGameTime = GameObject.Find("Graphics").GetComponentInChildren<InGameTime>();
            }
        }

        public void SaveCharacter(string save = "Default")
        {
            LoadInstances();
            if (_playerGameObject == null)
            {
                _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            }
            if (_playerComponent == null)
            {
                if (_playerGameObject != null)
                    _playerComponent = _playerGameObject.GetComponent<PlayerComponent>();
            }
            if (_playerComponent == null)
            {
                MyDebug.LogError("I cant save this game");
                return;
            }
            Debug.Log(_playerComponent);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + save + ".save");
            PlayerData data = new PlayerData();
            data.PrefabName = _playerComponent.Prefab.name;
            data.Name = _playerComponent.Name;
            data.ExpCurrent = _playerComponent.ExpCurrent;
            data.ExpMultiplier = _playerComponent.ExpMultiplier;
            data.ExpToNextLevel = _playerComponent.ExpToNextLevel;
            data.Level = _playerComponent.Level;
            data.ProfessionList = _playerComponent.ProfessionList;
            data.SkillArray = _playerComponent.character.skillArray;
            data.VitalArray = _playerComponent.character.vitalArray;
            data.DamageStatsArray = _playerComponent.character.damageStatsArray;
            data.Time = DateTime.Now;
            if(_inGameTime != null)
                data.InGameTime = _inGameTime.GetActualTime();
            data.PlayerPosition = new SerializableVector3(_playerGameObject.transform.position);
            //data.PlayerRotation = new SerializableQuaternion(Player.transform.rotation);
            if (!SaveQuest(data, _playerComponent) || !SaveInventory(data) || !SaveCamera(data))
            {
                if (save == "Default")
                {
                    bf.Serialize(file, data);
                    file.Close();
                    return;
                }
                MyDebug.LogError("I cant save this game");
                file.Close();
                return;
            }
            bf.Serialize(file, data);
            file.Close();
        }

        public static DateTime? IsSaveExist(string saveFileName)
        {
            if (File.Exists(Application.persistentDataPath + "/" + saveFileName + ".save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + saveFileName + ".save", FileMode.Open);
                PlayerData data;
                try
                {
                    data = (PlayerData)bf.Deserialize(file);
                }
                catch (SerializationException e)
                {
                    MyDebug.LogError("The save game saved with error");
                    return null;
                }
                file.Close();
                return data.Time;
            }
            return null;
        }

        public void LoadCharacter(string save = "Default")
        {
            if (File.Exists(Application.persistentDataPath + "/" + save + ".save"))
            {
                LoadInstances();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + save + ".save", FileMode.Open);
                PlayerData data;
                try
                {
                    data = (PlayerData)bf.Deserialize(file);
                }
                catch (SerializationException e)
                {
                    MyDebug.LogError("The save game saved with error");
                    return;
                }
                file.Close();
                if (_playerGameObject == null)
                {
                    _playerGameObject = GameObject.FindGameObjectWithTag("Player");
                }
                if (_playerComponent == null)
                {
                    if (_playerGameObject != null)
                        _playerComponent = _playerGameObject.GetComponent<PlayerComponent>();
                }
                if (_playerGameObject != null)
                {
                    if (_playerComponent.character.ECharacterState == ECharacterState.Dead)
                    {
                        _playerComponent.character.ECharacterState = ECharacterState.Alive;
                        _playerGameObject.GetComponent<ThirdPersonCamera>().enabled = true;
                        _playerGameObject.GetComponent<AttackAI>().enabled = true;
                        _playerGameObject.GetComponent<CharacterController>().enabled = true;
                        UnityEngine.Camera.main.GetComponent<MyCamera>().enabled = true;
                        _playerGameObject.GetComponent<PlayerComponent>().enabled = true;
                    }
                    if (GameState == EGameState.InGameLoad)
                    {
                        _playerGameObject.transform.position = data.PlayerPosition.GetVector3();
                        //player.transform.rotation = data.PlayerRotation.GetQuaternion();
                    }
                    _playerComponent.Name = data.Name;
                    _playerComponent.name = data.Name;
                    _playerComponent.AddExp(data.ExpCurrent, true);
                    _playerComponent.ExpMultiplier = data.ExpMultiplier;
                    _playerComponent.ExpToNextLevel = data.ExpToNextLevel;
                    _playerComponent.SavedPosition = data.PlayerPosition.GetVector3();
                    //playerComponent.SavedRotation = data.PlayerRotation.GetQuaternion();
                    _playerComponent.Level = data.Level;
                    _playerComponent.ProfessionList = data.ProfessionList;
                    _playerComponent.character.skillArray = data.SkillArray;
                    _playerComponent.character.damageStatsArray = data.DamageStatsArray;
                    _playerComponent.character.vitalArray = data.VitalArray;
                    _playerComponent.QuestList.Clear();
                    LoadQuest(data, _playerComponent);
                    LoadInventory(data);
                    LoadCamera(data);
                    LoadTime(data);
                }
                else
                {
                    Debug.LogWarning("#001 Player gamebject is null [GameSettings]");
                    MyDebug.LogWarning("#001 Player gamebject is null [GameSettings]");
                }
            }
        }

        private bool SaveCamera(PlayerData data)
        {
            /* data.CameraRotation = new SerializableQuaternion(UnityEngine.Camera.main.transform.rotation);
             data.CameraPosition = new SerializableVector3(UnityEngine.Camera.main.transform.position);*/
            return true;
        }

        private void LoadCamera(PlayerData data)
        {
            //UnityEngine.Camera.main.GetComponent<MyCamera>().enabled = false;
            /*
            UnityEngine.Camera.main.transform.rotation = data.CameraRotation.GetQuaternion();
            UnityEngine.Camera.main.transform.position = data.CameraPosition.GetVector3();
            Debug.Log(UnityEngine.Camera.main.transform.position);
            Debug.Log(UnityEngine.Camera.main.transform.rotation);*/
            //UnityEngine.Camera.main.GetComponent<MyCamera>().enabled = true;
        }

        private bool SaveInventory(PlayerData data)
        {
            if (_gameMaster != null)
            {
                foreach (ComponentItem item in _componentInventory.InventoryList.Where(s => s != null))
                {
                    data.InventoryList.Add(new ItemSave(item.ID, item.ActualStack, false));
                }
            } 
            else return false;
            if (_playerGameObject != null)
            {
                foreach (ComponentItem item in _playerComponent.ArmorList.Where(s => s != null))
                {
                    data.InventoryList.Add(new ItemSave(item.ID, item.ActualStack, true));
                }
            }
            else return false;
            return true;
        }

        private void LoadInventory(PlayerData data)
        {
            if (_gameMaster != null)
                SlotManagement.ClearList(_componentInventory.InventoryList);
            if (_playerComponent != null)
                SlotManagement.ClearList(_playerComponent.ArmorList);
            foreach (ItemSave item in data.InventoryList)
            {
                Debug.Log(item.Id);
                NewItem newItem = new NewItem(NewItem.IdToItem(item.Id));
                newItem.ActualStack = item.ActualStack;
                if (!item.Wearing)
                    SlotManagement.AddToInventory(newItem);
                else
                {
                    SlotManagement.AddToArmor(newItem);
                }
            }
        }

        private bool SaveQuest(PlayerData data, PlayerComponent playerComponent)
        {
            if (playerComponent.QuestList != null)
                foreach (ModifyQuest quest in playerComponent.QuestList)
                {
                    data.CollectedQuestList.Add(new QuestSave(quest.QuestID, quest.QuestMasterAsign, quest.CurrentKills, quest.QuestMasterTalkedList, quest.QuestMasterList, quest.EQuestState));
                }
            else return false;
            if (QuestMasterGenerate.QuestMasterList != null)
                foreach (QuestMasterObject questMaster in QuestMasterGenerate.QuestMasterList)
                {
                    if (GameObject.FindGameObjectsWithTag("QuestMaster").Length > 0)
                        if (GameObject.FindGameObjectsWithTag("QuestMaster").First(s => s.name == questMaster.Name) != null)
                            foreach (ModifyQuest quest in GameObject.FindGameObjectsWithTag("QuestMaster")
                                .First(s => s.name == questMaster.Name).GetComponent<QuestMasterObject>().ThisQuestMaster
                                .QuestList)
                            {
                                data.UnAssignedQuestList.Add(new QuestSave(quest.QuestID, quest.QuestMasterAsign,
                                    quest.CurrentKills, quest.QuestMasterTalkedList, quest.QuestMasterList,
                                    quest.EQuestState));
                            }
                        else return false;
                }
            else return false;
            return true;
        }

        private void LoadQuest(PlayerData data, PlayerComponent playerComponent)
        {
            if (GameState == EGameState.InGameLoad || GameState == EGameState.MenuLoad)
            {
                foreach (QuestMasterObject obj in QuestMasterGenerate.QuestMasterList)
                {
                    obj.ThisQuestMaster.QuestList.Clear();
                }
                if (_questGameObject != null)
                {
                    _questGameObject.GetComponent<QuestObject>().QuestList.Clear();
                }
            }
            foreach (QuestSave quest in data.CollectedQuestList)
            {
                ModifyQuest q = new ModifyQuest(Quest.IdToQuest(quest.Id));
                q.CurrentKills = quest.CurrentKills;
                q.EQuestState = quest.EQuestState;
                q.QuestMasterTalkedList = quest.QuestMasterTalkedList;
                q.QuestMasterList = quest.QuestMasterList;
                playerComponent.QuestList.Add(q);
                HUDQuestList.AddQuest(q);
                if (data.UnAssignedQuestList.Any(s => s.Id == q.QuestID))
                {
                    data.UnAssignedQuestList.RemoveAll(s => s.Id == q.QuestID);
                }
            }
            foreach (QuestSave quest in data.UnAssignedQuestList)
            {
                if (_questGameObject != null)
                {
                    _questGameObject.GetComponent<QuestObject>().QuestList.Add(new ModifyQuest(Quest.IdToQuest(quest.Id)));
                }
            }
            foreach (QuestMasterObject obj in QuestMasterGenerate.QuestMasterList)
            {               
                obj.AddQuestToQuestMaster();
            }
        }

        private void LoadTime(PlayerData data)
        {
            if(data.InGameTime != null)
                _inGameTime.SetActualTime(data.InGameTime);
        }

    }
}
