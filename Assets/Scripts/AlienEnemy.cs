using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienEnemy : MonoBehaviour {

	public bool walkByDefault = true; //If we want to walk by default

	public int health=10;
	public float attackRange = 5f;
	public bool dead=false;
	UnityEngine.AI.NavMeshAgent agent;
	private EnemyMove character; //reference to our character movement script
	private Transform cam; //reference to our case
	public GameObject targetedPlayer;
	public Vector3 targetDirection;
	public GameObject myBones;

	Animator anim; //reference to our animator

	WeaponManager weaponManager; //reference to the weapon manager

	//Ik stuff
	[SerializeField] public IK ik;
	[System.Serializable] public class IK
	{
		public Transform spine; //the bone where we rotate the body of our character from
		//The Z/x/y values, doesn't really matter the values here since we ovveride them depending on the weapon
		public float aimingZ = 213.46f; 
		public float aimingX = -65.93f;
		public float aimingY = 20.1f;
		//The point in the ray we do from our camera, basically how far the character looks
		public float point = 30; 

	}
		
	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		//and our Character Movement
		character = GetComponent<EnemyMove>();
		//and our animator
		anim = GetComponent<Animator>();
		targetedPlayer= GameObject.Find ("Player");
		if (agent.isActiveAndEnabled) {
			agent.SetDestination (targetedPlayer.transform.position);
		}
	}
		
	void Update()
	{
		if (agent.isActiveAndEnabled && !dead) {
			if (targetedPlayer != null) {
				if (agent.remainingDistance <= attackRange && !anim.IsInTransition (0)) {// && anim.GetFloat ("Speed") < 0.5f
					//Then attack
					anim.SetBool ("Attack", true);

				} else {
					anim.SetBool ("Attack", false);	
				}
			}
		}
	}

	float GetWalkMultiplier(bool walkToggle)
	{
		//the walk multiplier determines if the character is running or walking
		//if walkByDefault is set and walkToggle is pressed
		float walkMultiplier = 1;
		if(walkByDefault) {
			if(walkToggle) {
				walkMultiplier = 1;
			} else {
				walkMultiplier = 0.5f;
			}
		} else {
			if(walkToggle) {
				walkMultiplier = 0.5f;
			} else {
				walkMultiplier = 1f;
			}
		}
		return walkMultiplier;
	}	

	void FixedUpdate () 
	{
		bool walkToggle = false; //change this variable to alternate between walking and running
		if (!dead) // of the character is alive
		{
			if (targetedPlayer != null) {
				//pass it to our move function from our character movement script
				character.MoveToTarget (targetedPlayer, this.GetWalkMultiplier (walkToggle), agent);
			}
		}
		else //if the character is dead
		{
			targetedPlayer = null;
			if (agent.isActiveAndEnabled) {
				agent.Stop ();
			}
		}
	}
	public void MeleeHit(float punchForce){
		Vector3 direction =  this.transform.position-targetedPlayer.transform.position;
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
		ControleGame.alienDown++;
		ControleGame.pontos += 100;
	}
//	IEnumerator Damage(Vector3 direction,float hitForce){
//		yield return new WaitForSecondsRealtime (0.001f);
//		if(!anim.IsInTransition(0) && health>0){
//			anim.SetBool ("Damage", true);
//			ControleGame.LandedShots++;
//			this.GetComponentInParent<Rigidbody>().AddForce (direction * hitForce);
//			health--;
//		}
//
//		if (health <= 0 && !dead) {
//			anim.SetBool ("Death",true);
//			dead = true;
//			ControleGame.alienDown++;
//			ControleGame.pontos += 100;
//		}
//	}
}
