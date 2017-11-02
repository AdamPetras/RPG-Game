using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.CharacterFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.CombatFolder
{
    public class EnemyAI : MonoBehaviour
    {
        public Transform Target;
        public int MoveSpeed;
        public int RotationSpeed;
        public const int minDistance = 2;
        private Transform myTransform;
        private EnemyStatistics enemyStatistics;
        // Use this for initialization
        void Start()
        {
            GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
            enemyStatistics = transform.GetComponent<EnemyStatistics>();
            Target = gameObject.transform;
            myTransform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            if ((enemyStatistics.EnemyCharacter.Angry && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive) && Target.GetComponent<PlayerComponent>().character.ECharacterState == ECharacterState.Alive)
            {
                GoTo(Target.position, 2.0f);
            }
            else if ((!enemyStatistics.EnemyCharacter.Angry && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive) || (Target.GetComponent<PlayerComponent>().character.ECharacterState != ECharacterState.Alive && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive))
            {

                GoTo(enemyStatistics.DefaultPosition, 1f);
            }
        }

        private void GoTo(Vector3 positionToGo, float minDist)
        {
            if (Vector3.Distance(positionToGo, myTransform.position) > minDist)
            {
                Debug.DrawLine(Target.position, myTransform.position, Color.black);
                //otáčení enemy
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                    Quaternion.LookRotation(positionToGo - myTransform.position), RotationSpeed * Time.deltaTime);
                //pathfind
                myTransform.position += myTransform.forward * MoveSpeed * Time.deltaTime;
                myTransform.position = Utilities.SetPositionToCopyTerrain(myTransform.position); //kolize s terénem
            }
        }
    }
}
