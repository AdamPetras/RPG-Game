using System;
using System.Collections.Generic;
using Assets.Script.Camera;
using Assets.Script.CharacterFolder;
using UnityEngine;

namespace Assets.Script.Extension
{
    public class ObjectGenerate
    {
        public GameObject Prefab;
        public Vector3 Position;
        public string Name;
        public int Id;
        public ObjectGenerate(int id, string name, Vector3 position, GameObject prefab)
        {
            Id = id;
            Name = name;
            Position = position;
            Prefab = prefab;
        }

        public static void Generate(ObjectGenerate gameObject, Func<GameObject, Vector3, Quaternion, GameObject> Instantiate)
        {
            Vector3 position = gameObject.Position;
            position.y = Terrain.activeTerrain.SampleHeight(position) + Terrain.activeTerrain.GetPosition().y;  //nastavení Y na hodnotu terénu
            position.y += gameObject.Prefab.GetComponent<BoxCollider>().size.y / 2;
            GameObject enemyObject = Instantiate(gameObject.Prefab, position, Quaternion.identity) as GameObject; //vytvoření objektu a instanciování
            enemyObject.name = gameObject.Name;
        }

        public static GameObject CharacterInstantiate(GameObject characterObj, Vector3 position)
        {
            GameObject playerCharacter;
            try
            {
                playerCharacter = GameObject.Instantiate(characterObj, position, Quaternion.identity);
                playerCharacter.GetComponent<CharacterController>().enabled = true;
                playerCharacter.GetComponent<ThirdPersonCamera>().enabled = true;
                playerCharacter.GetComponent<PlayerComponent>().Created = true;
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning("Character instantiate failed");
                return null;
            }
            return playerCharacter;
        }

        public static bool IsDestroyed(GameObject gameObject)
        {
            return gameObject == null && !ReferenceEquals(gameObject, null);
        }
    }
}