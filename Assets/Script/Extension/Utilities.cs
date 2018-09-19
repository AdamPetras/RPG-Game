using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Camera;
using Assets.Script.QuestFolder;
using Assets.Script.SpellFolder;
using Assets.Script.StatisticsFolder;
using Assets.Scripts;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.Extension
{
    public class Utilities
    {
        public static bool _dragging;
        private static Vector2 _startDrag;
        private static Vector2 _dragPosition;
        public Utilities()
        {
            _dragging = false;
            _startDrag = Vector2.zero;
            _dragPosition = Vector2.zero;
        }
        public static Rect? DragAndDrop(Rect draggingRect, ref Vector2 position)
        {
            _dragPosition = position;
            Rect screenRect = new Rect(50, 50, Screen.width - 50, Screen.height - 50);
            if (!Input.GetMouseButton(0))
            {
                _dragging = false;
                MyCamera.Enabled = true;
                return null;
            }
            else if (Input.GetMouseButton(0) && draggingRect.Contains(MyInput.CurrentMousePosition()) && !_dragging)
            {
                MyCamera.Enabled = false;
                _dragging = true;
                _startDrag = MyInput.CurrentMousePosition() - _dragPosition;
            }

            if (_dragging)
            {
                _dragPosition = MyInput.CurrentMousePosition() - _startDrag;
                position = _dragPosition;
            }
            Rect returnRect = new Rect(_dragPosition.x, _dragPosition.y, draggingRect.width, draggingRect.height);
            if (screenRect.Overlaps(returnRect))
            {
                return returnRect;
            }
            return new Rect(_dragPosition.x, _dragPosition.y, draggingRect.width, draggingRect.height);
        }

        public static Color ColorByItemRank(ERank rank)
        {
            if (rank == ERank.Common)
                return Color.white;
            if (rank == ERank.Uncommon)
                return Color.yellow;
            if (rank == ERank.Rare)
                return Color.green;
            if (rank == ERank.Epic)
                return Color.red;
            return Color.white;
        }

        public static bool IsDistanceLess(Transform obj1, Transform obj2, float distance)
        {
            return Vector3.Distance(obj1.position, obj2.position) <= distance;
        }

        public static bool IsDistanceBigger(Transform obj1, Transform obj2, float distance)
        {
            return Vector3.Distance(obj1.position, obj2.position) > distance;
        }
        public static void DisableOrEnableAll(GameObject obj,bool state = false)
        {
            MonoBehaviour[] comps = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = state;
            }
        }

        public static Rect CalcDragedRect(Rect rect, Vector2 newPosition)
        {
            rect.position = newPosition;
            return rect;
        }

        public static Transform MouseHit(int numberOfButton, params string[] tags)
        {
            if (Input.GetMouseButtonDown(numberOfButton))
            {
                //Shoot ray from mouse position
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
                if (hit.Any(s => tags.Any(a => s.transform.tag == a)))
                {
                    return hit.Find(s => tags.Equals(s)).transform;
                }
            }
            return null;
        }

        public static void ClearStaticCaches()
        {
            ComponentSpell.SpellList.Clear();
            QuestMasterGenerate.QuestMasterList.Clear();
        }

        public static Vector3 SetPositionToCopyTerrain(Vector3 position)
        {
            position.y = Terrain.activeTerrain.SampleHeight(position);
            return position;
        }
        public static bool IsRayCastHit(Transform transform, params string[] ignoreTransformTag)
        {
            //Shoot ray from mouse position
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
            if (hit.Any(s => s.transform == transform && ignoreTransformTag.Any(a => a != s.transform.tag)))
            {
                return true; //Set that we hit something
            }
            return false;
        }
        public static bool IsRayCastHit(Transform transform)
        {
            //Shoot ray from mouse position
            if (EventSystem.current.IsPointerOverGameObject())
                return false;
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
            if (hit.Any(s => s.transform == transform && s.transform.tag != "Player"))
            {
                return true; //Set that we hit something
            }
            return false;
        }
        public static bool IsFirstRayCastHit(Transform transform)
        {
            //Shoot ray from mouse position
            if (EventSystem.current.IsPointerOverGameObject())
                return false;
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
            if (hit.Count > 0)
            {
                bool first = false;
                foreach (RaycastHit rayHit in hit)
                {                 
                    if(rayHit.transform == transform && !first)
                    {
                        return true; //Set that we hit something
                    }
                    if (rayHit.transform.tag == transform.tag)
                        first = true;
                }
            }
            return false;
        }

        public static bool IsMouseHit(int numberOfButton, params string[] tags)
        {
            if (Input.GetMouseButtonUp(numberOfButton))
            {
                //Shoot ray from mouse position
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
                if (hit.Any(s => tags.Any(a => s.transform.name == a)))
                {
                    return true;
                }
            }
            return false;
        }
        public static Transform MouseHit(Action firstHandle, Action sedondHandle, string[] tags)
        {
            //Shoot ray from mouse position
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            List<RaycastHit> hit = Physics.RaycastAll(ray, 100).ToList();
            if (hit.Any(s => tags.Any(a => s.transform.name == a)))
            {
                firstHandle.Invoke();
                sedondHandle.Invoke();
                return hit.Find(s => tags.Any(a => a == s.transform.tag)).transform;
            }
            return null;
        }
    }
}
