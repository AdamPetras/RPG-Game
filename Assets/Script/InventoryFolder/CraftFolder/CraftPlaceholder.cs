using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Script.InventoryFolder.CraftFolder
{
    public class CraftPlaceholder:MonoBehaviour
    {
        public EProfession Type;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                TypeSwitch(true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                TypeSwitch(false);
            }
        }

        private void TypeSwitch(bool trueOrFalse)
        {
            switch (Type)
            {
                case EProfession.Smithing:
                    ComponentCraftMenu.IsNearAnvil = trueOrFalse;
                    break;
                case EProfession.Cooking:
                    ComponentCraftMenu.IsNearRange = trueOrFalse;
                    break;
                case EProfession.Crafting:
                    ComponentCraftMenu.IsNearCraftTable = trueOrFalse;
                    break;
                case EProfession.Tailoring:
                    ComponentCraftMenu.IsNearTailorKit = trueOrFalse;
                    break;
            }
        }
    }
}