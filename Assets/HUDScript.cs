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
		pontos.text = "Pontos: " + ControleGame.pontos;
		vida.text = "Vida: " + ControleGame.vida * 10;
		balas.text = "Tiros restantes: " + ControleGame.balas;
		inimigos.text = "Inimigos restantes: " + ControleGame.inimigos;
		if (ControleGame.vida <= 0) {
			gameover.gameObject.SetActive(true);
			btnMenu.gameObject.SetActive(true);
		}
	}
	public void IniciarJogo(string nome)
	{
		ControleGame.pontos = 0;
		ControleGame.vida = 0;
		ControleGame.balas = 0;
		ControleGame.inimigos = 0;
		ControleGame.shotsFired = 0;
		ControleGame.LandedShots = 0;
		ControleGame.alienDown = 0;
		SceneManager.LoadSceneAsync (nome);
	}
}
