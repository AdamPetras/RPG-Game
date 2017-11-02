using System;
using Assets.Script.CharacterFolder;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.Enemy
{
    [Serializable]
    public struct DropItem
    {
        public int ItemID;
        public int Quantity;
        public float Chance;

        public DropItem(int itemID, int quantity, float chance)
        {
            ItemID = itemID;
            Quantity = quantity;
            Chance = chance;
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
        private delegate void EnemyDie();
        private event EnemyDie EventEnemyDie;
        private bool done;
        private float corpseTimer;
        private bool _dropExist;
        public readonly DropItem[] _dropList;
        private readonly int _respawnTime;
        private readonly Transform _enemyTransform;
        public EnemyCharacter(BoxCollider collider, Transform enemyTransform, DropItem[] dropList, int respawnTime)
        {
            ExperiencesForKill = 40;
            ExpMultiplier = 1.2f;
            MinLevel = 1;
            Name = "Enemy";
            _enemyTransform = enemyTransform;
            EventEnemyDie += AddExperiences;
            EventEnemyDie += CorpseVanishTimer;
            EventEnemyDie += AddDrop;
            _dropExist = false;
            this._dropList = dropList;
            _respawnTime = respawnTime;
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

        private void CorpseVanishTimer()
        {
            corpseTimer += Time.deltaTime;
            if (corpseTimer >= _respawnTime)
            {
                /*   Drop dr = ComponentDrop.DropList.Find(s => s.DropClickCollider == DropClickCollider);
                   if(dr!= null)
                   if (!dr.Opened)
                   {
                       ECharacterState = ECharacterState.Delete;
                       ComponentDrop.DropList.Remove(dr);
                   }*/
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