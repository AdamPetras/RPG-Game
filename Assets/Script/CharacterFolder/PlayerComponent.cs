using System.Collections.Generic;
using Assets.Script.Camera;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.InventoryFolder;
using Assets.Script.Menu;
using Assets.Script.QuestFolder;
using Assets.Script.SpellFolder;
using Assets.Script.StatisticsFolder;
using Assets.Script.TargetFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.UI;

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
        public long Money { get; set; }
        public Vector3 SavedPosition { get; set; }
        public Quaternion SavedRotation { get; set; }
        public ESubtype Weapon { private get; set; }
        public Character character;
        public List<ModifyQuest> QuestList;
        public List<Profession> ProfessionList;
        public List<ComponentItem> ArmorList;        
        private bool _sendFirstBroadcast;
        public static bool ExperiencesPooling;
        public GameObject Prefab;
        
        private ThirdPersonCamera _tps;
        private bool _energyRegen;
        private void Awake()
        {
            ArmorList = new List<ComponentItem>();
            character = new Character();          
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
            ExpMultiplier = 1.2f;
            Level = 0;
            Created = false;
            _sendFirstBroadcast = false;
            character.BuffObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/BuffPanel"));
            character.BuffObj.name = "BuffPanel";
            character.BuffPanel = character.BuffObj.transform.Find("Background");
            _tps = GetComponent<ThirdPersonCamera>();
            _energyRegen = true;
        }

        private void Start()
        {
            Name = name;
            gameObject.transform.Find("PlayerTargetPrefab").Find("Background").Find("Name").GetComponent<Text>().text = Name;
            LevelUp();
            BlackScreen.EndPrint();
        }

        public void LevelUp()
        {
            while (ExpCurrent >= ExpToNextLevel)
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

        public void RegeneratingEnergy()
        {
            if (character.GetVital((int) EVital.Energy).CurrentValue < character.GetVital((int) EVital.Energy).MaxValue)
                character.GetVital((int) EVital.Energy).CurrentValue ++;
            if(character.GetVital((int)EVital.Energy).CurrentValue > character.GetVital((int)EVital.Energy).MaxValue)
            {
                character.GetVital((int) EVital.Energy).CurrentValue = character.GetVital((int) EVital.Energy).MaxValue;
                CancelInvoke("RegeneratingEnergy");
            }
            _tps.canIRun = true;
        }


        public void Run()
        {
            character.GetVital((int)EVital.Energy).CurrentValue -= 0.1f;
            if (character.GetVital((int)EVital.Energy).CurrentValue <= 0)
            {
                character.GetVital((int)EVital.Energy).CurrentValue = 0;
                _tps.canIRun = false;
            }
        }

        void Update()
        {
            character.HealthRegen();
            if (_tps.isWalking == false && _tps.swimming == false&& Created)
            {
                _energyRegen = true;
                Run();
                CancelInvoke("RegeneratingEnergy");
            }
            else if(_energyRegen)
            {
                _energyRegen = false;
                InvokeRepeating("RegeneratingEnergy",5,0.5f);
            }
            if (character.ECharacterState == ECharacterState.Dead)
            {
                MainMenu menu = GameObject.Find("Graphics").transform.Find("Menu").GetComponent<MainMenu>();
                menu.OnVisible();
                menu.CouldBeExited = false;
                Debug.Log("Dead");
                character.GetVital((int)EVital.Health).CurrentValue = 0;
                GetComponent<ThirdPersonCamera>().enabled = false;
                GetComponent<AttackAI>().enabled = false;
                GetComponent<CharacterController>().enabled = false;
                UnityEngine.Camera.main.GetComponent<MyCamera>().enabled = false;
                GetComponent<PlayerComponent>().enabled = false;
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

        public EDamageStats SelectWhichWeapon()
        {
            switch (Weapon)
            {
                    case ESubtype.Axe:
                    return EDamageStats.AxeDamage;
                    case ESubtype.Bow: return  EDamageStats.RangedPower;
                    case ESubtype.Knife: return  EDamageStats.SwordDamage;
                    case ESubtype.Sword: return  EDamageStats.SwordDamage;
                    case ESubtype.Spear: return EDamageStats.MaceDamage;
                    case ESubtype.None: return  EDamageStats.SwordDamage;
            }
            return EDamageStats.SwordDamage;
        }
    }
}
