using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement {
	public class EnemyMove : MonoBehaviour {

		//How much we multiply our move speed, we give it a default value of 1 because as you know, everything multiplied by zero will return zero, so no speed 
		float moveSpeedMultiplier = 1; 

		float stationaryTurnSpeed = 180; //if the character is not moving, how fast he will turn
		float movingTurnSpeed = 360; //same as the above but for when the character is moving

		public bool onGround; //if true the character is on the ground
		private static int nFramesPathfinding = 120; // we will run the pathfinding every nFramesPathfinding
		//Our reference to the animator
		Animator animator; 

		Vector3 moveVector; //The move vector
		public float turnAmount; //the calculated turn amount to pass to mecanim
		public float forwardAmount; //the calculated forward amount to pass to mecanim
		Vector3 velocity; //the 3d velocity of the character

		float jumpPower = 10;

		//Reference to our IComparer
		IComparer rayHitComparer;

		public float autoTurnThreshold = 10; //The threshold before the character turns to face the camera
		public float autoTurnSpeed = 20; //How fast will he turn
		Vector3 currentLookPos; //where we are currently looking

		Rigidbody rigidBody; //reference to our rigidbody

		float lastAirTime; //our airtime

		//our two physics materials where we assign them depending on the occasion
		public PhysicMaterial highFriction;
		public PhysicMaterial lowFriction;

		private Vector3 velocityOfAgent;

		CapsuleCollider col; //reference to our collider
		float startHeight; //we store the starting heigh of our collider here
		float oldYcenter;
		float oldRadius;
		float newRadius;
		public int raycatHitsCount = 0; 

		public ArrayList currentPath; // this will hold our current path to the target
		public int indexOfCurNodePath;
		// every 2 seconds perform the print()
		public static IEnumerator targetingCoroutine(AlienEnemy alien, EnemyMove movementScript) {
			int nNodes = 0;

			while (true) {
				if (Game.Gobals.graph != null){
					yield return new WaitForEndOfFrame ();
					//Debug.Log ("WaitAndPrint " + Time.time);
					//Debug.Log ("Frame: " + Time.frameCount);
					if ((Time.frameCount % nFramesPathfinding) == 1) {
						Debug.Log ("Frame: " + Time.frameCount);
						movementScript.currentPath = Pathfinder.aStar (movementScript.rigidBody.position, alien.currentTarget.transform.position, Game.Gobals.graph);
						movementScript.indexOfCurNodePath = 0;
					}
				}
				else{
					Debug.Log ("Waiting for graph to be ready.");
					yield return new WaitForSeconds (2.0f);
				}
				//we decide whether to move or not
				if (!alien.dead) // of the character is alive
				{
					if (alien.currentTarget != null && alien.distanceToTarget > alien.attackRange) { //if we have a target and we are out of the attack range we should go after it
						movementScript.FollowPathToTarget(movementScript.currentPath, alien.GetWalkMultiplier ());
					} else {
						movementScript.TurnAtTarget(alien.currentTarget); // stop and just face the target
					}
				}
				else //if the character is dead
				{
					//clear targets
					alien.currentTarget = null;
				}

			}
		}

		// Use this for initialization
		void Start () {

			//Get the first animator you will find in the children
			animator = GetComponentInChildren<Animator> ();






			//Call the functions that set's up the Animator
			SetUpAnimator();

			//Setup the reference to the rigidBody and our collider
			rigidBody = GetComponent<Rigidbody>();
			col = GetComponent<CapsuleCollider>();
			//store the starting height
			startHeight = col.height;
			//store the old Y heihg of the center
			oldYcenter = col.center.y;
			oldRadius = col.radius;
			newRadius = oldRadius * 1.1f;
		}


		void SetUpAnimator()
		{
			// this is a reference to the animator component on the root.
			animator = GetComponent<Animator>();

			// we use the avatar from a child animator component if present
			// this is to enable easy swapping of the character model as a child node
			//so search every child that has an animator
			foreach (var childAnimator in GetComponentsInChildren<Animator>()) 
			{ 
				//and if the first animator we find, is not the one on the parent
				if (childAnimator != animator) 
				{	//then take the avatar and put into the parent animator
					animator.avatar = childAnimator.avatar;
					//and remove that animator
					Destroy (childAnimator);
					break; //and if you find the first animator, stop searching
				}
			}
		}

		//Updates the movement of the character based on its current speed and the moveSpeedMultiplier
		public void OnAnimatorMove()
		{
			//If the character is on the ground and is not the first frame of play
			if(onGround && Time.deltaTime > 0) 
			{
				//calculate the speed that the character should have
				Vector3 v = (animator.deltaPosition * moveSpeedMultiplier)/ Time.deltaTime;
				//Delta position (position difference) - The difference in the position between the current frame and the previous one
				//Speed = the position difference of the animator * speed multiplier / time

				v.y = rigidBody.velocity.y; //store the characters vertical velocity (in order to not to affect jump speed)
				rigidBody.velocity = v; //update the character's speed
			}
		}
		 
		public void Turn(Vector3 move, Vector3 lookPos){
			this.moveVector = move; 
			this.currentLookPos = lookPos;
			//Call the function that converts our move vector
			ConvertMove();
			TurnForward();
			//Applys extra rotation speed so that the character turns faster
			ApplyExtraTurnRotation ();
		}
		public void Turn(){
			ConvertMove();
			TurnForward();
			ApplyExtraTurnRotation ();
		}

		public void Move(Vector3 move, Vector3 lookPos)
		{
			//Vector3 move is the input in world space

			//pass the variable status to the local variables
			this.moveVector = move; 
			this.currentLookPos = lookPos;
			velocity = rigidBody.velocity; //store the current velocity

			Turn();
			//As the name says, checks if we are on the ground
			GroundCheck ();
			//Assigns the correct physics material depending on the occasion
			SetFriction();
			//Call the appropriate function that handles our velocities depending if we are on the ground or on the air
			if(onGround)
			{
				HandleGroundVelocities();
			}
			else
			{
				HandleAirborneVelocities();
			}

			//Update the Animator parameters
			UpdateAnimator ();
		}
		public void FollowPathToTarget (ArrayList path,float walkMultiplier){
			//pass it to our move function from our character movement script
			float minDist = 0.5f;
			Node nextNode = (Node) path [indexOfCurNodePath];
			float distToNextNode = Movement.Node.distanceFunction(this.transform.position, nextNode.position);
			if (distToNextNode > minDist) {
				MoveToTarget (this.transform.position, walkMultiplier);
			} else {
				indexOfCurNodePath++;
			}

		}
		public void TurnAtTarget(GameObject target){
			Vector3 move;
			Vector3 lookPos;
			Vector3 targetDirection = target.transform.position - this.transform.position;
			targetDirection = targetDirection.normalized;
			move = targetDirection;

			//Look position
			lookPos = transform.position + transform.forward * 100;

			if (move.magnitude > 1) //Make sure that the movement is normalized
				move.Normalize ();
			
			Turn (move, lookPos);
			Move (new Vector3 (0f, 0f, 0f),lookPos);
		}

		public void MoveToTarget(Vector3 targetPos, float walkMultiplier)
		{	
			Vector3 move;
			Vector3 lookPos;
			Vector3 curPos = this.transform.position;
			Vector3 targetDirection = targetPos - curPos;


			targetDirection = targetDirection.normalized;
			move = targetDirection;

			//Look position
			lookPos = curPos + transform.forward * 100;

			if (move.magnitude > 1) //Make sure that the movement is normalized
				move.Normalize ();

			//apply the multiplier to our move vector
			move *= walkMultiplier;





			this.Move (move, lookPos);






			if (animator.IsInTransition (0)) {
				//if (animator.GetCurrentAnimatorStateInfo (0).Equals ("Damage")) {
				animator.SetBool ("Damage", false);
				animator.SetBool ("Attack", false);
			}
		}


		void ConvertMove ()
		{
			// convert the world relative move vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction. 

			//convert the move input (e.g. left -> (-1, 0, 0) from the world space to the characters local space
			Vector3 localMove = transform.InverseTransformDirection (moveVector);

			//calculate the turn amount trigonometrically
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
			//Our forward amount is our localmove forward
			forwardAmount = localMove.z;
		}

		void ApplyExtraTurnRotation ()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			//based on movingTurnSpeed and stationaryTurnSpeed and the forward amount of the character
			float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

		void UpdateAnimator ()
		{
			//We want to use root motion
			animator.applyRootMotion = true;

			//pass the forward and turn amount to the animator
			//animator.SetBool("Aiming", false);
			animator.SetFloat ("Speed",velocity.magnitude);

		}

		//Checks if the character is on the ground or airborne
		void GroundCheck ()
		{
			float raycastHeight = 1.5f;
			//Create a ray with origin the character's transform + 0.5 on the y axis and direction the -y axis
			Ray ray = new Ray (transform.position + Vector3.up * raycastHeight, -Vector3.up); 

			RaycastHit[] hits = Physics.RaycastAll (ray, raycastHeight); //perform a raycast using that ray for a distance of 0.5
			rayHitComparer = new RayHitComparer();
			System.Array.Sort (hits, rayHitComparer); //sort the hits using our comparer (based on distance)
			raycatHitsCount = hits.Length;
	//		Debug.Log ("got "+ hits.Length + " hits!");
	//		foreach (var hit in hits) { //for each of the hits
	//			// check whether we hit a non-trigger collider (and not the character itself)
	//			if (!hit.collider.isTrigger) {
	//				
	//			}
	//		}
			if (velocity.y < jumpPower * raycastHeight) { //if the character is not airborne due to a jump
				//assume that the character is on the air and falling
				onGround = false;
				rigidBody.useGravity = true;
				foreach (var hit in hits) { //for each of the hits
					// check whether we hit a non-trigger collider (and not the character itself)

					if (!hit.collider.isTrigger) {
						//NOT THAT THE ALIEN COLLIDER MUST BE A TRIGGER OR ELSE THE ALIEN WILL JUST FLOAT UP, HITTING ITSELF
						// this counts as being on ground.

						// stick to surface - helps character stick to ground - specially when running down slopes
						//if the character is falling and is close to the ground, we assume that he goes down a slope
						if (velocity.y <= 0) { 
							rigidBody.position = Vector3.MoveTowards (rigidBody.position, hit.point, Time.deltaTime * 5);
							//change the rigid body position to the hit point
						}

						onGround = true; //set the on ground variable since we found our collider
						rigidBody.useGravity = false; //disable gravity since we use the above to stick the character to the ground

						break; //ignore the rest of the hits
					}
				}
			}

			//Store the air time
			if(!onGround)
			{
				lastAirTime = Time.time;
			}

		}

		void TurnForward()
		{
			autoTurnThreshold = 10; //The threshold before the character turns to face the camera
			autoTurnSpeed = 20; //How fast will he turn
			//If the absolute value of the forward amount is less than .01
			if(Mathf.Abs(forwardAmount) < .01f)
			{
				//Find our look angle
				Vector3 lookDelta = transform.InverseTransformDirection(currentLookPos - transform.position);
				float lookAngle = Mathf.Atan2(lookDelta.x,lookDelta.z) * Mathf.Rad2Deg;

				//and if it's higher than our turn threshold
				if(Mathf.Abs(lookAngle) > autoTurnThreshold)
				{
					//correct the character's rotation
					turnAmount += lookAngle * autoTurnSpeed * .001f;
				}
			}
		}

		void SetFriction()
		{
			//Set the appropriate friction depending if we are on the ground or not
			if(onGround)
			{
				//If we are not moving and are on the ground
				if(moveVector.magnitude == 0)
				{
					//we give it a high friction material so that we down slide down slopes
					col.material = highFriction;
				}
				else
				{
					//However if we are moving we don't want friction
					col.material = lowFriction;
				}
			}
			else
			{
				col.material = lowFriction;
			}
		}


		void HandleGroundVelocities()
		{

			Vector3 groundMove = new Vector3(moveVector.x, 0, moveVector.z);
			velocity = Vector3.Lerp(velocity,groundMove,Time.deltaTime*2);
			rigidBody.velocity = velocity;
			//This will stop the character from moving when there is no input
			if(moveVector.magnitude == 0)
			{
				rigidBody.velocity = new Vector3(0, 0, 0);
			}
		}

		void HandleAirborneVelocities()
		{
			//On the air we still want to move but way different than we move on the ground
			//So we simply manipulate the passing move inputs 
			Vector3 airMove = new Vector3(moveVector.x *6, velocity.y, moveVector.z *6);
			rigidBody.velocity = Vector3.Lerp(velocity,airMove,Time.deltaTime*2);

			rigidBody.useGravity = true;

			//and apply extra gravity so that we fall faster
			Vector3 extraGravityForce = (Physics.gravity * 2);

			rigidBody.AddForce(extraGravityForce);
		}


		//Compares two raycasts based on their distance
		class RayHitComparer: IComparer
		{
			public int Compare(object x, object y)
			{
				return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
				//this returns < 0 if x < y
				// > 0 if x > y
				// 0 if x = y
			}	
		}

	}
}
