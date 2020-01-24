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
	public int NMaxAliens=20;
	private bool night=false;
	private bool spawned=false;
	public List<GameObject> aliens;
	private List<GameObject> mortos;
	void Start () {
		aliens.Add(GameObject.Find ("Alien_prefab"));
		NaliensToSpawn = (int) Random.Range (1f, 5f) ;
		mortos = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		float temp = this.transform.rotation.x % 180;
		this.transform.Rotate (Vector3.up * (rotationSpeed * Time.deltaTime));
		if (temp < -0.5f || temp > 0.5f) {
			night = true;
		} else {
			spawned = false;
			night = false;
		}
		if(mortos!=null)mortos.Clear();
		foreach(GameObject a in aliens){
			if(a.GetComponentInParent<AlienEnemy>().dead){
				mortos.Add (a);
				Naliens= aliens.Count - mortos.Count;
			}
		}
		ControleGame.inimigos= Naliens;
		if (night==true && spawned ==false) {
			if (Naliens < NMaxAliens + 5) {
				//apaga os mortos
				if (mortos != null) {
					foreach (GameObject m in mortos) {
						aliens.Remove (m);
						Destroy (m);
					}
				}
				for (int i = 0; i < NaliensToSpawn; i++) {
					Vector3 position = new Vector3 (Random.Range (caracter.position.x + 10f, caracter.position.x + 50f), caracter.position.y, Random.Range (caracter.position.z + 10f, caracter.position.z + 50f));
					aliens.Add(Instantiate (alien, position, Quaternion.identity));	
					Naliens++;
				}
				spawned=true;
				NaliensToSpawn = (int) Random.Range (1f, 5f);
			}
		}
	}

}
