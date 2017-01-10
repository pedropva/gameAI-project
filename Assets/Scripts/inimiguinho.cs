using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inimiguinho : MonoBehaviour {

	public bool walkByDefault = true; //If we want to walk by default

	public int health=10;

	private EnemyMove character; //reference to our character movement script
	private Transform cam; //reference to our case
	public Vector3 move; //our move vector
	public GameObject targetedPlayer;
	public bool aim; //if we are aiming
	public float aimingWeight; //the aiming weight, helps with IK
	public GameObject camera;
	public float camDistanceNormal=-0.8f;
	public float camDistanceAiming=-0.6f;
	public Vector3 targetDirection;
	public GameObject myBones;

	public bool lookInCameraDirection; // if we want the character to look at the same direction as the camera
	Vector3 lookPos; //the looking position

	Animator anim; //reference to our animator

	WeaponManager weaponManager; //reference to the weapon manager

	public bool debugShoot; //helps us debug the shooting (basically shoots the current weapon)
	WeaponManager.WeaponType weaponType; //the current weapon type we have equipped


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

		public bool DebugAim; 
		//Help us debug the aim, basically makes it possible to change the current values 
		//on runtime since we are hardcoding them
	}

	//Reference to the camera
	FreeCameraLook cameraFunctions;

	UnityEngine.AI.NavMeshAgent agent;
	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		//and our Character Movement
		character = GetComponent<EnemyMove> ();
		//and our animator
		anim = GetComponent<Animator>();

		cam = Camera.main.transform;

	
		targetedPlayer= GameObject.Find ("Player");
	}
		
	void Update()
	{
		Debug.Log (agent.pathStatus.ToString());
		if (agent.remainingDistance<=3f && !anim.IsInTransition (0) && anim.GetFloat ("Speed") < 0.5f) {
			//Then attack
			anim.SetBool ("Attack", true);

		}
	}
		

	//We do everything that has to do with IK on LateUpdate and after the animations have played to remove jittering
	void LateUpdate()
	{
		if(aim) //if we aim
		{
			//pass the new rotation to the IK bone
			Vector3 eulerAngleOffset = Vector3.zero;
			eulerAngleOffset = new Vector3(ik.aimingX,ik.aimingY,ik.aimingZ);

			//do a ray from the center of the camera and forward
			Ray ray = new Ray(cam.position, cam.forward);

			//find where the character should look
			Vector3 lookPosition = ray.GetPoint(ik.point);

			//and apply the rotation to the bone
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming")||anim.GetCurrentAnimatorStateInfo(0).IsName("Fire")){
				ik.spine.LookAt (lookPosition);
				ik.spine.Rotate (eulerAngleOffset, Space.Self);
			}
		}


	}

	float horizontal;
	float vertical;
	float offsetCross;

	void FixedUpdate () 
	{
		//our connection with the variables and our Input
		horizontal = 0f;
		vertical = 1f;



		//if we are not aiming
		if (health > 0)
		{
			targetDirection = targetedPlayer.transform.position - this.transform.position;
			targetDirection = targetDirection.normalized;
			agent.SetDestination (targetedPlayer.transform.position);

			//move = vertical * camForward + horizontal * cam.right; //antigo move
			move= targetDirection;
		}
		else //but if we are aiming
		{
			agent.Stop ();
			//agent.SetPath (null);
		}

		if (move.magnitude > 1) //Make sure that the movement is normalized
			move.Normalize ();

		bool walkToggle = Input.GetKey (KeyCode.LeftShift) || aim; //check for walking input or aiming input

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

		//Our look position depends on if we want the character to look towards the camera or not
		lookPos = lookInCameraDirection && cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;

		//apply the multiplier to our move input
		move *= walkMultiplier;

		//pass it to our move function from our character movement script
		character.Move (move,aim,lookPos,agent.velocity);
		if (anim.IsInTransition (0)) {
			//if (anim.GetCurrentAnimatorStateInfo (0).Equals ("Damage")) {
			anim.SetBool ("Damage", false);
			anim.SetBool ("Death", false);
			if (anim.GetBool ("Attack") == true) {
				anim.SetBool ("Attack", false);
				targetedPlayer.GetComponent<UserInput> ().Hit ();
			}
			//}
		}
	}
	public void Hit(){
		if(!anim.IsInTransition(0))
			anim.SetBool ("Damage", true);
	
		health--;
		if (health <= 0) {
			anim.SetBool ("Death",true);
		}
	}
}
