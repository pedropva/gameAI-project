using UnityEngine;
using System.Collections;

//this script is basically the stats for each weapon
public class WeaponControl : MonoBehaviour {

	public bool equip; //if the weapon is equiped
	public WeaponManager.WeaponType weaponType; //the weapon type

	//pretty much self explanatory
	public int MaxAmmo; 
	public int MaxClipAmmo = 30;
	public int curAmmo;
	public bool CanBurst;
	public float Kickback = 0.1f;
	public Collider other;

	public GameObject HandPosition; //where the left hand should go
	public GameObject bulletPrefab; //the bullet prefab, this is an absolete variable
	public Transform bulletSpawn; //where the bullets come from

	//absolete variables
	GameObject bulletSpawnGO; 
	ParticleSystem bulletPart;
	bool fireBullet;
	public AudioSource audioSource;
	
	//reference to our weapon manager from the parent
	WeaponManager parentControl;

	//reference to the weapon animator
	Animator weaponAnim;
	
	//we store here the equip and unequip position/rotation
	[Header("Positions")]
	public bool hasOwner;
	public Vector3 EquipPosition;
	public Vector3 EquipRotation;
	public Vector3 UnEquipPosition;
	public Vector3 UnEquipRotation;
	//Debug Scale
	Vector3 scale;
	public bool fire;
	public Animator anim;

	//on which body part should the weapon be placed if it's not equipped?
	public RestPosition restPosition;
	public enum RestPosition
	{
		RightHip,
		Waist
	}


	void Start () 
	{
		curAmmo = MaxClipAmmo;
		//we don't use this anymore
		/*bulletSpawnGO = Instantiate(bulletPrefab, transform.position,Quaternion.identity) as GameObject;
		bulletSpawnGO.AddComponent<ParticleDirection>();
		bulletSpawnGO.GetComponent<ParticleDirection>().weapon = bulletSpawn;
		bulletPart = bulletSpawnGO.GetComponent<ParticleSystem>();*/

		//store our references and our scale
		audioSource = GetComponent<AudioSource>();
		//weaponAnim = GetComponent<Animator>();
		//scale = transform.localScale;
		anim = GameObject.Find("Player").GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//the local scale is always the stored scale
		//we do this because we don't have consistent scale for all our assets and parenting/unparenting children might mess with the scale
		//transform.localScale = scale;
		this.fire = anim.GetBool ("Attack");
	}

	public bool Fire()
	{

		//if the weapon is equipped
		if(equip)
		{
			//put it to the correct position and rotation
			//transform.parent = transform.GetComponentInParent<WeaponManager>().transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
			//transform.localPosition = EquipPosition;
			//transform.localRotation = Quaternion.Euler(EquipRotation);
			if (weaponType == WeaponManager.WeaponType.Melee) {
				return true;
			}
			if(curAmmo > 0)
			{
				curAmmo --;
				ControleGame.balas = curAmmo;
				//bulletPart.Emit(1);
				audioSource.Play();

				this.transform.GetComponent<MultiShooter> ().BasicBeamAttack();
				ControleGame.shotsFired++;
				fireBullet = false;
				fire = false;	
				return true;
			}
			else
			{
				if(MaxAmmo >= MaxClipAmmo)
				{
					curAmmo = MaxClipAmmo;
					MaxAmmo -= MaxClipAmmo;
				}
				else
				{
					curAmmo = MaxClipAmmo - (MaxClipAmmo - MaxAmmo);

				}
				fire = false;	
				fireBullet = false;
				Debug.Log("Reload");
			}
		}
	return false;
	}
	public void OnTriggerStay(Collider other){
		Vector3 direction = other.transform.position - this.transform.position;
		direction = direction.normalized;
		if (weaponType == WeaponManager.WeaponType.Melee && fire) {
			if (other.GetComponentInParent<Inimiguinho> ()) {
				anim.SetBool("Hitting",true);
			}else if (other.attachedRigidbody) {
				other.attachedRigidbody.AddForce (direction * 1000);
			}
		}
		fire = false;
		this.other=other;
	}
	void OnTriggerExit(Collider other){
		this.other = null;
		anim.SetBool("Hitting",false);
	}
}
