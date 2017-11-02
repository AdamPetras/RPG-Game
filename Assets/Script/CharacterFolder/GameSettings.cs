using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Script.Camera;
using Assets.Script.Extension;
using Assets.Script.HUD;
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
        void Awake()
        {

        }

        // Use this for initialization
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void DestroyAllGameObjects()
        {
            GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

            for (int i = 0; i < GameObjects.Length; i++)
            {
                Destroy(GameObjects[i]);
            }
        }

        public void SaveCharacter(string save = "Default")
        {
            GameObject Player = GameObject.FindGameObjectWithTag("Player");
            PlayerComponent playerComponent = Player.GetComponent<PlayerComponent>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + save + ".save");
            PlayerData data = new PlayerData();
            data.PrefabName = playerComponent.Prefab.name;
            data.Name = playerComponent.Name;
            data.ExpCurrent = playerComponent.ExpCurrent;
            data.ExpMultiplier = playerComponent.ExpMultiplier;
            data.ExpToNextLevel = playerComponent.ExpToNextLevel;
            data.Level = playerComponent.Level;
            data.Money = playerComponent.Money;
            data.ProfessionList = playerComponent.ProfessionList;
            data.SkillArray = playerComponent.character.skillArray;
            data.VitalArray = playerComponent.character.vitalArray;
            data.DamageStatsArray = playerComponent.character.damageStatsArray;
            data.Time = DateTime.Now;
            data.PlayerPosition = new SerializableVector3(Player.transform.position);
            //data.PlayerRotation = new SerializableQuaternion(Player.transform.rotation);
            if (!SaveQuest(data, playerComponent) || !SaveInventory(data) || !SaveCamera(data))
            {
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
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    if (GameState == EGameState.InGameLoad)
                    {
                        player.transform.position = data.PlayerPosition.GetVector3();
                        //player.transform.rotation = data.PlayerRotation.GetQuaternion();
                    }
                    PlayerComponent playerComponent = player.GetComponent<PlayerComponent>();
                    playerComponent.Name = data.Name;
                    playerComponent.name = data.Name;
                    playerComponent.AddExp(data.ExpCurrent, true);
                    playerComponent.ExpMultiplier = data.ExpMultiplier;
                    playerComponent.ExpToNextLevel = data.ExpToNextLevel;
                    playerComponent.SavedPosition = data.PlayerPosition.GetVector3();
                    //playerComponent.SavedRotation = data.PlayerRotation.GetQuaternion();
                    playerComponent.Level = data.Level;
                    playerComponent.Money = data.Money;
                    playerComponent.ProfessionList = data.ProfessionList;
                    playerComponent.character.skillArray = data.SkillArray;
                    playerComponent.character.damageStatsArray = data.DamageStatsArray;
                    playerComponent.character.vitalArray = data.VitalArray;
                    playerComponent.QuestList.Clear();
                    LoadQuest(data, playerComponent);
                    LoadInventory(data);
                    LoadCamera(data);
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
            if (GameObject.Find("GameMaster") != null)
                foreach (ComponentItem item in GameObject.Find("GameMaster").GetComponent<ComponentInventory>()
                    .InventoryList.Where(s => s != null))
                {
                    data.InventoryList.Add(new ItemSave(item.ID, item.ActualStack, false));
                }
            else return false;
            if (GameObject.FindGameObjectWithTag("Player") != null)
                foreach (ComponentItem item in GameObject.FindGameObjectWithTag("Player")
                    .GetComponent<PlayerComponent>().ArmorList.Where(s => s != null))
                {
                    data.InventoryList.Add(new ItemSave(item.ID, item.ActualStack, true));
                }
            else return false;
            return true;
        }

        private void LoadInventory(PlayerData data)
        {
            if (GameObject.Find("GameMaster") != null)
                SlotManagement.ClearList(GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList);
            if (GameObject.FindGameObjectWithTag("Player") != null)
                SlotManagement.ClearList(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList);
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
                foreach (ObjectGenerate questMaster in QuestMasterGenerate.QuestMasterList)
                {
                    if (GameObject.FindGameObjectsWithTag("QuestMaster").Length > 0)
                        if (GameObject.FindGameObjectsWithTag("QuestMaster").First(s => s.name == questMaster.Name) != null)
                            foreach (ModifyQuest quest in GameObject.FindGameObjectsWithTag("QuestMaster")
                                .First(s => s.name == questMaster.Name).GetComponent<QuestMasterObject>().QuestMaster
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
            if (GameState == EGameState.MenuLoad)
                if (GameObject.Find("Quest") != null)
                {
                    GameObject.Find("Quest").GetComponent<QuestObject>().QuestList.Clear();
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
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("QuestMaster");
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.GetComponent<QuestMasterObject>().QuestMaster.QuestList.Clear();
            }
            foreach (QuestSave quest in data.UnAssignedQuestList/*.Where(s => !data.CollectedQuestList.Exists(a => a.Id == s.Id))*/)
            {
                if (GameObject.Find("Quest") != null)
                {
                    GameObject.Find("Quest").GetComponent<QuestObject>().QuestList.Add(new ModifyQuest(Quest.IdToQuest(quest.Id)));
                }
            }
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.GetComponent<QuestMasterObject>().AddQuestToQuestMaster();
            }
        }
    }
}
