using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    public class DamageHud : MonoBehaviour
    {
        private static readonly List<GameObject> Signs = new List<GameObject>();
        private static Transform Background;

        private void Start()
        {
            Background = gameObject.transform.GetChild(0);
        }

        public static void Hit(bool isPlayer, float damageDone)
        {
            GameObject obj;
            if (isPlayer)
               obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/PlayerHit"), Background);
            else obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/EnemyHit"), Background);
            obj.GetComponent<Text>().text = Math.Round(damageDone, 1).ToString();
            Signs.Add(obj);
        }
    }
}