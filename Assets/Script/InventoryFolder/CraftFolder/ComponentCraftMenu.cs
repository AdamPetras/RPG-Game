using UnityEngine;

namespace Assets.Script.InventoryFolder.CraftFolder
{
    public class ComponentCraftMenu : MonoBehaviour
    {

        private CraftSettings _craftSettings;
        public static bool Visible;
        public static bool CanIDeactive;
        // Use this for initialization
        void Start()
        {
            _craftSettings = new CraftSettings(GameObject.Find("CraftMenu"));
        }

        // Update is called once per frame
        void Update()
        {
            _craftSettings.Update();
        }
    }
}
