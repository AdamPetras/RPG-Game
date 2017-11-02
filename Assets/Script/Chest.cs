using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(Input.GetKeyDown(KeyCode.A))
        GetComponent<Animation>().Play();
    }
}
