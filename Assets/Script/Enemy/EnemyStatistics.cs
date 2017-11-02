using System;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.Enemy
{
    public class EnemyStatistics : MonoBehaviour
    {
        public EnemyCharacter EnemyCharacter;
        public int Level;
        public int RespawnTime;
        private const int OFFSET = 5;
        private const int TABLE_HEIGHT = 25;
        private const int TABLE_WIDTH = 100;
        public Vector3 DefaultPosition { get; set; }
        public DropItem[] DropList;
        public EEnemies EEnemies;
        public int[] LevelRange;
        private float _timer;
        public GameObject Spawn { get; set; }
        // Use this for initialization
        void Start()
        {
            EnemyCharacter = new EnemyCharacter(GetComponent<BoxCollider>(), transform, DropList, RespawnTime) { Level = Level };
            EnemyCharacter.CalculateStats();
        }

        // Update is called once per frame
        void Update()
        {
            EnemyCharacter.Update();
            if (EnemyCharacter.ECharacterState == ECharacterState.Dead)
            {
                EnemyCharacter.GetVital((int)EVital.Health).CurrentValue = 0;
                EnemyCharacter.Angry = false;
                EnemyCharacter.OnEventEnemyDie();
            }
            else if (EnemyCharacter.ECharacterState == ECharacterState.Delete)
            {
                Respawn();
                Destroy(gameObject);
            }
        }

        private void Respawn()
        {
            EnemyGenerate.GenerateEnemy(new SEnemyGenerate(gameObject, 1, Spawn));
        }

        /* private void ShowVitals()
         {
             for (int i = 0; i < Enum.GetValues(typeof(EDamageStats)).Length; i++)
             {
                 GUI.Label(new Rect(OFFSET, OFFSET + i * TABLE_HEIGHT + 250, TABLE_WIDTH, TABLE_HEIGHT), ((EDamageStats)i).ToString());
                 GUI.Label(new Rect(TABLE_WIDTH + OFFSET * 2, OFFSET + i * TABLE_HEIGHT + 250, TABLE_WIDTH, TABLE_HEIGHT), EnemyCharacter.GetDamageStats(i).MaxValue.ToString());
             }
         }
         private void ShowSkills()
         {
             for (int i = 0; i < Enum.GetValues(typeof(ESkill)).Length; i++)
             {
                 GUI.Label(new Rect(-OFFSET * 2 + Screen.width - TABLE_WIDTH * 2, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), EnemyCharacter.GetSkill(i).ValuesTogether.ToString());
                 GUI.Label(new Rect(-OFFSET + Screen.width - TABLE_WIDTH, OFFSET + i * TABLE_HEIGHT, TABLE_WIDTH, TABLE_HEIGHT), ((ESkill)i).ToString());
             }
         }*/
    }
}
