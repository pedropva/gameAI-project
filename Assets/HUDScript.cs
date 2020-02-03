using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDScript : MonoBehaviour {
	public Text pontos;
	public Text vida;
	public Text balas;
	public Text inimigos;
	public Text gameover;
	public Button btnMenu;
	// Use this for initialization
	void Start () {
		gameover.gameObject.SetActive(false);
		btnMenu.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		pontos.text = "Pontos: " + Game.Gobals.pontos;
		vida.text = "Vida: " + Game.Gobals.vida * 10;
		balas.text = "Tiros restantes: " + Game.Gobals.balas;
		inimigos.text = "Inimigos restantes: " + Game.Gobals.inimigos;
		if (Game.Gobals.vida <= 0) {
			gameover.gameObject.SetActive(true);
			btnMenu.gameObject.SetActive(true);
		}
	}
	public void IniciarJogo(string nome)
	{
		Game.Gobals.pontos = 0;
		Game.Gobals.vida = 0;
		Game.Gobals.balas = 0;
		Game.Gobals.inimigos = 0;
		Game.Gobals.shotsFired = 0;
		Game.Gobals.LandedShots = 0;
		Game.Gobals.alienDown = 0;
		SceneManager.LoadSceneAsync (nome);
	}
}
