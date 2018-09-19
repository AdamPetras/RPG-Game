using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Enemy
{
    public class SpawnComponent : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> SpawnedEnemies;
        private static bool _onSpawnSet;
        private void Awake()
        {
            SpawnedEnemies = new List<GameObject>();
            _onSpawnSet = false;
        }

        private void Start()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            foreach (GameObject enemies in SpawnedEnemies.Where(s => Vector3.Distance(s.transform.position, playerTransform.position) < 50))
            {
                foreach (MonoBehaviour c in enemies.GetComponents<MonoBehaviour>())
                {
                    c.enabled = true;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                foreach (GameObject enemies in SpawnedEnemies)
                {
                    foreach (MonoBehaviour c in enemies.GetComponents<MonoBehaviour>())
                    {
                        c.enabled = true;
                    }
                }

        }

        /*  void OnTriggerStay(Collider other)
          {
              if (!_onSpawnSet)
              {
                  if (other.tag == "Player")
                      foreach (GameObject enemies in SpawnedEnemies)
                      {
                          foreach (MonoBehaviour c in enemies.GetComponents<MonoBehaviour>())
                          {
                              c.enabled = true;
                          }
                      }
                  _onSpawnSet = true;
              }
          }*/

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                foreach (GameObject enemies in SpawnedEnemies)
                {
                    foreach (MonoBehaviour c in enemies.GetComponents<MonoBehaviour>())
                    {
                        c.enabled = false;
                    }
                }
        }
    }
}