using System;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.Enemy
{
    public class EnemyStatistics : MonoBehaviour
    {
        public EnemyCharacter EnemyCharacter;
        public GameObject Prefab;
        public int Level;
        public int RespawnTime;
        public int Id;
        private const int OFFSET = 5;
        private const int TABLE_HEIGHT = 25;
        private const int TABLE_WIDTH = 100;
        public Vector3 DefaultPosition { get; set; }
        public DropItem[] DropList;
        public DropMoney MoneyDrop;
        public EEnemies EEnemies;
        public int[] LevelRange;
        private float _timer;
        public GameObject Spawn { get; set; }
        public float HowFarCanIGo { get; set; }
        // Use this for initialization
        void Start()
        {
            EnemyCharacter = new EnemyCharacter(Id,GetComponent<BoxCollider>(), transform, DropList, RespawnTime, MoneyDrop) { Level = Level };
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
                gameObject.transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
                gameObject.transform.Find("Projector").GetComponent<Projector>().enabled = false;
                gameObject.transform.Find("TargetPrefab").GetComponent<Canvas>().enabled = false;                
                Respawn();
                transform.parent.GetComponent<SpawnComponent>().SpawnedEnemies.Remove(gameObject);
                Destroy(gameObject);
            }
        }

        private void Respawn()
        {           
            EnemyGenerate.GenerateEnemy(new SEnemyGenerate(gameObject, 1, true, HowFarCanIGo, Spawn.name, true, Spawn));           
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
