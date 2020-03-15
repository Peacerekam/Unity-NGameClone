using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour {

    [SerializeField]
    private GameObject playerObject;
    
	void Start ()
    {
        SpawnPlayer(transform.position);
	}

    public void SpawnPlayer(Vector3 pos)
    {
        transform.position = pos;
        playerObject.transform.position = pos;

        playerObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

}
