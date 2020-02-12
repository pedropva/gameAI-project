﻿using UnityEngine;
using System.Collections;

//This script is basically how the user controls the character
public class UserInput : MonoBehaviour {

	public bool walkByDefault = false; //If we want to walk by default
	
	private CharMove character; //reference to our character movement script
	private Transform cam; //reference to our case
	private Vector3 camForward; //stores the forward vector of the cam
	public Vector3 move; //our move vector
	public int health=30;
	public bool aim; //if we are aiming
	public bool squat;
	public bool jump;
	public float aimingWeight; //the aiming weight, helps with IK
	public GameObject camera;
	public float camDistanceNormal=-0.8f;
	public float camDistanceAiming=-0.6f;
	public float hitForce=100;
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


	// Use this for initialization
	void Start ()
	{
		Game.Gobals.vida=health;
		//Setup our camera reference
		if(Camera.main != null)
		{
			cam = Camera.main.transform;
		}

		//and our Character Movement
		character = GetComponent<CharMove> ();
		//and our animator
		anim = GetComponent<Animator>();
		//and our weapon manager
		weaponManager = GetComponent<WeaponManager>();
		//and the collider


		//And setup the reference to the FreeCameraLook, 
		//but since we already have store the camera, 
		//we can navigate to the root transform to get the component from there instead of searching for it
		cameraFunctions = Camera.main.transform.root.GetComponent<FreeCameraLook>();
		
		//Store the offset of the crosshair
		//offsetCross = cameraFunctions.crosshairOffsetWiggle;
	}

	//Function that corrects the Ik depending on the current weapon type
	void CorrectIK()
	{
		//weaponType = weaponManager.weaponType;

		if(!ik.DebugAim)
		{
			switch(weaponType)
			{
			case WeaponManager.WeaponType.Pistol:
				ik.aimingZ = 221.4f;
				ik.aimingX = -71.5f;
				ik.aimingY = 20.6f;
				break;
			case WeaponManager.WeaponType.Rifle:
				ik.aimingZ = 212.19f;
				ik.aimingX = -66.1f;
				ik.aimingY = 14.1f;
				break;
			}
		}
	}



	void Update()
	{
		CorrectIK();

		if(!ik.DebugAim) //if we do not debug the aim
			aim = Input.GetMouseButton (1) || debugShoot; //then the aim bool is controlled by the right mouse click
		jump = Input.GetKey (KeyCode.Space);
		if(Input.GetKeyDown (KeyCode.C)){
			squat = !squat; 
		}	
		//the same goes for the aim of the weapon manager
		//weaponManager.aim = aim;

		//if we are aiming
		if(aim)
		{
			//And our active weapon can't burst fire
			if(!weaponManager.ActiveWeapon.CanBurst)
			{
				//and we left click
				if((Input.GetMouseButtonDown(0))&& !anim.IsInTransition(0))
				{
					//Then shoot
					if (weaponManager.FireActiveWeapon ()) {
						anim.SetTrigger("Attack");
					}
					//ShootRay();//Call our shooting ray, see below
					//and wiggle our crosshair and camera
					//cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon, true);
				}
			}
			else //if it can burst fire
			{
				//then do the same as above for as long the fire mouse button is pressed
				if(Input.GetMouseButton(0) || debugShoot)
				{
					if (weaponManager.FireActiveWeapon ()) {
						anim.SetTrigger("Attack");
					}
					//ShootRay();
					//cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon, true);
				}
			}
		}
		//Switches between our weapons, linear
		if(Input.GetAxis("Mouse ScrollWheel") <= -0.1f)
			{
				weaponManager.ChangeWeapon(false);
			}

			if(Input.GetAxis("Mouse ScrollWheel") >= 0.1f)
			{
				weaponManager.ChangeWeapon(true);
			}
		//HandleCurves();
	}

	//the prefab for our bullets
	public GameObject bulletPrefab;

	//Shoots a ray everytime we shoot
	void ShootRay()
	{
		//find the center of the screen
		float x = Screen.width /2;
		float y = Screen.height/2;

		//and make a ray from it
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(x,y,0));
		RaycastHit hit;

		//Instantiate the bullet prefab that has a line render and store it in a variable
		GameObject go = Instantiate(bulletPrefab,transform.position,Quaternion.identity) as GameObject;
		LineRenderer line = go.GetComponent<LineRenderer>();
		
		//the first position of or "bullet" will be the bullet spawn point
		//of our active weapon, converted from local to world position
		Vector3 startPos = weaponManager.ActiveWeapon.bulletSpawn.TransformPoint(Vector3.zero);
		Vector3 endPos = Vector3.zero;

