using System;
using Assets.Script.CharacterFolder;
using UnityEngine;

namespace Assets.Script.StatisticsFolder
{
    public enum ECharacterState
    {
        Alive,
        Dead,
        Delete
    }

    public enum ECombatState
    {
        InCombat,
        AFK,
        NoneCombat
    }

    public struct CharacterSkill
    {
        public Skill Skills;
        public float Bonus;

        public CharacterSkill(Skill skills, float bonus)
        {
            Skills = skills;
            Bonus = bonus;
        }
    }

    public class Character
    {
        public Skill[] skillArray;
        public DamageStats[] damageStatsArray;
        public Vital[] vitalArray;
        public ECharacter ECharacter { get; set; }
        public ECharacterState ECharacterState { get; set; }
        public ECombatState ECombatState { get; set; }
        private float hpRegenTrashold = 1f;
        private float timer;
        public Character()
        {
            skillArray = new Skill[Enum.GetValues(typeof(ESkill)).Length];
            damageStatsArray = new DamageStats[Enum.GetValues(typeof(EDamageStats)).Length];
            vitalArray = new Vital[Enum.GetValues(typeof(EVital)).Length];
            ECharacter = ECharacter.Human;
            ECharacterState = ECharacterState.Alive;
            ECombatState = ECombatState.NoneCombat;
            LoadSkill();
            LoadVitalStats();
            LoadDamageStats();
        }

        public void HealthRegen()
        {
            if ((ECombatState == ECombatState.AFK || ECombatState == ECombatState.NoneCombat) &&
                ECharacterState == ECharacterState.Alive)
            {
                if (GetVital((int)EVital.Health).CurrentValue < GetVital((int)EVital.Health).MaxValue ||
                    GetVital((int)EVital.Mana).CurrentValue < GetVital((int)EVital.Mana).MaxValue)
                {
                    timer += Time.deltaTime;
                    if (timer > hpRegenTrashold)
                    {
                        if (GetVital((int)EVital.Health).CurrentValue < GetVital((int)EVital.Health).MaxValue)
                        {
                            GetVital((int)EVital.Health).CurrentValue +=
                                GetDamageStats((int)EDamageStats.HealthRegen).CurrentValue;
                        }
                        if (GetVital((int)EVital.Mana).CurrentValue < GetVital((int)EVital.Mana).MaxValue)
                            GetVital((int)EVital.Mana).CurrentValue +=
                                GetDamageStats((int)EDamageStats.HealthRegen).CurrentValue;
                        timer = 0;
                    }
                }
                else if (GetVital((int)EVital.Health).CurrentValue > GetVital((int)EVital.Health).MaxValue) //přetečení
                {
                    GetVital((int)EVital.Health).CurrentValue = GetVital((int)EVital.Health).MaxValue;
                }
                else if (GetVital((int)EVital.Mana).CurrentValue > GetVital((int)EVital.Mana).MaxValue)
                {
                    GetVital((int)EVital.Mana).CurrentValue = GetVital((int)EVital.Mana).MaxValue;
                }
            }
        }

        private void LoadSkill()
        {
            for (int i = 0; i < skillArray.Length; i++)
            {
                skillArray[i] = new Skill();
            }
        }
        private void LoadDamageStats()
        {
            for (int i = 0; i < damageStatsArray.Length; i++)
                damageStatsArray[i] = new DamageStats();
            SetupDamageStats();
        }

        private void LoadVitalStats()
        {
            for (int i = 0; i < vitalArray.Length; i++)
                vitalArray[i] = new Vital();
            SetupVital();
        }
        public Skill GetSkill(int index)
        {
            return skillArray[index];
        }
        public DamageStats GetDamageStats(int index)
        {
            return damageStatsArray[index];
        }

        public Vital GetVital(int index)
        {
            return vitalArray[index];
        }

        public bool AddHealth(float value)
        {
            if ((int)GetVital((int)EVital.Health).CurrentValue == (int)GetVital((int)EVital.Health).MaxValue && value > 0)
                return false;
            if (GetVital((int)EVital.Health).CurrentValue + value < GetVital((int)EVital.Health).MaxValue)
                GetVital((int)EVital.Health).BonusValue += value;
            else GetVital((int)EVital.Health).CurrentValue = GetVital((int)EVital.Health).MaxValue;
            return true;
        }

        public void DamageDone(float damage, bool player)
        {
            GetVital((int)EVital.Health).CurrentValue -= damage;
        }

