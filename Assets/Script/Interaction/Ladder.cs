using System.Collections.Generic;
using System.Linq;
using Assets.Script.HUD;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Interaction
{
    public class Ladder:MonoBehaviour
    {
        public enum ELadder
        {
            BottomTrigger,
            TopTrigger
        }
        [SerializeField]
        public ELadder ELadderState;
        private void Start()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if(ELadderState == ELadder.BottomTrigger)
                Information.SetText("Stisknutím klávesy F vylezete.");
            else if(ELadderState == ELadder.TopTrigger)
                Information.SetText("Stisknutím klávesy F slezete.");
        }

        void OnTriggerExit(Collider other)
        {
            Information.SetText("");
        }

        void OnTriggerStay(Collider other)
        {
            Debug.Log(GetComponent<BoxCollider>().center);
            if (Input.GetKeyUp(KeyCode.F) && !BlackScreen.Visible)
            {
                BlackScreen.Print(0.5f);
                if (ELadderState == ELadder.BottomTrigger)
                {
                    Debug.Log("Up");
                    GameObject.FindGameObjectWithTag("Player").transform.position =
                        gameObject.transform.parent.GetComponent<BoxCollider>().bounds.max;
                }
                else if (ELadderState == ELadder.TopTrigger)
                {
                    Debug.Log("Down");
                    GameObject.FindGameObjectWithTag("Player").transform.position = gameObject.transform.parent
                        .GetComponent<BoxCollider>().bounds.min;
                }
                
            }
        }
    }
}