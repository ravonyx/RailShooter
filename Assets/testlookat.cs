using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

public class testlookat : MonoBehaviour {


    [SerializeField]
    CamerasAndInputsManager m_cameraInputManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(m_cameraInputManager.CurrentCamera.transform);
        transform.Rotate(0, 180, 0);
	}
}
