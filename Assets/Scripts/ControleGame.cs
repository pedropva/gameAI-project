using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControleGame: MonoBehaviour {
	public static int pontos = 0;
	public static int vida = 0;
	public static int balas = 0;
	public static int inimigos = 0;
	public static int shotsFired = 0;
	public static int LandedShots = 0;
	public static int alienDown = 0;
	public static float tempoPartida;
	void Start () {
		
	}
	void Update () {
		tempoPartida = Time.timeSinceLevelLoad;
	}
}