        protected virtual void SetupDamageStats()
        {
            //sword damage
            ModifySkill sword = new ModifySkill(GetSkill((int)ESkill.Attack), 1f);//1
            GetDamageStats((int)EDamageStats.SwordDamage).AddSkill(sword);
            //nesčítat
            GetSkill((int)ESkill.Dexterity).IfAdd = false;
            //attack speed
            ModifySkill attackSpeed = new ModifySkill(GetSkill((int)ESkill.Dexterity), -0.005f, 2);
            GetDamageStats((int)EDamageStats.AttackSpeed).AddSkill(attackSpeed);
            //defence
            ModifySkill def = new ModifySkill(GetSkill((int)ESkill.Defence), 0.1f);
            GetDamageStats((int)EDamageStats.DamageBlock).AddSkill(def);
            //mace damage
            ModifySkill mace = new ModifySkill(GetSkill((int)ESkill.Strenght), 1);
            GetDamageStats((int)EDamageStats.MaceDamage).AddSkill(mace);
            //axe damage
            ModifySkill axe = new ModifySkill(GetSkill((int)ESkill.Attack), GetSkill((int)ESkill.Strenght), 0.6f, 0.6f);
            GetDamageStats((int)EDamageStats.AxeDamage).AddSkill(axe);
            //ranged damage
            ModifySkill ranged = new ModifySkill(GetSkill((int)ESkill.Ranged), 1);
            GetDamageStats((int)EDamageStats.RangedPower).AddSkill(ranged);
            //magic power
            ModifySkill magic = new ModifySkill(GetSkill((int)ESkill.Magic), 1);
            GetDamageStats((int)EDamageStats.MagicPower).AddSkill(magic);
            //health regen
            ModifySkill healthRegen = new ModifySkill(GetSkill((int)ESkill.Regenerate), 0.1f);
            GetDamageStats((int)EDamageStats.HealthRegen).AddSkill(healthRegen);
            //nesčítat
            GetSkill((int)ESkill.Agility).IfAdd = false;
            //chance to critical damage
            ModifySkill critChance = new ModifySkill(GetSkill((int)ESkill.Agility), 0.1f, 15f);
            GetDamageStats((int)EDamageStats.CriticalChance).AddSkill(critChance);
        }
        protected virtual void SetupVital()
        {
            //healh
            ModifySkill health = new ModifySkill(GetSkill((int)ESkill.Constitution), 10);
            GetVital((int)EVital.Health).AddSkill(health);
            //mana
            ModifySkill mana = new ModifySkill(GetSkill((int)ESkill.Intelect), 10);
            GetVital((int)EVital.Mana).AddSkill(mana);
        }

        public void UpdateStats()
        {
            foreach (Vital t in vitalArray)
            {
                t.Update();
            }
            foreach (DamageStats t in damageStatsArray)
            {
                t.Update();
            }
        }

        public void IncrementSkills(bool delete = false)    //every level incrementation
        {

            if (delete)
                foreach (Skill skill in skillArray)
                {
                    skill.BasicValue = 0;
                }
            if (ECharacterState == ECharacterState.Alive)
            {
                if (ECharacter == ECharacter.Human)
                {
                    AddStats(new CharacterSkill(GetSkill((int)ESkill.Attack), 1.2f),//1.2f
                        new CharacterSkill(GetSkill((int)ESkill.Constitution), 1.9f),
                        new CharacterSkill(GetSkill((int)ESkill.Magic), 1.4f),
                        new CharacterSkill(GetSkill((int)ESkill.Defence), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Ranged), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Strenght), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Intelect), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Regenerate), 1f),
                        new CharacterSkill(GetSkill((int)ESkill.Agility), 1.5f),
                        new CharacterSkill(GetSkill((int)ESkill.Dexterity), 1f)
                        );
                    //celkem 12.7
                }
                if (ECharacter == ECharacter.Bull)
                {
                    AddStats(new CharacterSkill(GetSkill((int)ESkill.Attack), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Constitution), 2.3f),
                        new CharacterSkill(GetSkill((int)ESkill.Magic), 1),
                        new CharacterSkill(GetSkill((int)ESkill.Defence), 1.3f),
                        new CharacterSkill(GetSkill((int)ESkill.Ranged), 1),
                        new CharacterSkill(GetSkill((int)ESkill.Strenght), 1.4f),
                        new CharacterSkill(GetSkill((int)ESkill.Intelect), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Regenerate), 1f),
                        new CharacterSkill(GetSkill((int)ESkill.Agility), 1.5f),
                        new CharacterSkill(GetSkill((int)ESkill.Dexterity), 1f)
                        );
                    //celkem 12.7
                }
                if (ECharacter == ECharacter.Elf)
                {
                    AddStats(new CharacterSkill(GetSkill((int)ESkill.Attack), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Constitution), 1.9f),
                        new CharacterSkill(GetSkill((int)ESkill.Magic), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Defence), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Ranged), 1.5f),
                        new CharacterSkill(GetSkill((int)ESkill.Strenght), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Intelect), 1.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Regenerate), 1f),
                        new CharacterSkill(GetSkill((int)ESkill.Agility), 1.5f),
                        new CharacterSkill(GetSkill((int)ESkill.Dexterity), 1f)
                        );
                    //celkem 12.7              
                }
                if (ECharacter == ECharacter.Ork)
                {
                    AddStats(new CharacterSkill(GetSkill((int)ESkill.Attack), 1.4f),
                        new CharacterSkill(GetSkill((int)ESkill.Constitution), 2.2f),
                        new CharacterSkill(GetSkill((int)ESkill.Magic), 1),
                        new CharacterSkill(GetSkill((int)ESkill.Defence), 1.3f),
                        new CharacterSkill(GetSkill((int)ESkill.Ranged), 1),
                        new CharacterSkill(GetSkill((int)ESkill.Strenght), 1.3f),
                        new CharacterSkill(GetSkill((int)ESkill.Intelect), 1.1f),
                        new CharacterSkill(GetSkill((int)ESkill.Regenerate), 1f),
                        new CharacterSkill(GetSkill((int)ESkill.Agility), 1.5f),
                        new CharacterSkill(GetSkill((int)ESkill.Dexterity), 1f)
                        );
                    //celkem 12.7
                }
            }
        }
        public void AddStats(params CharacterSkill[] values)
        {
            foreach (CharacterSkill value in values)
            {
                value.Skills.BasicValue += value.Bonus;
            }
            UpdateStats();
        }
    }
}