		//bit shift a layer mask
		int mask = ~(1<< 8);

		//so that our raycast collides with all the colliders in all the layers, except the one masked
		if(Physics.Raycast(ray,out hit,Mathf.Infinity,mask))
		{
			//find the distance between the bullet spawn position and the hit.point
			float distance = Vector3.Distance(weaponManager.ActiveWeapon.bulletSpawn.transform.position, hit.point);

			//and raycast everything in that direction and for that distance
			RaycastHit[] hits = Physics.RaycastAll(startPos,hit.point - startPos, distance);

			//and for every hit
			foreach(RaycastHit hit2 in hits)
			{
				//Add the logic to what happens on whatever we hit
				
				//for example, if we hit a gameobject that has a rigidbody
				if(hit2.transform.GetComponent<Rigidbody>())
				{
					//then apply the appropriate force at the correct direction
					Vector3 direction = hit2.transform.position - transform.position;
					direction = direction.normalized;
					hit2.transform.GetComponent<Rigidbody>().AddForce(direction * hitForce);
				}
			}

			//the end position of our bullet is the hit.point
			endPos = hit.point;
		}
		else //else if the raycast didn't hit anything
		{
			//the end position will be a far away point upon the ray
			endPos = ray.GetPoint(100);
		}

		//set up the positions to the line renderer
		line.SetPosition(0,startPos);
		line.SetPosition(1,endPos);
	}


	//We do everything that has to do with IK on LateUpdate and after the animations have played to remove jittering
	void LateUpdate()
	{
		//our aiming weight smoothly becomes 0 or 1 depending if we are aiming or not, 
		aimingWeight = Mathf.MoveTowards(aimingWeight, (aim)? 1.0f : 0.0f , Time.deltaTime * 5);
		
		//the normal and aiming state of the camera, basically how much close to the player it is
		Vector3 normalState = new Vector3(0,0,camDistanceNormal);
		Vector3 aimingState = new Vector3(0,0,camDistanceAiming);
		
		//and that is lerped depending on t = aimigweight
		Vector3 pos = Vector3.Lerp(normalState,aimingState,aimingWeight);
		cam.transform.localPosition= pos;

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
			if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming")||anim.GetCurrentAnimatorStateInfo(0).IsName("Fire")) && weaponManager.ActiveWeapon.weaponType != WeaponManager.WeaponType.Melee){
				ik.spine.LookAt (lookPosition);
				ik.spine.Rotate (eulerAngleOffset, Space.Self);
			}
		}


	}

	//our variables where we store our input and the offset of the crosshair 
	float horizontal;
	float vertical;
	float offsetCross;

	void FixedUpdate () 
	{
		//our connection with the variables and our Input
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");


		if (this.transform.position.y < -5f) {
			Debug.Log ("Damage");
			StartCoroutine(Damage());
		}

		//if we are not aiming
		
		if(!aim)
		{
			lookInCameraDirection = false;
			if (camera.GetComponent<FreeCameraLook>()){
				camera.GetComponent<FreeCameraLook> ().OffAim ();
			}
			if(cam != null) //if there is a camera
			{
				//Take the forward vector of the camera (from its transform) and 
				// eliminate the y component
				// scale the camera forward with the mask (1, 0, 1) to eliminate y and normalize it
				camForward = Vector3.Scale(cam.forward, new Vector3(1,0,1)).normalized;

				//move input front/backward = forward direction of the camera * user input amount (vertical)
				//move input left/right = right direction of the camera * user input amount (horizontal)
				move = vertical * camForward + horizontal * cam.right;
			}
			else
			{
				//if there is not a camera, use the global forward (+z) and right (+x)
				move = vertical * Vector3.forward + horizontal * Vector3.right;
			}
		}

		else //but if we are aiming
		{
			lookInCameraDirection = true;
			if (camera.GetComponent<FreeCameraLook>()){
				camera.GetComponent<FreeCameraLook> ().OnAim ();
			}
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
		character.Move (move,aim,lookPos,squat,jump);
	}
	public void Hit(){
		StartCoroutine(Damage());
	}
	IEnumerator Damage(){
		yield return new WaitForSecondsRealtime (0.5f);
		anim.SetBool ("Damage", true);
		if (health >0) {
			health--;	
			Game.Gobals.vida = health;
		}
		if (health <= 0) {
			anim.SetBool ("Death", true);
			this.enabled = false;
			anim.SetBool ("Death", true);
		} 
	}
}
