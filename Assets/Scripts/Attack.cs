using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : StateMachineBehaviour {
	public GameObject targeted;
	private GameObject player;
	private Collider fists;
	private WeaponManager weapon;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		player = GameObject.Find ("Player");
		targeted = player;
		weapon = player.GetComponent<WeaponManager> ();
		if (animator.GetBool("Attack")) {
			targeted.GetComponent<UserInput> ().Hit ();	
		}else if(weapon.weaponType == WeaponManager.WeaponType.Melee){
			fists = player.GetComponentInChildren<WeaponControl> ().other;
			try//if there is any alien coliding with the fists
			{
				fists.GetComponentInParent<AlienEnemy> ().MeleeHit (20000f);	
			}catch{}
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
//	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		this.
//	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		
//	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
