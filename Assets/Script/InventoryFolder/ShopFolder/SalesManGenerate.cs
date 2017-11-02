using System;
using System.Collections.Generic;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using Assets.Script.QuestFolder;
using UnityEngine;

namespace Assets.Script.InventoryFolder.ShopFolder
{
    public class SalesManGenerate:MonoBehaviour
    {
        public static List<ObjectGenerate> SalesManList { get; private set; }
        public EGenerateState EGenerateState { get; private set; }
        void Start()
        {
            SalesManList = new List<ObjectGenerate>();
        }

        void Update()
        {
            switch (EGenerateState)
            {
                case EGenerateState.Init:
                    Initialize();
                    break;
                case EGenerateState.Generate:
                    foreach (ObjectGenerate salesMan in SalesManList)
                    {
                        ObjectGenerate.Generate(new ObjectGenerate(salesMan.Id,salesMan.Name, salesMan.Position, salesMan.Prefab), Instantiate);
                    }
                    EGenerateState = EGenerateState.Idle;
                    break;
            }
        }
        private void Initialize()
        {
            SalesManList.Add(new ObjectGenerate(0, "Aaaaam", new Vector3(22, 0, 27), (GameObject)Resources.Load("Prefab/ArmorSalesMan")));
            SalesManList.Add(new ObjectGenerate(1,"Preedaj", new Vector3(22, 0, 30), (GameObject)Resources.Load("Prefab/ConsumableSalesMan")));
            EGenerateState = EGenerateState.Generate;
        }
    }
}