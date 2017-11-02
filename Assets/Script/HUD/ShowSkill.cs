using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.HUD
{
    public class ShowSkill:MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {            
            Transform transParent = transform.parent;
            transParent.Find("MaxExp").gameObject.SetActive(true);
            transParent.Find("MinExp").gameObject.SetActive(true);
            transParent.Find("CurrentExp").gameObject.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Transform transParent = transform.parent;
            transParent.Find("MaxExp").gameObject.SetActive(false);
            transParent.Find("MinExp").gameObject.SetActive(false);
            transParent.Find("CurrentExp").gameObject.SetActive(false);
        }
    }
}