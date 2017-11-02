using System.Collections.Generic;
using System.Linq;
using Assets.Script.Enemy;
using Assets.Script.Extension;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public class QuestMasterGenerate : MonoBehaviour
    {
        public static List<ObjectGenerate> QuestMasterList { get; private set; }
        public EGenerateState EGenerateState { get; private set; }

        void Awake()
        {
            QuestMasterList = new List<ObjectGenerate>();
        }

        void Update()
        {
            switch (EGenerateState)
            {
                case EGenerateState.Init:
                    Initialize();
                    break;
                case EGenerateState.Generate:
                    foreach (ObjectGenerate QuestMaster in QuestMasterList)
                    {
                        ObjectGenerate.Generate(QuestMaster,Instantiate);
                        ObjectGenerate.QuestMarksGenerate(QuestMaster, GameObject.FindGameObjectsWithTag("QuestMaster").First(s=>s.name == QuestMaster.Name), Instantiate);
                    }      
                    EGenerateState = EGenerateState.Idle;
                    break;
            }
        }
        private void Initialize()
        {
            QuestMasterList.Add(new ObjectGenerate(1, "Adam",new Vector3(26, 0, 27), (GameObject)Resources.Load("Prefab/QuestMaster")));
            QuestMasterList.Add(new ObjectGenerate(2, "dam", new Vector3(26, 0, 30), (GameObject)Resources.Load("Prefab/QuestMaster")));
            QuestMasterList.Add(new ObjectGenerate(3, "Patrick", new Vector3(20, 0, 30), (GameObject)Resources.Load("Prefab/QuestMaster")));
            EGenerateState = EGenerateState.Generate;
        }


    }
}
