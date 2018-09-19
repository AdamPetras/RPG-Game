using System;
using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.HUD;
using Assets.Script.StatisticsFolder;
using Assets.Script.TargetFolder;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Script.CombatFolder
{
    public class PlayerAttack : Attack
    {
        private Transform _enemyTransformBefore;
        public static GameObject Interact;
        private bool _isCrit;
        public PlayerAttack(Transform transform)
        {
            PlayerComponent = transform.GetComponent<PlayerComponent>();
            PlayerTransform = transform;
            EnemyTransform = null;
            Interact = null;
            _isCrit = false;
        }

        protected override void LetsAttack()
        {

            base.LetsAttack();
        }

        protected override void WhichAttack(float distance)
        {
            Vector3 dir;
            dir = (EnemyTransform.position - PlayerComponent.transform.position).normalized;    //směr
            float direction = Vector3.Dot(dir, PlayerTransform.forward);     //výpočet natočení modelu           
            if (distance < 2.5f && direction > 0 && Interact != null)   //pokud je vzdálenost menší než... a směr neni menší než 0 a 0
            {
                if (EnemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive)    //pokud je živý
                {
                    EnemyStatistics.EnemyCharacter.Angry = true;    //nastavení enemy angry
                    float damage = PlayerComponent.character.GetDamageStats((int)PlayerComponent.SelectWhichWeapon()).CurrentValue;      //nastavení damagu
                    float block = EnemyStatistics.EnemyCharacter.GetDamageStats((int)EDamageStats.DamageBlock).CurrentValue;    //nastavení bloku
                    if (Random.Range(0, 100) <=
                        PlayerComponent.character.GetDamageStats((int)EDamageStats.CriticalChance).CurrentValue)
                    {
                        DamageDone = (damage + DamageOscilation(damage) - block) * 2;     //Critic damage
                        _isCrit = true;
                    }
                    else
                    {
                        DamageDone = damage + DamageOscilation(damage) - block;        //výsledný útok
                        _isCrit = false;
                    }
                    EnemyStatistics.EnemyCharacter.DamageDone(DamageDone, false);   //odečtení od vitalu     
                    DamageHud.Hit(true,DamageDone);
                    if (EnemyStatistics.EnemyCharacter.GetVital((int)EVital.Health).CurrentValue <= 1) //pokud má méně životu mež 0 nebo 0 životů tak nastavení enumu
                    {
                        EnemyStatistics.EnemyCharacter.ECharacterState = ECharacterState.Dead;
                        EnemyStatistics.EnemyCharacter.ECombatState = ECombatState.NoneCombat;
                        EnemyStatistics.EnemyCharacter.Angry = false;
                    }
                    //nastavení na default hodnoty
                    _combatTimer = 0;
                    _attackTimer = 0;
                }
            }
        }

        protected override void AttackTimer()
        {
            base.AttackTimer();
        }

        protected override void CombatTimer()
        {
            _combatTimer += Time.deltaTime;
            if (EnemyStatistics != null)
                if (_combatTimer >= COMBAT_TIMER && !EnemyStatistics.EnemyCharacter.Angry)
                {
                    PlayerComponent.character.ECombatState = ECombatState.NoneCombat;
                }
                else
                {
                    PlayerComponent.character.ECombatState = ECombatState.InCombat;
                }
        }

        public override void Update()
        {
            AttackSpeed = PlayerComponent.character.GetDamageStats((int)EDamageStats.AttackSpeed).CurrentValue;
            if (Interact != null)
                EnemyTransform = Interact.transform;
            if (EnemyTransform != null)
                MobEnemy();
            base.Update();
        }

        private void MobEnemy()
        {
            if (EnemyTransform.tag == "Enemy" && EnemyTransform != _enemyTransformBefore) //set enemy statistics z targetu
            {
                _enemyTransformBefore = EnemyTransform;

                if (EnemyTransform.GetComponent<EnemyStatistics>() != null)
                {
                    EnemyStatistics = EnemyTransform.GetComponent<EnemyStatistics>();
                }
            }
        }

        private void GetTransform(Transform t1)
        {
            EnemyTransform = t1;
        }
    }
}
