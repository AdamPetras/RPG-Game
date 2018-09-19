using System;
using Assets.Script.CharacterFolder;
using Assets.Script.InventoryFolder;
using Assets.Script.QuestFolder;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.Enemy
{
    [Serializable]
    public struct DropItem
    {
        public int ItemID;
        
        public Quantity Quantity;
        public float Chance;
        
        public DropItem(int itemID, Quantity quantity, float chance)
        {
            ItemID = itemID;
            Quantity = quantity;
            Chance = chance;
        }
    }
    [Serializable]
    public struct DropMoney
    {
        public Quantity Quantity;
        public float Chance;

        public DropMoney(Quantity quantity, float chance)
        {
            Quantity = quantity;
            Chance = chance;
        }
    }
    [Serializable]
    public struct Quantity
    {
        public int QuantityFrom;
        public int QuantityTo;

        public Quantity(int quantityFrom, int quantityTo)
        {
            QuantityFrom = quantityFrom;
            QuantityTo = quantityTo;
        }
    }

    public class EnemyCharacter : Character, IBasicAtribute
    {
        private int MinLevel;
        public float ExpMultiplier { get; set; }
        public int Level { get; set; }
        public int ExperiencesForKill { get; private set; }
        public string Name { get; set; }
        public bool Angry { get; set; }
        private int _id;
        private delegate void EnemyDie();
        private event EnemyDie EventEnemyDie;
        private bool done;
        private bool questAdded;
        private float corpseTimer;
        private bool _dropExist;
        public readonly DropItem[] DropList;
        public readonly DropMoney MoneyDrop;
        private readonly int _respawnTime;
        private readonly Transform _enemyTransform;
        public EnemyCharacter(int id, BoxCollider collider, Transform enemyTransform, DropItem[] dropList, int respawnTime, DropMoney moneyDrop)
        {
            _id = id;
            ExperiencesForKill = 40;
            ExpMultiplier = 1.2f;
            MinLevel = 1;
            Name = "Enemy";
            _enemyTransform = enemyTransform;
            EventEnemyDie += AddExperiences;
            EventEnemyDie += CorpseVanishTimer;
            EventEnemyDie = AddQuestProgress;
            EventEnemyDie += AddDrop;
            _dropExist = false;
            questAdded = false;
            done = false;
            this.DropList = dropList;
            _respawnTime = respawnTime;
            MoneyDrop = moneyDrop;
            
        }

        public void Update()
        {
            HealthRegen();
        }

        public void CalculateStats()
        {
            for (; MinLevel <= Level; MinLevel++)
            {
                CalculateExperiences();
                GetSkill((int)ESkill.Attack).BasicValue += 1;
                GetSkill((int)ESkill.Constitution).BasicValue += 2;
                GetSkill((int)ESkill.Intelect).BasicValue += 2;
                GetSkill((int)ESkill.Defence).BasicValue += 2;
                GetSkill((int)ESkill.Regenerate).BasicValue = 1;
                GetSkill((int)ESkill.Dexterity).BasicValue = 2;
                if (MinLevel == Level)
                    UpdateStats();
            }
        }

        private void AddDrop()
        {
            if (!_dropExist)
            {
                GameObject dropObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DropPrefab"));
                dropObj.name = "DropPrefab";
                dropObj.transform.SetParent(_enemyTransform);
                _dropExist = true;
            }
        }

        private void AddExperiences()
        {
            if (!done)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().AddExp(ExperiencesForKill);               
                done = true;
            }
        }

        private void AddQuestProgress()
        {
            if (questAdded)
                return;
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                foreach (ModifyQuest quest in GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().QuestList)
                {
                    if (quest.EnemyId == _id)
                    {
                        Debug.Log("Add");
                        quest.CurrentKills++;
                        if (quest.CurrentKills >= quest.TotalKills)
                        {
                            quest.EQuestState = EQuestState.Complete;
                        }
                        questAdded = true;
                    }
                }      
            }
        }

        private void CorpseVanishTimer()
        {         
            corpseTimer += Time.deltaTime;
            if (corpseTimer >= _respawnTime)
            {
                Drop dr = _enemyTransform.Find("DropPrefab").GetComponent<Drop>();
                if(dr != null)
                if (dr.DropItemList.Count == 0)
                {
                    if (!Drop.Visible || (Drop.Visible && corpseTimer >= 2 * _respawnTime))
                    {
                        dr.DropItemList.Clear();
                        GameObject.Destroy(dr);
                        ECharacterState = ECharacterState.Delete;                       
                    }
                }
            }
        }


        protected override void SetupDamageStats()
        {
            ModifySkill sword = new ModifySkill(GetSkill((int)ESkill.Attack), 1.1f);
            GetDamageStats((int)EDamageStats.SwordDamage).AddSkill(sword);
            ModifySkill attackSpeed = new ModifySkill(GetSkill((int)ESkill.Dexterity), 1);
            GetDamageStats((int)EDamageStats.AttackSpeed).AddSkill(attackSpeed);
            ModifySkill def = new ModifySkill(GetSkill((int)ESkill.Defence), 0.05f);
            GetDamageStats((int)EDamageStats.DamageBlock).AddSkill(def);
            ModifySkill healthRegen = new ModifySkill(GetSkill((int)ESkill.Regenerate), 1f);
            GetDamageStats((int)EDamageStats.HealthRegen).AddSkill(healthRegen);
        }

        protected override void SetupVital()
        {
            //životy
            ModifySkill health = new ModifySkill(GetSkill((int)ESkill.Constitution), 6);
            GetVital((int)EVital.Health).AddSkill(health);
            ModifySkill mana = new ModifySkill(GetSkill((int)ESkill.Intelect), 3);
            GetVital((int)EVital.Mana).AddSkill(mana);
        }

        private void CalculateExperiences()
        {
            ExperiencesForKill = (int)(ExperiencesForKill * ExpMultiplier);
        }

        public void OnEventEnemyDie()
        {
            if (EventEnemyDie != null) EventEnemyDie.Invoke();
        }
    }
}