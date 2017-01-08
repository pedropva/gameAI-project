using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemie : MonoBehaviour {
	public bool hit;
	PlayerController controller;
	public GameObject character;
	Animator anim; //reference to our animator
	private AnimatorStateInfo currentState;	
	private AnimatorStateInfo previousState;	
	// Use this for initialization
	void Start () 
	{
		controller = character.GetComponent<PlayerController> ();
		anim = GetComponent<Animator>(); //setup the refence to the animator

		currentState = anim.GetCurrentAnimatorStateInfo (0);
		previousState = currentState;
	}

	void Update ()
	{
		if(hit)
		{
			anim.SetBool ("Next", true);
			hit = false;
		}
		if (anim.GetBool ("Next")) {
			// 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す
			currentState = anim.GetCurrentAnimatorStateInfo (0);
			if (previousState.nameHash != currentState.nameHash) {
				anim.SetBool ("Next", false);
				previousState = currentState;				
			}
		}
	}
}
