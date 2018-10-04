using System;
using Assets.Script.CharacterFolder;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.InventoryFolder
{
    public class SaleSlot : MonoBehaviour, IPointerClickHandler
    {
        public int Id { get; set; }
        public static GameObject StackAmount;
        private static PlayerComponent _playerComponent;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_playerComponent == null)
                    _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                NewItem item = new NewItem(NewItem.IdToItem(Id));
                if (_playerComponent.Money >= item.BuyPrice)
                {
                    if (StackAmount == null)
                    {
                        if (!SlotManagement.AddToInventory(item))
                        {
                            Debug.LogWarning("Inventory is full");
                            return;
                        }
                        _playerComponent.Money -= item.BuyPrice;
                    }
                }
            }
        }

     /*  private void OnRepairInputValue(GameObject stackAmount, NewItem item)
        {
            int stack;
            Int32.TryParse(stackAmount.transform.Find("Background").Find("InputField").GetComponent<InputField>().text, out stack);
            if (stack > item.MaximumStack)
                stack = item.MaximumStack;
            stackAmount.transform.Find("Background").Find("InputField").GetComponent<InputField>().text = stack.ToString();
        }*/

        private void OnStackAmount(GameObject stackAmount, NewItem item)
        {
            int stack;
            Int32.TryParse(stackAmount.transform.Find("Background").Find("InputField").GetComponent<InputField>().text, out stack);
            Destroy(stackAmount);
            if (stack <= 0)
            {
                return;
            }
            item.ActualStack = stack;
            if (!SlotManagement.AddToInventory(item))
            {
                Debug.LogWarning("Inventory is full");
                return;
            }
        }
    }
}
