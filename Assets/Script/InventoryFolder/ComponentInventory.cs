using System.Collections.Generic;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.InventoryFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.InventoryFolder
{
    public class ComponentInventory : MonoBehaviour
    {
        public List<ComponentItem> InventoryList;
        public List<GameObject> InventorySlotList;
        private GameObject _playerGameObject;
        private PlayerComponent _playerComponent;
        public static bool Visible;
        //******************canvas***********************
        private GameObject _inventory;
        private Transform _backgroundTransform;
        //***********************************************

        // Use this for initialization
        public GameObject GraphicsPanel;
        void Start()
        {
            InventoryList = new List<ComponentItem>();
            InventorySlotList = new List<GameObject>();

            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (_playerGameObject == null)
            {
                Debug.LogWarning("#001 Player gamebject is null [ComponentInventory]");
                MyDebug.LogWarning("#001 Player gamebject is null [ComponentInventory]");
            }
            else
            {
                _playerComponent = _playerGameObject.GetComponent<PlayerComponent>();
                new SlotManagement(_playerComponent.character);
                _inventory = Instantiate(Resources.Load<GameObject>("Prefab/InventoryPrefab"), GraphicsPanel.transform);
                _inventory.name = "Inventory";
                _backgroundTransform = _inventory.transform.Find("Background");
                new InventorySettings(_backgroundTransform);
                //Debug.Log("inventory size " + InventorySettings.InventoryList.Count);
            }
            _inventory.SetActive(false);
            Utilities.DisableOrEnableAll(_inventory);
            Debug.Log(InventorySlotList.Count);
        }

        // Update is called once per frame
        void Update()
        {
            if (_inventory == null)
                return;
            if (Input.GetKeyUp(KeyCode.I) && !_inventory.activeSelf)
            {
                _inventory.SetActive(true);
                Utilities.DisableOrEnableAll(_inventory, true);
            }
            else if (Input.GetKeyUp(KeyCode.I) && _inventory.activeSelf)
            {
                _inventory.SetActive(false);
                Utilities.DisableOrEnableAll(_inventory);
            }
        }
    }
}
