using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.SpellFolder
{
    public class SpellDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static GameObject SpellDraged;
        private Transform _startParent;
        private Transform DragPanel;

        private void Start()
        {
            DragPanel = GameObject.Find("DragPanel").transform.Find("Background");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {  
            if (!gameObject.GetComponent<ComponentSpell>().Spell.Unlocked)
            {
                return;
            }
            _startParent = transform.parent;
            if (transform.parent.GetComponent<SpellSlot>() != null)
                if (!transform.parent.GetComponent<SpellSlot>().IsBook)
                {
                    transform.parent.GetComponent<SpellSlot>().Occupied = false;
                    SpellDraged = gameObject;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else if (transform.parent.GetComponent<SpellSlot>().IsBook)
                {
                    
                    GameObject newObj = Instantiate(gameObject, _startParent);
                    newObj.name = "Spell";
                    newObj.GetComponent<ComponentSpell>().Spell = gameObject.GetComponent<ComponentSpell>().Spell;
                    if (!newObj.GetComponent<ComponentSpell>().Spell.CooldownEnable)
                    {
                        newObj.transform.Find("Cooldown").GetComponent<Image>().fillAmount = 0;
                    }
                    newObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    SpellDraged = newObj;
                    
                }         
            SpellDraged.transform.SetParent(DragPanel); //nastavení na drag panel aby šlo vidět jak se přetahuje
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!gameObject.GetComponent<ComponentSpell>().Spell.Unlocked)
            {
                return;
            }
            SpellDraged.transform.position = Input.mousePosition;            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!gameObject.GetComponent<ComponentSpell>().Spell.Unlocked)
            {
                return;
            }
            SpellDraged.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (SpellDraged.transform.parent == DragPanel)
            {
                ComponentSpell.SpellList.Remove(SpellDraged.GetComponent<ComponentSpell>());
                Destroy(SpellDraged.gameObject);
            }
            SpellDraged = null;
        }
    }
}