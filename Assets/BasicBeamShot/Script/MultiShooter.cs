using UnityEngine;
using System.Collections;

public class MultiShooter : MonoBehaviour {

	public GameObject Shot1;
	public GameObject Shot2;
    public GameObject Wave;
	Transform bulletSpawn;
	public GameObject caracter;
	private PlayerController t;
	public float Disturbance = 0;

	public int ShotType = 0;

	private GameObject NowShot;
	GameObject Bullet;
	void Start () {
		
	}

	void Update () {
		NowShot = null;
	}
	public void setSpawn(){
		if (bulletSpawn == null){
			t = caracter.GetComponent<PlayerController> ();
			if (t.RightGun != null) {
				bulletSpawn = t.RightGun.transform.GetChild (0);	
			}
		}
	}

	//create BasicBeamShot
	public void BasicBeamAttack()
	{
		setSpawn ();
		Bullet = Shot1;
		//Fire
		GameObject s1 = (GameObject)Instantiate(Bullet, bulletSpawn.position, bulletSpawn.transform.rotation);
		s1.GetComponent<BeamParam>().SetBeamParam(this.GetComponent<BeamParam>());

		GameObject wav = (GameObject)Instantiate(Wave, bulletSpawn.position, bulletSpawn.transform.rotation);
		wav.transform.localScale *= 0.25f;
		wav.transform.Rotate(Vector3.left, 90.0f);
		wav.GetComponent<BeamWave>().col = this.GetComponent<BeamParam>().BeamColor;

	}


	//create GeroBeam
	public void GeroBeamAttack()
	{
		setSpawn ();
		GameObject wav = (GameObject)Instantiate(Wave, bulletSpawn.position, bulletSpawn.transform.rotation);
		wav.transform.Rotate(Vector3.left, 90.0f);
		wav.GetComponent<BeamWave>().col = this.GetComponent<BeamParam>().BeamColor;

		Bullet = Shot2;
		//Fire
		NowShot = (GameObject)Instantiate(Bullet, bulletSpawn.position, bulletSpawn.transform.rotation);
	}
	//it's Not "GetButtonDown"
	public void BasicGeroBeamAttack()
	{
		setSpawn ();
		BeamParam bp = this.GetComponent<BeamParam>();
		if(NowShot.GetComponent<BeamParam>().bGero)
			NowShot.transform.parent = transform;

		Vector3 s = new Vector3(bp.Scale,bp.Scale,bp.Scale);

		NowShot.transform.localScale = s;
		NowShot.GetComponent<BeamParam>().SetBeamParam(bp);
	}
	public void StopGeroBeamAttack()
	{
		if(NowShot != null)
		{
			NowShot.GetComponent<BeamParam>().bEnd = true;
		}
	}
}
