using UnityEngine;

namespace Assets.Script.InventoryFolder.CraftFolder
{
    public class ComponentCraftMenu : MonoBehaviour
    {

        private CraftSettings _craftSettings;
        public static bool Visible;
        public static bool CanIDeactive;

        public static bool IsNearAnvil;
        public static bool IsNearRange;
        public static bool IsNearCraftTable;
        public static bool IsNearTailorKit;
        // Use this for initialization
        void Start()
        {
            IsNearAnvil = false;
            IsNearCraftTable = false;
            IsNearRange = false;
            IsNearTailorKit = false;
            _craftSettings = new CraftSettings(GameObject.Find("Graphics").transform.Find("CraftMenu").gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            _craftSettings.Update();
        }

        public void OnVisible()
        {
            _craftSettings.OnVisible();
        }

        public void OnHide()
        {
            _craftSettings.OnHide();
        }
    }
}
