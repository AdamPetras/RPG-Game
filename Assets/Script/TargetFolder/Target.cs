using System;
using Assets.Script.CharacterFolder;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder.ShopFolder;
using Assets.Script.QuestFolder;
using Assets.Script.StatisticsFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.TargetFolder
{
    public enum ETarget
    {
        Enemy,
        SalesMan,
        QuestMaster,
        None
    }

    public class Target : MonoBehaviour
    {
        public GameObject TargetPrefab;
        public GameObject LevelPrefab;
        private Transform _background;
        private const int MAXBARLENGHT = 134;
        private const float targetTreshold = 0.15f;
        private float targetTimer;
        public bool IsPlayer;

        private void Awake()
        {
            TargetPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/TargetPrefab"), transform);
            TargetPrefab.name = "TargetPrefab";
            LevelPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/LevelPrefab"),
                TargetPrefab.transform);
            _background = TargetPrefab.transform.Find("Background");
            if (!IsPlayer)
            {
                _background.position = new Vector3(350, -40);
                LevelPrefab.transform.position = new Vector3(440, -70);
            }
            else
            {
                _background.position = new Vector3(50, 15);
                LevelPrefab.transform.position = new Vector3(140, -20);
                InvokeRepeating("TargetSelect", 0, 0.2f);
            }
        }

        private void Start()
        {

        }

        private void Update()
        {

            if (IsPlayer)
                return;
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !EventSystem.current.IsPointerOverGameObject())
            {
                targetTimer += Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Utilities.IsFirstRayCastHit(transform) && targetTimer < targetTreshold)
                {
                    if (PlayerAttack.Interact == gameObject)
                        PlayerAttack.Interact = null;
                    InvokeRepeating("TargetSelect", 0, 0.2f);
                    gameObject.transform.Find("Projector").GetComponent<Projector>().enabled = true;
                }
                else if (targetTimer < targetTreshold)
                {
                    if (PlayerAttack.Interact == gameObject)
                        PlayerAttack.Interact = null;
                    CancelInvoke("TargetSelect");
                    TargetPrefab.GetComponent<Canvas>().enabled = false;
                    transform.Find("Projector").GetComponent<Projector>().enabled = false;
                    transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
                    transform.Find("Projector").GetComponent<Projector>().material.color = Color.green;
                }
                targetTimer = 0;
            }
            else if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Utilities.IsFirstRayCastHit(transform) && targetTimer < targetTreshold)
                {
                    PlayerAttack.Interact = gameObject;
                    gameObject.transform.Find("Projector").GetComponent<Projector>().enabled = true;
                    gameObject.transform.Find("Projector").GetComponent<Projector>().material.color = Color.red;
                    InvokeRepeating("TargetSelect", 0, 0.2f);
                }
                else if (targetTimer < targetTreshold)
                {
                    if (PlayerAttack.Interact == gameObject)
                        PlayerAttack.Interact = null;
                    CancelInvoke("TargetSelect");
                    TargetPrefab.GetComponent<Canvas>().enabled = false;
                    transform.Find("Projector").GetComponent<Projector>().enabled = false;
                    transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
                }
                targetTimer = 0;
            }
        }

        public void TargetSelect()
        {
            if (IsPlayer)
            {
                PlayerComponent playerComponent = gameObject.GetComponent<PlayerComponent>();
                OnChangeBarSize(playerComponent.character.GetVital((int)EVital.Health).CurrentValue,
                    playerComponent.character.GetVital((int)EVital.Health).MaxValue, true);
                OnChangeBarSize(playerComponent.character.GetVital((int)EVital.Mana).CurrentValue,
                    playerComponent.character.GetVital((int)EVital.Mana).MaxValue, false);
                LevelPrefab.transform.Find("Text").GetComponent<Text>().text = playerComponent.Level.ToString();
            }
            else
            {
                TargetPrefab.GetComponent<Canvas>().enabled = true;
                ETarget target = (ETarget)Enum.Parse(typeof(ETarget), gameObject.tag);
                if (target == ETarget.Enemy)
                {
                    EnemyStatistics stats = GetComponent<EnemyStatistics>();
                    OnChangeBarSize(stats.EnemyCharacter.GetVital((int)EVital.Health).CurrentValue,
                        stats.EnemyCharacter.GetVital((int)EVital.Health).MaxValue, true);
                    OnChangeBarSize(stats.EnemyCharacter.GetVital((int)EVital.Mana).CurrentValue,
                        stats.EnemyCharacter.GetVital((int)EVital.Mana).MaxValue, false);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                }
                else if (target == ETarget.QuestMaster)
                {
                    QuestMasterObject stats = GetComponent<QuestMasterObject>();
                    OnChangeBarSize(stats.Health, stats.Health, true);
                    OnChangeBarSize(stats.Mana, stats.Mana, false);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                }
                else if (target == ETarget.SalesMan)
                {
                    ComponentSalesMan stats = GetComponent<ComponentSalesMan>();
                    OnChangeBarSize(stats.Health, stats.Health, true);
                    OnChangeBarSize(stats.Mana, stats.Mana, false);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                }
                transform.Find("Name").GetComponent<MeshRenderer>().enabled = true;
                transform.Find("Name").GetComponent<TextMesh>().text = name;
                transform.Find("Name").transform.rotation = Quaternion.Slerp(transform.Find("Name").transform.rotation, Quaternion.LookRotation(transform.Find("Name").transform.position - UnityEngine.Camera.main.transform.position), 10);
                _background.Find("Name").GetComponent<Text>().text = name;

            }
        }

        private void OnChangeBarSize(float currSize, float maxSize, bool isHealth)
        {

            if (isHealth)
            {
                SelectBar(currSize, maxSize, "Health");
            }
            else
            {
                SelectBar(currSize, maxSize, "Mana");
            }
        }

        public void SelectBar(float currSize, float maxSize, string s)
        {
            float currBarLenght = (int)((currSize / maxSize) * MAXBARLENGHT);
            _background.Find(s).GetComponent<RectTransform>().sizeDelta = new Vector2(currBarLenght, _background.Find(s).GetComponent<RectTransform>().sizeDelta.y);
            _background.Find(s + "Text").GetComponent<Text>().text = Math.Round(currSize, 1) + "/" + Math.Round(maxSize, 1);
        }
    }
}