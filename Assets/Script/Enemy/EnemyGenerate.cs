using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Script.Enemy
{
    [Serializable]
    public enum EEnemies
    {
        FireElement,
        WaterElement
    }

    public enum EGenerateState
    {
        Init,
        Generate,
        Idle
    }
    [Serializable]
    public struct SEnemyGenerate
    {
        public GameObject prefab;
        public int numberOfEnemies;
        public GameObject[] Spawns;
        public SEnemyGenerate(GameObject prefab, int numberOfEnemies,params GameObject[] spawns)
        {
            this.prefab = prefab;
            Spawns = spawns;
            this.numberOfEnemies = numberOfEnemies;
        }
    }

    public class EnemyGenerate : MonoBehaviour
    {
        public List<SEnemyGenerate> _enemyGenerateList;

        // Use this for initialization
        void Start()
        {
            Generate();
        }

        private void Generate()
        {
            for (int i = 0; i < _enemyGenerateList.Count; i++)
            {
                SEnemyGenerate enemy = _enemyGenerateList[i];
                for (int j = 0; j < _enemyGenerateList[i].numberOfEnemies; j++)
                {
                    GenerateEnemy(enemy);
                }
            }
        }

        public static void GenerateEnemy(SEnemyGenerate enemy)
        {
            EnemyStatistics enemyStatistics = enemy.prefab.GetComponent<EnemyStatistics>();
            int randomSpawn = Rnd(0, enemy.Spawns.Length);
            Vector3 position = new Vector3(enemy.Spawns[randomSpawn].transform.position.x + Rnd(-5, 5), 0, enemy.Spawns[randomSpawn].transform.position.z + Rnd(-5, 5));  //nastavení pozice enemy
            position.y = Terrain.activeTerrain.SampleHeight(position);  //nastavení Y na hodnotu terénu
            GameObject enemyObject = Instantiate(enemy.prefab, position, Quaternion.identity) as GameObject;  //vytvoření objektu a instanciování
            enemyObject.name = enemyStatistics.EEnemies.ToString();   //nastavení jména
            enemyObject.GetComponent<EnemyStatistics>().Spawn = enemy.Spawns[randomSpawn];
            enemy.Spawns[randomSpawn].GetComponent<SpawnComponent>().SpawnedEnemies.Add(enemyObject);
            enemyObject.GetComponent<EnemyStatistics>().Level = Rnd(enemyStatistics.LevelRange[0], enemyStatistics.LevelRange[1] + 1);
            enemyObject.GetComponent<EnemyStatistics>().DefaultPosition = position;
            enemyObject.transform.parent = GameObject.Find(enemy.Spawns[randomSpawn].name).transform;   //nastavení transformu na spawnovy transform
            foreach (MonoBehaviour c in enemyObject.GetComponents<MonoBehaviour>())
            {
                c.enabled = false;
            }
        }

        private static int Rnd(int start, int end)
        {
            return Random.Range(start, end);
        }
    }
}
