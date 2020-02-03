using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ControleGame: MonoBehaviour {
	void Start () {
		//starting navigation graph
		UnityEngine.AI.NavMeshTriangulation navTriangles = UnityEngine.AI.NavMesh.CalculateTriangulation (); // get baked Navigation Mesh Data;
		Vector3[] vertices = navTriangles.vertices;
		Debug.Log (vertices.Length + " Vertices loaded into the pahtfinding graph!");
		Game.Gobals.graph = this.startGraph (vertices);
	}
	void Update () {
		Game.Gobals.tempoPartida = Time.timeSinceLevelLoad;
	}
	Movement.Node[] startGraph(Vector3[] vertices){
		Movement.Node[] graph = new Movement.Node[vertices.Length];
		//create graph
		for(int i = 0; i < vertices.Length; i++){
			Movement.Node cur = new Movement.Node (vertices [i]);
			graph[i] = cur;
		}
		foreach (Movement.Node node1 in graph) {
			foreach (Movement.Node node2 in graph) {
				float distance = (node1.position - node2.position).sqrMagnitude;
				node1.updateNeighborDists (distance, node2);
			}
		}
		return graph;
	}
}
	
