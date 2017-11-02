using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.StatisticsFolder;
using Assets.Scripts.InventoryFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.SpellFolder
{
    public enum ESpell
    {
        Buff,
        Damage,
        Passive,
        None

    }

    public class Spell
    {
        public int ID { get; set; }
        public Sprite Icon { get; set; }
        public string Name { get; set; }
        public int LevelToAccess { get; set; }
        public int GoldToAccess { get; set; }
        public float ManaCost { get; set; }
        public float Range { get; set; }
        public float Cooldown { get; set; }
        public ESpell SpellType { get; set; }
        public float Duration { get; set; }
        public ESkill Skill { get; set; }
        public float PercentageIncrease { get; set; }
        public float PercentageDamage { get; set; }
        public float Timer { get; set; }
        public float CoolDownTimer { get; set; }
        public bool Unlocked { get; set; }
        public string Description { get; set; }
        public bool CooldownEnable { get; set; }
        public float CastTime { get; set; }
        private float _savedValue;
        private GameObject _instantiateBuff;
        private GameObject dialog;
        public bool IWantToDestroy { get; set; }
        public Spell(Dictionary<string, string> spell)
        {
            ID = int.Parse(spell["ID"]);
            Name = spell["Name"];
            Description = spell["Description"];
            Icon = Resources.Load<Sprite>("Graphics/Spell/" + Name);
            LevelToAccess = int.Parse(spell["LevelToAccess"]);
            GoldToAccess = int.Parse(spell["GoldToAccess"]);
            ManaCost = float.Parse(spell["ManaCost"]);
            Range = float.Parse(spell["Range"]);
            Cooldown = float.Parse(spell["Cooldown"]);
            //CastTime******************************************
            CastTime = 0f;
            //
            if (spell["SpellType"] != "")
                SpellType = (ESpell)Enum.Parse(typeof(ESpell), spell["SpellType"], true);
            else
                SpellType = ESpell.None;
            Duration = float.Parse(spell["Duration"]);
            if (spell["Skill"] != "")
                Skill = (ESkill)Enum.Parse(typeof(ESkill), spell["Skill"], true);
            else
                Skill = ESkill.None;
            PercentageIncrease = float.Parse(spell["PercentageIncrease"]);
            PercentageDamage = float.Parse(spell["PercentageDamage"]);
            CooldownEnable = true;
            CoolDownTimer = 0f;
            Unlocked = false;
        }

        public Spell(Spell spell)
        {
            ID = spell.ID;
            Name = spell.Name;
            Icon = spell.Icon;
            LevelToAccess = spell.LevelToAccess;
            GoldToAccess = spell.GoldToAccess;
            ManaCost = spell.ManaCost;
            Range = spell.Range;
            Cooldown = spell.Cooldown;
            SpellType = spell.SpellType;
            Skill = spell.Skill;
            Duration = spell.Duration;
            PercentageIncrease = spell.PercentageIncrease;
            PercentageDamage = spell.PercentageDamage;
            Unlocked = spell.Unlocked;
            Description = spell.Description;
            CoolDownTimer = spell.CoolDownTimer;
            CooldownEnable = spell.CooldownEnable;
        }

        public static Spell IdToSpell(int id)
        {
            return Database.SpellDatabase.Find(s => s.ID == id);
        }

        public void Unlock(GameObject gameObject)
        {
            if (dialog != null)
                return;
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
                return;
            PlayerComponent plComp = playerObject.GetComponent<PlayerComponent>();
            if (plComp.Level < LevelToAccess)
            {
                MyDebug.Log("Not enough level.");
                return;
            }
            dialog = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DialogWindow"));
            Transform bckg = dialog.transform.Find("Background");
            bckg.Find("Text").GetComponent<Text>().text = "Do you want to unlock this spell for " + GoldToAccess + " golds?";
            bckg.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate { OnYes(dialog, plComp, gameObject); });
            bckg.Find("No").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Destroy(dialog); });
        }

        private void OnYes(GameObject dialog, PlayerComponent plComp, GameObject gameObject)
        {

            if (GoldToAccess > plComp.Money)
            {
                MyDebug.Log("Not enough money");
                return;
            }
            plComp.Money -= (uint)GoldToAccess;
            Unlocked = true;
            GameObject.Destroy(dialog);
        }

        public bool Cast(GameObject gameObject)
        {
            if (!CooldownEnable)
                return false;
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj == null)
                return false;
            PlayerComponent playerComponent = obj.GetComponent<PlayerComponent>();
            if (SpellType == ESpell.Buff || SpellType == ESpell.Passive)
            {
                float f = playerComponent.character.GetSkill((int)Skill).ValuesTogether;
                f *= PercentageIncrease * 0.01f;
                _savedValue = f - playerComponent.character.GetSkill((int)Skill).ValuesTogether;
                playerComponent.character.GetSkill((int)Skill).BonusValue += _savedValue;
                playerComponent.SpellList.Add(this);
                playerComponent.character.UpdateStats();
                _instantiateBuff = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/BuffItem"),
                    playerComponent.BuffPanel);
                _instantiateBuff.GetComponent<Image>().sprite = Icon;
                if (SpellType == ESpell.Buff)
                    _instantiateBuff.transform.Find("Text").GetComponent<Text>().text =
                        (Duration - Timer).ToString();
                CooldownEnable = false;
                return true;
            }
            else
            {
                GameObject enemyObject = PlayerAttack.Interact;
                if (enemyObject == null)
                {
                    MyDebug.Log("No target");
                    return false;
                }
                float damage = playerComponent.character.GetDamageStats((int)EDamageStats.SwordDamage).CurrentValue * (PercentageDamage * 0.01f);      //nastavení damagu
                float block = enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.GetDamageStats((int)EDamageStats.DamageBlock).CurrentValue;    //nastavení bloku
                enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.GetVital((int)EVital.Health).CurrentValue -= damage + Attack.DamageOscilation(damage) - block;
                CooldownEnable = false;
                return true;
            }
        }



        public IEnumerator CooldownTimer(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                float amount;
                if (Cooldown - CoolDownTimer != 0)
                    amount = (Cooldown - CoolDownTimer) / Cooldown;
                else amount = 1;
                foreach (ComponentSpell spell in ComponentSpell.SpellList.Where(s => s.Spell.ID == ID))
                {
                    spell.gameObject.transform.Find("Cooldown").GetComponent<Image>().fillAmount = amount;                   
                }
                CoolDownTimer += delay;
                if (CoolDownTimer >= Cooldown)
                {
                    CooldownEnable = true;
                    CoolDownTimer = 0;
                    foreach (ComponentSpell spell in ComponentSpell.SpellList.Where(s => s.Spell.ID == ID))
                    {
                        spell.gameObject.transform.Find("Cooldown").GetComponent<Image>().fillAmount = 0f;
                    }
                    yield break;
                }
            }
        }

        public IEnumerator AnullBuff(float delay)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj == null)
                yield return null;
            PlayerComponent playerComponent = obj.GetComponent<PlayerComponent>();
            while (true)
            {
                yield return new WaitForSeconds(delay);
                Timer += delay;
                if (_instantiateBuff != null)
                    _instantiateBuff.transform.Find("Text").GetComponent<Text>().text = (Duration - Timer).ToString();
                if (Timer >= Duration)
                {
                    Timer = 0;
                    float f = playerComponent.character.GetSkill((int)Skill).ValuesTogether;
                    f *= 1 / (PercentageIncrease * 0.01f);
                    _savedValue = playerComponent.character.GetSkill((int)Skill).ValuesTogether - f;
                    playerComponent.character.GetSkill((int)Skill).BonusValue -= _savedValue;
                    playerComponent.character.UpdateStats();
                    playerComponent.SpellList.Remove(this);
                    GameObject.Destroy(_instantiateBuff);
                    yield break;
                }
            }
        }
    }
}