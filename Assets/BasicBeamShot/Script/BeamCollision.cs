using UnityEngine;
using System.Collections;

public class BeamCollision : MonoBehaviour {
	
    public bool Reflect = false;
	private BeamLine BL;
	public GameObject HitEffect = null;
	private GameObject whoshooting;
	private bool bHit = false;
	private BeamParam BP;

	// Use this for initialization
	void Start () {
		BL = (BeamLine)this.gameObject.transform.FindChild("BeamLine").GetComponent<BeamLine>();
		BP = this.transform.root.gameObject.GetComponent<BeamParam>();
		whoshooting = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		//RayCollision
		RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("NoBeamHit") | 1 << 2);
        if (HitEffect != null && !bHit && Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            GameObject hitobj = hit.collider.gameObject;
			if(hit.distance < BL.GetNowLength())
		    {
				BL.StopLength(hit.distance);
				bHit = true;

                Quaternion Angle;
                //Reflect to Normal
                if (Reflect)
                {
                    Angle = Quaternion.LookRotation(Vector3.Reflect(transform.forward, hit.normal));
                }
                else
                {
                    Angle = Quaternion.AngleAxis(180.0f, transform.up) * this.transform.rotation;
                }
                GameObject obj = (GameObject)Instantiate(HitEffect,this.transform.position+this.transform.forward*hit.distance,Angle);
				obj.GetComponent<BeamParam>().SetBeamParam(BP);
				obj.transform.localScale = this.transform.localScale;
			}
			Hit(hit.collider.gameObject,whoshooting);
		}
		/*
		if(bHit && BL != null)
		{
			BL.gameObject.renderer.material.SetFloat("_BeamLength",HitTimeLength / BL.GetNextLength() + 0.05f);
		}
		*/
	}
	public void Hit (GameObject who,GameObject whoshooted){
		Vector3 direction = who.transform.position - whoshooted.transform.position;
		direction = direction.normalized;
		if (who.transform.GetComponent<AlienEnemy> ()) {
			who.transform.GetComponent<AlienEnemy> ().Hit (direction,BP.hitForce);
			return;
		}
		if (who.transform.GetComponent<Rigidbody> ()) {
			who.transform.GetComponent<Rigidbody> ().AddForce (direction * BP.hitForce);
		}
		if (who.transform.GetComponent<Enemie> ()) {
			who.transform.GetComponent<Enemie> ().hit = true;
		}
	}
}
