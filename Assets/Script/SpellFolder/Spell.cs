using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder;
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
        Dot,
        Weakness,
        None
    }

    public class Spell
    {
        public int ID { get; set; }
        public Sprite Icon { get; set; }
        public string Name { get; set; }
        public int LevelToAccess { get; set; }
        public uint GoldToAccess { get; set; }
        public float ManaCost { get; set; }
        public float Range { get; set; }
        public float Cooldown { get; set; }
        public ESpell SpellType { get; set; }
        public float Duration { get; set; }
        public ESkill Skill { get; set; }
        public EVital Vital { get; set; }
        public EDamageStats DamageStats { get; set; }
        public float PercentageIncreaseDecrease { get; set; }
        public float PercentageDamage { get; set; }
        public float Timer { get; set; }
        public float CoolDownTimer { get; set; }
        public bool Unlocked { get; set; }
        public string Description { get; set; }
        public bool CooldownEnable { get; set; }
        public float CastTime { get; set; }
        public GameObject PlantedObject { get; set; }
        private float _deltaValueVital;
        private float _deltaValueDamage;
        private float _savedValue;      
        private GameObject _instantiateBuff;
        private GameObject dialog;
        private static GameObject staticDialog;
        public bool IWantToDestroy { get; set; }
        public Spell(Dictionary<string, string> spell)
        {
            ID = int.Parse(spell["ID"]);
            Name = spell["Name"];
            Description = spell["Description"];
            Icon = Resources.Load<Sprite>("Graphics/Spell/" + Name);
            LevelToAccess = int.Parse(spell["LevelToAccess"]);
            GoldToAccess = uint.Parse(spell["GoldToAccess"]);
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
            if (spell["Vital"] != "")
                Vital = (EVital)Enum.Parse(typeof(EVital), spell["Vital"], true);
            else
                Vital = EVital.None;
            if (spell["DamageStats"] != "")
                DamageStats = (EDamageStats)Enum.Parse(typeof(EDamageStats), spell["DamageStats"], true);
            else
                DamageStats = EDamageStats.None;
            PercentageIncreaseDecrease = float.Parse(spell["PercentageIncreaseDecrease"]);
            PercentageDamage = float.Parse(spell["PercentageDamage"]);
            CooldownEnable = true;
            CoolDownTimer = 0f;
            Unlocked = false;
            PlantedObject = null;
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
            PercentageIncreaseDecrease = spell.PercentageIncreaseDecrease;
            PercentageDamage = spell.PercentageDamage;
            Unlocked = spell.Unlocked;
            Description = spell.Description;
            CoolDownTimer = spell.CoolDownTimer;
            CooldownEnable = spell.CooldownEnable;
            PlantedObject = spell.PlantedObject;
            Vital = spell.Vital;
            DamageStats = spell.DamageStats;
        }

        public static Spell IdToSpell(int id)
        {
            return Database.SpellDatabase.Find(s => s.ID == id);
        }

        public void Unlock(GameObject spellObject)
        {
            if(staticDialog != null)
                GameObject.Destroy(staticDialog);
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
            staticDialog = dialog;
            Transform bckg = dialog.transform.Find("Background");
            bckg.Find("Text").GetComponent<Text>().text = "Do you want to unlock this spell for " + GoldToAccess + " golds?";
            bckg.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate { OnYes(dialog, spellObject); });
            bckg.Find("No").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Destroy(dialog); });
            bckg.Find("Exit").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Destroy(dialog); });
        }

        private void OnYes(GameObject dialog, GameObject spellObject)
        {

            if (!SlotManagement.AreTheseMoneyEnough(GoldToAccess))
            {
                MyDebug.Log("Not enough money "+ GoldToAccess+ " you have got "+SlotManagement.MoneyPounch());
                return;
            }
            SlotManagement.MoneyWithdraw(GoldToAccess);
            Unlocked = true;
            spellObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
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
                return WeakDotBuffAndPassive(obj, obj.GetComponent<PlayerComponent>().character, "Prefab/BuffItem");
            }
            if(SpellType == ESpell.Damage)
            {
                GameObject enemyObject = PlayerAttack.Interact;
                if (enemyObject == null)
                {
                    MyDebug.Log("No target");
                    return false;
                }
                float damage = playerComponent.character.GetDamageStats((int)playerComponent.SelectWhichWeapon()).CurrentValue * (PercentageDamage * 0.01f);      //nastavení damagu
                float block = enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.GetDamageStats((int)EDamageStats.DamageBlock).CurrentValue;    //nastavení bloku
                float damageDone = damage + Attack.DamageOscilation(damage) - block;
                DamageHud.Hit(true,damageDone);
                enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.GetVital((int)EVital.Health).CurrentValue -= damageDone;
                if(enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.GetVital((int)EVital.Health).CurrentValue <=0)
                    enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.ECharacterState = ECharacterState.Dead;
                CooldownEnable = false;
                return true;
            }
            if (SpellType == ESpell.Weakness)
            {
                if (PlayerAttack.Interact != null)
                    return WeakDotBuffAndPassive(PlayerAttack.Interact,
                        PlayerAttack.Interact.GetComponent<EnemyStatistics>().EnemyCharacter, "Prefab/BuffItem");
            }
            if (SpellType == ESpell.Dot)
            {
                if (PlayerAttack.Interact != null)
                    return WeakDotBuffAndPassive(PlayerAttack.Interact,
                        PlayerAttack.Interact.GetComponent<EnemyStatistics>().EnemyCharacter, "Prefab/BuffItem");
            }
            return false;
        }

        private bool WeakDotBuffAndPassive(GameObject obj, Character playerOrEnemyCharacter, string wayToPrefab)
        {
            if (obj == null)
            {
                MyDebug.Log("No target");
                return false;
            }
            float f;
            if (SpellType == ESpell.Weakness)
            {
                if (Vital != EVital.None)
                {
                    playerOrEnemyCharacter.GetVital((int)Vital).CurrentValue -= Weakness(playerOrEnemyCharacter, playerOrEnemyCharacter.GetVital((int)Vital).CurrentValue, out _deltaValueVital);
                }
                if (DamageStats != EDamageStats.None)
                {
                    playerOrEnemyCharacter.GetDamageStats((int)DamageStats).CurrentValue -= Weakness(playerOrEnemyCharacter, playerOrEnemyCharacter.GetDamageStats((int)DamageStats).CurrentValue, out _deltaValueDamage);
                }
            }
            else
            {
                f = playerOrEnemyCharacter.GetSkill((int) Skill).ValuesTogether;
                _deltaValueVital = playerOrEnemyCharacter.GetSkill((int) Skill).BonusValue;
                f *= PercentageIncreaseDecrease * 0.01f;
                if (SpellType == ESpell.Buff)
                    _savedValue = f - playerOrEnemyCharacter.GetSkill((int) Skill).ValuesTogether;
                else if (SpellType == ESpell.Dot)
                    _savedValue = 0;
                playerOrEnemyCharacter.GetSkill((int) Skill).BonusValue += _savedValue;
                _deltaValueVital -= playerOrEnemyCharacter.GetSkill((int) Skill).BonusValue;
            }
            playerOrEnemyCharacter.SpellList.Add(this);
            playerOrEnemyCharacter.UpdateStats();
            _instantiateBuff = GameObject.Instantiate(Resources.Load<GameObject>(wayToPrefab),
                playerOrEnemyCharacter.BuffPanel);
            _instantiateBuff.GetComponent<Image>().sprite = Icon;
            if(SpellType == ESpell.Buff || SpellType == ESpell.Weakness|| SpellType == ESpell.Dot)
                _instantiateBuff.transform.Find("Text").GetComponent<Text>().text = (Duration - Timer).ToString();
            CooldownEnable = false;
            PlantedObject = obj;
            return true;
        }

        private float Weakness(Character playerOrEnemyCharacter, float skill,out float defaultValue)
        {
            float f = skill;
            f *= PercentageIncreaseDecrease * 0.01f;
            defaultValue = f;
            return f;
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

        public IEnumerator AnullBuffDotOrWeakness(float delay)
        {
            GameObject obj = null;
            Character playerOrEnemyCharacter;
            if (SpellType == ESpell.Buff)
            {
                obj = GameObject.FindGameObjectWithTag("Player");
                if (obj == null)
                    yield return null;
                playerOrEnemyCharacter = obj.GetComponent<PlayerComponent>().character;
            }
            else if (SpellType == ESpell.Dot || SpellType == ESpell.Weakness)
            {                                 
                obj = GameObject.FindGameObjectWithTag("Player");
                if (obj == null)
                    yield return null;
                if (PlantedObject == null)
                    yield return null;
                playerOrEnemyCharacter = PlantedObject.GetComponent<EnemyStatistics>().EnemyCharacter;
            }
            else
            {
                playerOrEnemyCharacter = null;
                yield return null;
            }
            while (true)
            {
                yield return new WaitForSeconds(delay);
                Timer += delay;
                if (_instantiateBuff != null)
                    _instantiateBuff.transform.Find("Text").GetComponent<Text>().text = (Duration - Timer).ToString();
                if (SpellType == ESpell.Dot)
                {
                    float damage = obj.GetComponent<PlayerComponent>().character.GetDamageStats((int)obj.GetComponent<PlayerComponent>().SelectWhichWeapon()).CurrentValue * (PercentageDamage * 0.01f);
                    float damageDone = damage + Attack.DamageOscilation(damage);
                    DamageHud.Hit(true, damageDone);
                    playerOrEnemyCharacter.GetVital((int)EVital.Health).CurrentValue -= damageDone;
                    if (playerOrEnemyCharacter.GetVital((int)EVital.Health).CurrentValue <= 0)
                        playerOrEnemyCharacter.ECharacterState = ECharacterState.Dead;
                }
                if (Timer >= Duration || playerOrEnemyCharacter.ECharacterState == ECharacterState.Dead)
                {
                    Timer = 0;
                    if(playerOrEnemyCharacter == null)
                        yield return null;
                    if(SpellType == ESpell.Buff)
                        playerOrEnemyCharacter.GetSkill((int) Skill).BonusValue += _deltaValueVital;
                    else if (SpellType == ESpell.Weakness)
                    {
                        playerOrEnemyCharacter.GetVital((int)Vital).CurrentValue += _deltaValueVital;
                        playerOrEnemyCharacter.GetDamageStats((int)DamageStats).CurrentValue += _deltaValueDamage;
                    }
                    playerOrEnemyCharacter.UpdateStats();
                    playerOrEnemyCharacter.SpellList.Remove(this);
                    GameObject.Destroy(_instantiateBuff);
                    yield break;
                }
            }
        }
    }
}