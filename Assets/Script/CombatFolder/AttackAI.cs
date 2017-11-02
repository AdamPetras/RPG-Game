using UnityEngine;

namespace Assets.Script.CombatFolder
{
    public class AttackAI : MonoBehaviour
    {
        public bool IsPlayer;
        private EnemyAttack _enemyAttack;
        private PlayerAttack _playerAttack;
        // Use this for initialization
        private void Start()
        {
            if (!IsPlayer)
                _enemyAttack = new EnemyAttack(transform);
            else
                _playerAttack = new PlayerAttack(transform);
        }

        void Update()
        {
            if (!IsPlayer)
                _enemyAttack.Update();
            else
                _playerAttack.Update();
        }

      /*  void OnGUI()
        {
            if (IsPlayer)
                _playerAttack.OnGUI();
            else 
                _enemyAttack.OnGUI();
        }*/

    }
}
