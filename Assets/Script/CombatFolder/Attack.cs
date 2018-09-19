using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.TargetFolder;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Script.CombatFolder
{
    public abstract class Attack
    {
        protected Transform PlayerTransform;
        protected PlayerComponent PlayerComponent;
        protected Transform EnemyTransform;
        protected EnemyStatistics EnemyStatistics;
        protected float DamageDone;
        protected float AttackSpeed;
        protected float _attackTimer;
        protected float _combatTimer;
        protected const int COMBAT_TIMER = 5;

        private delegate void AttackDelegate();

        private event AttackDelegate EventAttack;

        protected Attack()
        {
            EventAttack += LetsAttack;      //přidělení metody eventu
            EventAttack += CombatTimer;     //přidělení metody eventu
        }

        public virtual void Update()
        {
            OnEventAttack();    //event
            AttackTimer();
        }

        protected virtual void LetsAttack()
        {
            float distance = 10;
            if (EnemyTransform != null) //pokud enemy transform neni null tak
                if (EnemyTransform.tag == "Enemy")
                    distance = Vector3.Distance(PlayerTransform.position, EnemyTransform.position); //počítání vzdálenosti
            if (distance < 2.5f && _attackTimer >= AttackSpeed) //pokud je vzdálenost menší než a attacktimer větší nez attackSpeed
            {
                WhichAttack(distance);
            }
        }

        protected virtual void WhichAttack(float distance)
        {

        }

        protected virtual void CombatTimer()
        {

        }

        protected virtual void AttackTimer()
        {           
            if (_attackTimer < AttackSpeed) //pokud je attackTimer menší než attackSpeed
            {
                _attackTimer += Time.deltaTime;
            }
            else DamageDone = 0;
        }

        protected virtual void OnEventAttack()
        {
            if (EventAttack != null) EventAttack.Invoke();
        }

        public static float DamageOscilation(float damage)
        {
            return Random.Range(-damage * 1 / 6, damage * 1 / 6);
        }
    }
}
