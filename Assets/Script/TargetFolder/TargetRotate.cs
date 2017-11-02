using UnityEngine;

namespace Assets.Script
{
    public class TargetRotate:MonoBehaviour
    {
        void Update()
        {
            if(GetComponent<Projector>().enabled)
                transform.Rotate(0,0, Time.deltaTime * 100);
        }
    }
}