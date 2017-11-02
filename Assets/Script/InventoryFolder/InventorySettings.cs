using System.Collections.Generic;
using System.Linq;
using Assets.Script.InventoryFolder;
using UnityEngine;

namespace Assets.Scripts.InventoryFolder
{
    public class InventorySettings
    {
        private const int row = 6;
        private const int column = 5;
        public InventorySettings(Transform background)
        {
            int counter = 0;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                {
                    GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefab/InventoryItem"),
                        new Vector3(29 + j * 48,
                            -29- i * 48, 0), Quaternion.identity);
                    obj.name = "InventoryItem" +counter++;
                    obj.transform.SetParent(background,false);
                    obj.transform.localScale = Vector3.one;
                    GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventorySlotList.Add(obj);
                }
        }
    }
}
