using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tst : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButton("A"))
            Debug.Log(":mon cul");

        Debug.Log("update");
	}
}
