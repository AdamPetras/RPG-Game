using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.CombatFolder;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.StatisticsFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.SpellFolder
{
    public class ComponentSpell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Spell Spell;
        public GameObject SpellInfo;
        private const float INFO_TRESHOLD = 1;
        public bool preCast;
        bool isCasting;
        public bool cantCast;
        Animation castAnimation;
        private PlayerComponent _playerComponent;
        public static List<ComponentSpell> SpellList = new List<ComponentSpell>();
        private GameObject _spellBook;
        void Awake()
        {
        }

        void Start()
        {
            SpellList.Add(this);
            isCasting = false;
            cantCast = false;
            Debug.Log("AA");
            if(!Spell.Unlocked)
                GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            GetComponent<Button>().onClick.AddListener(delegate
            {
                if (Conditions(Spell.ManaCost))
                    StartCoroutine(PreCast());
                else if (!Spell.Unlocked)
                {
                    Spell.Unlock(gameObject);
                }
            });
            _spellBook = GameObject.Find("Graphics").transform.Find("SpellBook").gameObject;
        }

        void Update()
        {
            if (_playerComponent == null)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Player");
                if (obj != null)
                    _playerComponent = obj.GetComponent<PlayerComponent>();
            }
        }

        //bude hlásit že neni volána ale je volána (InvokeRepeating)
        private void CalcPointerOn()
        {
            SpellInfo.transform.Find("Background").position =
                new Vector2(transform.position.x - 24, transform.position.y + 24);
            SpellInfo.SetActive(true);
            CancelInvoke("CalcPointerOn");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (SpellInfo != null && SpellDragAndDrop.SpellDraged == null)
            {
                if (Spell != null)
                    if (Spell.Unlocked)
                        SpellInfo.transform.Find("Background").Find("Desc").GetComponent<Text>().text = Spell.Description;
                InvokeRepeating("CalcPointerOn", 0.5f, 1);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (SpellInfo != null)
            {
                SpellInfo.SetActive(false);
                CancelInvoke("CalcPointerOn");
            }
        }

        public void OnDisable()
        {
            if (SpellInfo != null)
                SpellInfo.SetActive(false);
            CancelInvoke("CalcPointerOn");
        }

        public IEnumerator PreCast()
        {
            if (gameObject.transform.parent != null) //spell slot
            {
                if (gameObject.transform.parent.GetComponent<SpellSlot>().IsBook)
                {
                    HUDSpellBook.CanIDeactive = false;
                }
            }
            while (true)
            {
                //Display Appropriate Indicator Here       
                if (true)//Input.GetMouseButtonDown(0)
                {
                    //Left click confirm
                    StartCoroutine(OnBeginCast());
                    yield break;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    //Right click deny
                    Debug.Log("I don't want to cast!");
                    yield break;
                }
                yield return null;
            }
        }

        IEnumerator OnBeginCast()
        {
            StartCoroutine(Spell.CooldownTimer(0.05f));
            isCasting = true;
            while (isCasting)
            {
                if (cantCast != true)
                {
                    yield return StartCoroutine(OnCast());
                }
            }
            yield return new WaitUntil(() => Spell.CooldownEnable);
            if (gameObject.transform.parent != null) //spell slot
            {
                if (gameObject.transform.parent.GetComponent<SpellSlot>().IsBook)
                {
                    if (!HUDSpellBook.Visible && !HUDSpellBook.CanIDeactive)      //pokud neni visible a nemůžu deaktivovat jak dojde cooldown tak deaktivuju
                        _spellBook.SetActive(false);
                    HUDSpellBook.CanIDeactive = true;
                }
            }
            if (Spell.IWantToDestroy)
            {
                SpellList.Remove(gameObject.GetComponent<ComponentSpell>());
                Destroy(gameObject);
                Spell.IWantToDestroy = false;
            }

        }

        IEnumerator OnCast()
        {
            //castAnimation.Play();
            if (cantCast == true)
            {
                //if Crowd-Controlled
                //castAnimation.Stop();
                isCasting = false;
                yield break;
            }
            yield return new WaitForSeconds(Spell.CastTime);
            castAbility();
            isCasting = false;
        }

        public void castAbility()
        {
            Spell.Cast(gameObject);
            if ((Spell.SpellType == ESpell.Buff || Spell.SpellType == ESpell.Weakness || Spell.SpellType == ESpell.Dot) && Spell.Unlocked)
                StartCoroutine(Spell.AnullBuffDotOrWeakness(1));
        }

        private bool Conditions(float manaCost)
        {
            if (!Spell.Unlocked)
                return false;
            if (!Spell.CooldownEnable)
                return false;
            if (_playerComponent.character.ECharacterState == ECharacterState.Dead)
                return false;
            if (PlayerAttack.Interact != null)
                if (Spell.Range < Vector3.Distance(_playerComponent.transform.position,
                        PlayerAttack.Interact.transform.position))
                {
                    MyDebug.Log("Not enought range.");
                    return false;
                }
            //buff conds
            if (Spell.SpellType == ESpell.Buff || Spell.SpellType == ESpell.Passive)
            {
                Debug.Log(_playerComponent.character.SpellList.Count);
                if (_playerComponent.character.SpellList.Any(s => s.ID == Spell.ID))
                {
                    Debug.Log("You cant set this buff on");
                    MyDebug.Log("You cant set this buff on");
                    return false;
                }
            }
            else if (Spell.SpellType == ESpell.Weakness|| Spell.SpellType == ESpell.Dot)
            {
                if (Spell.PlantedObject == null)
                {
                    MyDebug.Log("Spell plantedObject is NULL");                   
                }
                else
                {
                    if (Spell.PlantedObject.GetComponent<EnemyStatistics>().EnemyCharacter.SpellList
                        .Any(s => s.ID == Spell.ID))
                    {
                        Debug.Log("You cant set this Weakness/Dot on");
                        MyDebug.Log("You cant set this Weakness/Dot on");
                        return false;
                    }                   
                }
                return DamageWeaknessConds();
            }
            else if (Spell.SpellType == ESpell.Damage)
            {
                //damage spells
                return DamageWeaknessConds();
            }
            //mana withdraw
            if (manaCost > _playerComponent.character.GetVital((int)EVital.Mana).CurrentValue)
                return false; //NOT ENOUGH MANA
            _playerComponent.character.GetVital((int)EVital.Mana).CurrentValue -= manaCost;

            return true;
        }

        private bool DamageWeaknessConds()
        {
            GameObject enemyObject = PlayerAttack.Interact;
            if (enemyObject == null)
            {
                MyDebug.Log("No target");
                return false;
            }
            if (enemyObject.tag != "Enemy")
                return false;
            if (enemyObject.GetComponent<EnemyStatistics>().EnemyCharacter.ECharacterState ==
                ECharacterState.Dead)
                return false;
            return true;
        }
    }
}