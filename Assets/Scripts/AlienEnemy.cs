using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienEnemy : MonoBehaviour {

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
	public GameObject myBones;

	Animator anim; //reference to our animator

	WeaponManager weaponManager; //reference to the weapon manager

	//Ik stuff
	[SerializeField] public IK ik;
	[System.Serializable] public class IK
	{
		public Transform spine; //the bone where we rotate the body of our character from
	}
		
	// Use this for initialization
	void Start ()
	{
		//and our Character Movement
		character = GetComponent<Movement.EnemyMove>();
		//and our animator
		anim = GetComponent<Animator>();
		currentTarget = GameObject.Find ("Player");
	}
		
	void Update()
	{	
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

	float GetWalkMultiplier()
	{
		if (runToggle) {
			return runSpeed;
		}
		return walkSpeed;
	}	

	void FixedUpdate () 
	{
		if (!dead) // of the character is alive
		{
			if (currentTarget != null && distanceToTarget > attackRange) { //if we have a target and we are out of the attack range we should go after it
				//pass it to our move function from our character movement script
				character.MoveToTarget (currentTarget, this.GetWalkMultiplier ());
			} else {
				character.TurnAtTarget(currentTarget); // stop and just face the target
			}
		}
		else //if the character is dead
		{
			//clear targets
			currentTarget = null;
		}
	}
	public void MeleeHit(float punchForce){
		Vector3 direction =  this.transform.position-currentTarget.transform.position;
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
		Game.Gobals.alienDown++;
		Game.Gobals.pontos += 100;
	}

}
