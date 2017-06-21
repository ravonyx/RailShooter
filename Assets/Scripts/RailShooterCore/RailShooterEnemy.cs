using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailShooterEnemy : MonoBehaviour
{
    [SerializeField]
    private PathWalker m_pathWalker;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(m_pathWalker.PlayUpdateBackward());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
    }
}
