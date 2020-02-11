using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeControl : MonoBehaviour {
	public float rotationSpeed=1f;
	// Use this for initialization
	public Transform caracter;
	public GameObject alien;
	private int Naliens=1;
	private int NaliensToSpawn;
	public int NMaxAliens=10;
	public int NMinAliens=5;
	private bool night=false;
	private bool newNight=false;
	public List<GameObject> aliens;
	private List<GameObject> mortos;
	void Start () {
		aliens.Add(GameObject.Find ("Alien_prefab"));
		NaliensToSpawn = (int) Random.Range (NMinAliens, NMaxAliens) ;
		mortos = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		float rotPercentage = this.transform.rotation.x % 180;
		this.transform.Rotate (Vector3.up * (rotationSpeed * Time.deltaTime));
		//Debug.Log (rotPercentage);
		if (rotPercentage < 0.4f && rotPercentage > -0.4f) {
			Game.Gobals.night = true;
		} else {
			newNight = true;
			Game.Gobals.night = false;
		}
		if(mortos!=null)mortos.Clear();//recount dead aliens
		foreach(GameObject a in aliens){
			if(a.GetComponentInParent<EnemyAlien>().dead){
				mortos.Add (a);
				Naliens= aliens.Count - mortos.Count;
			}
		}
		Game.Gobals.inimigos = Naliens;
		if (Game.Gobals.night==true && (Game.Gobals.inimigos==0)) { // || newNight==true//at least wave per night?
			//apaga os mortos
			if (mortos != null) {
				foreach (GameObject m in mortos) {
					aliens.Remove (m);
					Destroy (m);
				}
			}
			for (int i = 0; i < NaliensToSpawn; i++) {
				Vector3 position = new Vector3 (Random.Range (caracter.position.x + 50f, caracter.position.x - 50f), caracter.position.y, Random.Range (caracter.position.z + 50f, caracter.position.z - 50f));
				aliens.Add(Instantiate (alien, position, Quaternion.identity));	
				Naliens++;
			}
			Game.Gobals.waveNumber++;
			newNight=false;
			NaliensToSpawn = (int) Random.Range (NMinAliens, NMaxAliens);
			NMaxAliens++;
			NMinAliens++;
		}
	}
}
