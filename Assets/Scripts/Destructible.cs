using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public bool hit;
	PlayerController controller;
	public GameObject character;


	// Use this for initialization
	void Start () 
	{
		controller = character.GetComponent<PlayerController> ();

	}

	void Update ()
	{
		if(hit)
		{
			
		}
	}
}
