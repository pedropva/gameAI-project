﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlien : MonoBehaviour {

	public bool runToggle = true; //change this variable to alternate between walking and running

	public int health=10;
	public float walkSpeed=10f;
	public float runSpeed=100f;
	public float attackRange = 1f;
	public bool dead=false;
	public float distanceToTarget;
	private Movement.EnemyMove character; //reference to our character movement script
	private Transform cam; //reference to our case
	public GameObject currentTarget;
	public Vector3 targetDirection;
	public Animator anim; //reference to our animator

		
	// Use this for initialization
	void Start ()
	{
		//and our Character Movement
		character = GetComponent<Movement.EnemyMove>();
		//and our animator
		anim = GetComponent<Animator>();
		currentTarget = GameObject.Find ("Player");
		//start the moving thing
		StartCoroutine (Movement.EnemyMove.targetingCoroutine (this,character));
	}
		
	void Update()
	{	
		if (anim.IsInTransition (0)) {
			//if (animator.GetCurrentAnimatorStateInfo (0).Equals ("Damage")) {
			anim.SetBool ("Damage", false);
			anim.SetBool ("Attack", false);
		}
		if (this.transform.position.y < -5f) {
			teleportBackToMap ();
		}
		if (currentTarget != null && currentTarget.GetComponent<UserInput> ().health == 0) {
			currentTarget = null;
		}
		if (currentTarget != null && !dead) {
			distanceToTarget = Vector3.Distance (currentTarget.transform.position, this.transform.position);
			if (distanceToTarget <= attackRange && !anim.IsInTransition (0)) {// && anim.GetFloat ("Speed") < 0.5f
				//Then attack
				anim.SetBool ("Attack", true);
			} else {
				anim.SetBool ("Attack", false);	
			}
		}
	}
	public void teleportBackToMap(){
		Vector3 targetPos = currentTarget.transform.position;
		Vector3 newPos = new Vector3 (Random.Range (targetPos.x + 50f, targetPos.x - 50f), targetPos.y, Random.Range (targetPos.z + 50f, targetPos.z - 50f));
		this.transform.position = newPos;
	}

	public float GetWalkMultiplier()
	{
		if (runToggle) {
			return runSpeed;
		}
		return walkSpeed;
	}	

	void FixedUpdate () 
	{
		
	}
	public void MeleeHit(float punchForce){
		Vector3 direction = Vector3.Scale((this.transform.position-currentTarget.transform.position), new Vector3(1f,0f,1f));
		direction = direction.normalized;
		this.Hit (direction, punchForce);

	}
	public void Hit(Vector3 direction,float hitForce){
		if(health>0  && !dead){//!anim.IsInTransition(0) &&
			anim.SetBool ("Damage", true);
			this.GetComponentInParent<Rigidbody>().AddForce (direction * hitForce);
			health--;
		}

		if (health <= 0 && !dead && !anim.IsInTransition(0)) {
			Die ();
		}
	}
	public void Die()
	{
		anim.SetBool ("Attack", false);
		anim.SetBool ("Damage", false);
		anim.SetBool ("Death",true);
		dead = true;
		//clear targets
		currentTarget = null;
		Game.Gobals.alienDown++;
		Game.Gobals.pontos += 100;
	}

}
