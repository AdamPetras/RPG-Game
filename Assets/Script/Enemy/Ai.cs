using System.Collections;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.StatisticsFolder;
using UnityEngine;

namespace Assets.Script.Enemy
{


    public class Ai : MonoBehaviour
    {
        private enum EMoveEnemy
        {
            Moving,
            Waiting,
            None
        }

        public Transform Target;
        public int MoveSpeed;
        public int RotationSpeed;
        public const int minDistance = 2;
        private Transform myTransform;
        private EnemyStatistics enemyStatistics;
        private BoxCollider _spawnCollider;
        private EMoveEnemy _eMoveEnemy;
        private const float THRESHOLD = 20;
        private float _timer;
        private float _waitTime;
        private Vector3 _whereToGo;

        private Vector3 CONSTFIRSTPOSITION;
        // Use this for initialization
        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            enemyStatistics = transform.GetComponent<EnemyStatistics>();
            CONSTFIRSTPOSITION = enemyStatistics.DefaultPosition;
            Target = player.transform;
            myTransform = transform;
            _spawnCollider = enemyStatistics.Spawn.GetComponent<BoxCollider>();
            _waitTime = Random.Range(THRESHOLD / 2, THRESHOLD);
            _eMoveEnemy = EMoveEnemy.Waiting;
            StartCoroutine(AfkMove());
            _whereToGo = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (_eMoveEnemy == EMoveEnemy.Moving && enemyStatistics.EnemyCharacter.ECharacterState != ECharacterState.Dead)
            {
                GoTo(_whereToGo, 0.1f);
            }
            if ((enemyStatistics.EnemyCharacter.Angry && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive) && Target.GetComponent<PlayerComponent>().character.ECharacterState == ECharacterState.Alive)
            {
                GoTo(Target.position, 2.0f);
            }
            else if ((!enemyStatistics.EnemyCharacter.Angry && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive) || (Target.GetComponent<PlayerComponent>().character.ECharacterState != ECharacterState.Alive && enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Alive))
            {
                if (_eMoveEnemy != EMoveEnemy.Moving)
                    GoTo(enemyStatistics.DefaultPosition, 1f);
            }
        }

        private IEnumerator AfkMove()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (enemyStatistics.EnemyCharacter.ECharacterState == ECharacterState.Dead)
                    yield return null;
                if (enemyStatistics.EnemyCharacter.Angry)   // pokud je enemy angry
                {
                    _timer = 0f;        //resetur timer
                    _eMoveEnemy = EMoveEnemy.Waiting;       //nastav enemymove na waiting
                    yield return new WaitUntil(() => !enemyStatistics.EnemyCharacter.Angry); //a čekej až nebude enemy angry
                }
                if (_eMoveEnemy == EMoveEnemy.Waiting)  //pokud enemy stojí a čeká
                {
                    _timer++;   //zapnutí timera a přičítání
                    if (_timer >= _waitTime)    //pokud je timer větší jako waitTime
                    {
                        //nastavení kde má jít pomocí defaultní pozice a offsetu howfarcanigo
                        _whereToGo = new Vector3(Random.Range(CONSTFIRSTPOSITION.x - enemyStatistics.HowFarCanIGo, CONSTFIRSTPOSITION.x + enemyStatistics.HowFarCanIGo), 0,
                            Random.Range(CONSTFIRSTPOSITION.z - enemyStatistics.HowFarCanIGo, CONSTFIRSTPOSITION.z + enemyStatistics.HowFarCanIGo));
                        //nastavení y pozice na pozici jakou tam má terén
                        _whereToGo.y = Terrain.activeTerrain.SampleHeight(_whereToGo);
                        enemyStatistics.DefaultPosition = _whereToGo;   //nastavení default pozice na novou defaultní
                        _waitTime = Random.Range(THRESHOLD / 2, THRESHOLD);     //nastavení nové čekací doby
                        _eMoveEnemy = EMoveEnemy.Moving;    //nastavení že může jít
                        _timer = 0f;    //nulování timeru
                    }
                }
                else if (Vector3.Distance(_whereToGo, myTransform.position) < 0.1)    // pokud je do dálky 1 od pozice
                {
                    _eMoveEnemy = EMoveEnemy.Waiting;   //tak čeká a nastaví timer
                    _timer = 0;
                }

            }
        }

        private void GoTo(Vector3 positionToGo, float minDist)
        {

            if (Vector3.Distance(positionToGo, myTransform.position) > minDist) //pokud je vzdálenost od bodu kde jdu větší jak zadaná tak jdu (pokud ne tak nejdu[přeskočím if])
            {
                myTransform.position += myTransform.forward * MoveSpeed * Time.deltaTime;   //nastavení pozice dopředu o rychlost * čas
                myTransform.SetPositionAndRotation(Utilities.SetPositionToCopyTerrain(myTransform.position),    //nastavení pozice a rotace
                    Quaternion.Slerp(myTransform.rotation,
                        Quaternion.LookRotation(positionToGo - myTransform.position), RotationSpeed * Time.deltaTime));
                //Debug.DrawLine(positionToGo, myTransform.position, Color.black);    //vykreslení dráhy kam jdu
            }
        }
    }
}