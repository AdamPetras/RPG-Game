using System.Collections.Generic;
using Assets.Script.Camera;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.InventoryFolder;
using Assets.Script.QuestFolder;
using Assets.Script.SpellFolder;
using Assets.Script.StatisticsFolder;
using Assets.Script.TargetFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Script.CharacterFolder
{
    public enum ECharacter
    {
        Human,
        Ork,
        Elf,
        Bull
    }

    public class PlayerComponent : MonoBehaviour, IBasicAtribute
    {
        public int ExpCurrent { get; private set; }
        public int ExpToNextLevel { get; set; }
        public int TotalExp { get; set; }
        public float ExpMultiplier { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public bool Created { get; set; }
        public uint Money { get; set; }
        public Vector3 SavedPosition { get; set; }
        public Quaternion SavedRotation { get; set; }
        public Character character;
        public List<ModifyQuest> QuestList;
        public List<Profession> ProfessionList;
        public List<ComponentItem> ArmorList;
        public List<Spell> SpellList;
        private bool _sendFirstBroadcast;
        public static bool ExperiencesPooling;
        public GameObject Prefab;
        private GameObject _buffObj;
        public Transform BuffPanel;
        private void Awake()
        {
            ArmorList = new List<ComponentItem>();
            character = new Character();
            SpellList = new List<Spell>();
            QuestList = new List<ModifyQuest>();
            ProfessionList = new List<Profession>();
            ProfessionList.Add(new Profession(EProfession.Cooking));
            ProfessionList.Add(new Profession(EProfession.Crafting));
            ProfessionList.Add(new Profession(EProfession.Tailoring));
            ProfessionList.Add(new Profession(EProfession.Smithing));
            ProfessionList.Add(new Profession(EProfession.Fishing));
            ExpCurrent = 200;
            ExpToNextLevel = 200;
            TotalExp = 0;
            Money = 0;
            ExpMultiplier = 1.2f;
            Level = 0;
            Created = false;
            _sendFirstBroadcast = false;
            _buffObj = Instantiate(Resources.Load<GameObject>("Prefab/BuffPanel"));
            _buffObj.name = "BuffPanel";
            BuffPanel = _buffObj.transform.Find("Background");
        }

        private void Start()
        {
            Name = name;
            LevelUp();
        }

        public void LevelUp()
        {
            if (ExpCurrent >= ExpToNextLevel)
            {
                TotalExp += ExpCurrent;         //přičtení total exp
                ExpCurrent -= ExpToNextLevel;   //přetečení expů
                ExpToNextLevel = (int)CalculateExpToNextLevel();
                character.IncrementSkills();
                character.UpdateStats();
                Level++;
            }
        }

        private uint CalculateExpToNextLevel()
        {
            return (uint)(ExpToNextLevel * ExpMultiplier);
        }

        void Update()
        {
            character.HealthRegen();
            if (character.ECharacterState == ECharacterState.Dead)
            {
                Debug.Log("Dead");
                character.GetVital((int)EVital.Health).CurrentValue = 0;
                GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonCamera>().enabled = false;
                GameObject.FindGameObjectWithTag("Player").GetComponent<AttackAI>().enabled = false;
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MyCamera>().enabled = false;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().enabled = false;
            }
        }

        public Profession SelectProfession(EProfession eProfession)
        {
            return ProfessionList.Find(s => s.EProfession == eProfession);
        }

        public void AddExp(int exp, bool isload = false)
        {
            if (isload)
                ExpCurrent = 0;
            ExpCurrent += exp;
            LevelUp();
            ExperiencesPooling = true;
        }
    }
}
