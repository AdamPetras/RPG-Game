using UnityEngine;

namespace Assets.Script
{
    public class DayNightCycle : MonoBehaviour
    {
        private void Start()
        {

        }

        private void Update()
        {
            transform.RotateAround(Vector3.zero, Vector3.right, 10f * Time.deltaTime);
            transform.LookAt(Vector3.zero);
        }
    }
}