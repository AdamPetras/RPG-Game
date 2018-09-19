using System;
using System.Collections.Generic;
using System.Linq;
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
        private enum Bars
        {
            Health,
            Mana,
            Energy
        }

        private void Awake()
        {      
            if (IsPlayer)
            {
                TargetPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/PlayerTargetPrefab"),
                    transform);
                TargetPrefab.name = "PlayerTargetPrefab";
                _background = TargetPrefab.transform.Find("Background");
                LevelPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/LevelPrefab"), TargetPrefab.transform);
                LevelPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(260, -80);
                InvokeRepeating("TargetSelect", 0, 0.2f);
            }
            else
            {
                TargetPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/TargetPrefab"), transform);
                TargetPrefab.name = "TargetPrefab";
                _background = TargetPrefab.transform.Find("Background");
                LevelPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/LevelPrefab"), TargetPrefab.transform);
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
                //if (targetTimer < targetTreshold)
                {
                    StopTarget();
                    transform.Find("Projector").GetComponent<Projector>().material.color = Color.green;
                }
                if (Utilities.IsFirstRayCastHit(transform)/* && targetTimer < targetTreshold*/)
                {
                    if (PlayerAttack.Interact == gameObject)
                        PlayerAttack.Interact = null;
                    StartTarget();
                }             
                targetTimer = 0;
            }
            else if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Utilities.IsFirstRayCastHit(transform) && targetTimer < targetTreshold)
                {
                    PlayerAttack.Interact = gameObject;
                    StartTarget();
                    gameObject.transform.Find("Projector").GetComponent<Projector>().material.color = Color.red;
                   
                }
                else if (targetTimer < targetTreshold)
                {
                    StopTarget();
                }
                targetTimer = 0;
            }
        }

        private void StartTarget()
        {
            gameObject.transform.Find("Projector").GetComponent<Projector>().enabled = true;
            InvokeRepeating("TargetSelect", 0, 0.2f);
        }

        private void StopTarget()
        {
            if (PlayerAttack.Interact == gameObject)
                PlayerAttack.Interact = null;
            CancelInvoke("TargetSelect");
            TargetPrefab.GetComponent<Canvas>().enabled = false;
            if (!IsPlayer)
            {
                if (GetComponent<EnemyStatistics>() != null)
                    if (GetComponent<EnemyStatistics>().EnemyCharacter.BuffObj != null)
                        Destroy(GetComponent<EnemyStatistics>().EnemyCharacter.BuffObj);
            }
            transform.Find("Projector").GetComponent<Projector>().enabled = false;
            transform.Find("Name").GetComponent<MeshRenderer>().enabled = false;
        }

        public void TargetSelect()
        {
            if (IsPlayer)
            {
                PlayerComponent playerComponent = gameObject.GetComponent<PlayerComponent>();
                OnChangeBarSize(playerComponent.character.GetVital((int)EVital.Health).CurrentValue,
                    playerComponent.character.GetVital((int)EVital.Health).MaxValue, Bars.Health);
                OnChangeBarSize(playerComponent.character.GetVital((int)EVital.Mana).CurrentValue,
                    playerComponent.character.GetVital((int)EVital.Mana).MaxValue, Bars.Mana);            
                OnChangeBarSize(playerComponent.character.GetVital((int)EVital.Energy).CurrentValue,
                    playerComponent.character.GetVital((int)EVital.Energy).MaxValue, Bars.Energy);
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
                        stats.EnemyCharacter.GetVital((int)EVital.Health).MaxValue, Bars.Health);
                    OnChangeBarSize(stats.EnemyCharacter.GetVital((int)EVital.Mana).CurrentValue,
                        stats.EnemyCharacter.GetVital((int)EVital.Mana).MaxValue, Bars.Mana);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                    if (stats.EnemyCharacter.BuffObj == null)
                    {
                        stats.EnemyCharacter.BuffObj =
                            GameObject.Instantiate(Resources.Load<GameObject>("Prefab/EnemyBuffPanel"));
                        stats.EnemyCharacter.BuffObj.name = "EnemyBuffPanel";
                        stats.EnemyCharacter.BuffPanel = stats.EnemyCharacter.BuffObj.transform.Find("Background");
                    }
                }
                else if (target == ETarget.QuestMaster)
                {
                    QuestMasterObject stats = GetComponent<QuestMasterObject>();
                    OnChangeBarSize(stats.Health, stats.Health, Bars.Health);
                    OnChangeBarSize(stats.Mana, stats.Mana, Bars.Mana);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                }
                else if (target == ETarget.SalesMan)
                {
                    ComponentSalesMan stats = GetComponent<ComponentSalesMan>();
                    OnChangeBarSize(stats.Health, stats.Health, Bars.Health);
                    OnChangeBarSize(stats.Mana, stats.Mana, Bars.Mana);
                    LevelPrefab.transform.Find("Text").GetComponent<Text>().text = stats.Level.ToString();
                }
                transform.Find("Name").GetComponent<MeshRenderer>().enabled = true;
                transform.Find("Name").GetComponent<TextMesh>().text = name;
                transform.Find("Name").transform.rotation = Quaternion.Slerp(transform.Find("Name").transform.rotation, Quaternion.LookRotation(transform.Find("Name").transform.position - UnityEngine.Camera.main.transform.position), 10);
                _background.Find("Name").GetComponent<Text>().text = name;

            }
        }

        private void OnChangeBarSize(float currSize, float maxSize, Bars eBar)
        {

            if (eBar == Bars.Health)
            {
                SelectBar(currSize, maxSize, "Health");
            }
            else if(eBar == Bars.Mana)
            {
                SelectBar(currSize, maxSize, "Mana");
            }
            else if (eBar == Bars.Energy)
            {
                SelectBar(currSize, maxSize, "Energy");
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