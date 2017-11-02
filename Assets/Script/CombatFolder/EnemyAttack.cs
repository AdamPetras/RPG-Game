using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.HUD;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.CombatFolder
{
    public class EnemyAttack : Attack
    {
        private DamageHud _damageHud;
        public EnemyAttack(Transform transform)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            EnemyTransform = transform;
            PlayerComponent = PlayerTransform.GetComponent<PlayerComponent>();
            EnemyStatistics = transform.GetComponent<EnemyStatistics>();
            _damageHud = new DamageHud(Color.red, 30, false);
            DamageDone = 0;
        }
       /* public void OnGUI()
        {
            if (DamageDone != 0)
                _damageHud.OnGUI(DamageDone, false);
        }*/
        protected override void LetsAttack()
        {
            base.LetsAttack();
        }

        protected override void AttackTimer()
        {
            base.AttackTimer();
        }

        protected override void WhichAttack(float distance)
        {
            if (EnemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive)
            {/*
                Vector3 dir;
                dir = (PlayerComponent.transform.position - EnemyTransform.position).normalized;    //směr
                float direction = Vector3.Dot(dir, EnemyTransform.forward);     //výpočet natočení modelu*/
                if (distance < 2.5f /*&& direction > 0*/)   //pokud je vzdálenost menší než... a směr neni menší než 0 a 0
                {
                    if (EnemyStatistics.EnemyCharacter.Angry)   //pokud je enemy naštván
                    {
                        float damage = EnemyStatistics.EnemyCharacter.GetDamageStats((int)EDamageStats.SwordDamage).ValuesTogether;    //nastavení damagu
                        float block = PlayerComponent.character.GetDamageStats((int)EDamageStats.DamageBlock).CurrentValue;     //nastavení bloku     //oscilace damagu
                        DamageDone = damage + DamageOscilation(damage) - block;     //výsledný útok
                        PlayerComponent.character.DamageDone(DamageDone, true); //odečtení od vitalu
                        if (PlayerComponent.character.GetVital((int)EVital.Health).CurrentValue <= 1)  //pokud má méně životu mež 0 nebo 0 životů tak nastavení enumu
                        {
                            PlayerComponent.character.ECharacterState = ECharacterState.Dead;
                            PlayerComponent.character.ECombatState = ECombatState.NoneCombat;
                            EnemyStatistics.EnemyCharacter.Angry = false;
                        }
                        //nastavení na default hodnoty
                        _attackTimer = 0;
                        _combatTimer = 0;
                    }
                }
            }
        }

        protected override void CombatTimer()
        {
            _combatTimer += Time.deltaTime;
            //pokud je combat timer větší než konstanta a nebbo vzdálenost hráče od defaultní pozice je větší jak 15
            if (_combatTimer >= COMBAT_TIMER || Vector3.Distance(PlayerComponent.transform.position, EnemyStatistics.DefaultPosition) > 15)
            {
                //vynulování combatu

                EnemyStatistics.EnemyCharacter.ECombatState = ECombatState.NoneCombat;
                EnemyStatistics.EnemyCharacter.Angry = false;
            } //nastavení combatu
            else
            {
                EnemyStatistics.EnemyCharacter.ECombatState = ECombatState.InCombat;
            }
        }

        public override void Update()
        {
            //nastavení atackspeedu podle statů
            AttackSpeed = EnemyStatistics.EnemyCharacter.GetDamageStats((int)EDamageStats.AttackSpeed).CurrentValue;
            if (EnemyStatistics.EnemyCharacter.Angry)
                base.Update();
        }
    }
}
