using System;
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

        public static void QuestMarksGenerate(ObjectGenerate generatedObject, GameObject gameObject, Func<GameObject, Vector3, Quaternion, GameObject> Instantiate)
        {
            Vector3 position = generatedObject.Position;
            position.y = Terrain.activeTerrain.SampleHeight(position) + Terrain.activeTerrain.GetPosition().y;  //nastavení Y na hodnotu terénu
            position.y += generatedObject.Prefab.GetComponent<BoxCollider>().size.y / 2;
            GameObject qMark = Instantiate(Resources.Load("Prefab/qMark") as GameObject, new Vector3(position.x, position.y + gameObject.GetComponent<Renderer>().bounds.size.y, position.z), Quaternion.identity) as GameObject;
            qMark.name = "qMark";
            qMark.transform.SetParent(gameObject.transform);
            GameObject exMark = Instantiate(Resources.Load("Prefab/exMark") as GameObject, new Vector3(position.x, position.y + gameObject.GetComponent<Renderer>().bounds.size.y, position.z), Quaternion.identity) as GameObject;
            exMark.name = "exMark";
            exMark.transform.SetParent(gameObject.transform);
            qMark.GetComponent<MeshRenderer>().enabled = false;
            exMark.GetComponent<MeshRenderer>().enabled = false;
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