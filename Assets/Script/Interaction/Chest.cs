using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Enemy;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder;

public class Chest : MonoBehaviour
{
    public List<DropItem> DropItems;
    public int DropRespawnTime;

    private bool _enter; 
    private GameObject _dropObject;
    private bool _canIRespawn;
    // Use this for initialization
    void Start ()
    {
        _enter = false;
        _dropObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DropPrefab"));
        _dropObject.transform.SetParent(transform); 
        if (DropItems != null)
           _dropObject.GetComponent<Drop>().AddItems(DropItems.ToArray());
        _dropObject.GetComponent<Drop>().DropClickCollider = GetComponent<BoxCollider>();
        _canIRespawn = false;
    }

    // Update is called once per frame
	void Update ()
	{
	    if (_enter)
	    {
	        if (Input.GetKeyUp(KeyCode.F))
            {               
	            if (_dropObject != null)
	            {
	                _dropObject.GetComponent<Drop>().Show();
                }
	        }
	    }
    }
    private void OnTriggerEnter(Collider other)
    {
        Information.SetText("Stisknutím klávesy F otevřete truhlu.");
        _enter = true;   
    }
    private void OnTriggerExit(Collider other)
    {
        Information.SetText("");
        _enter = false;
    }

    public IEnumerator Respawn(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            _canIRespawn = true;
        }
    }
}
