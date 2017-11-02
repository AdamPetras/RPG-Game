using Assets.Script.CharacterFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class ExpBarText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private void Awake()
        {
            gameObject.transform.Find("ExpPanel").gameObject.SetActive(false);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.Find("ExpPanel").gameObject.SetActive(true);

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.Find("ExpPanel").gameObject.SetActive(false);
        }
    }
}