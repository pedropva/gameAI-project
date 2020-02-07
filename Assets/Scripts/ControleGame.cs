using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ControleGame: MonoBehaviour {
	void Start () {
		//starting navigation graph
		UnityEngine.AI.NavMeshTriangulation navTriangles = UnityEngine.AI.NavMesh.CalculateTriangulation (); // get baked Navigation Mesh Data;
		int[] areas = navTriangles.areas;
		int[] indices = navTriangles.indices;
		Vector3[] vertices = navTriangles.vertices;
		//ArrayList verticesArray = this.trimTriangles (vertices,indices);
		//Debug.Log(vertices.Length - verticesArray.Count +" Vertices were trimmed from the graph");
		ArrayList verticesArray = new ArrayList();
		verticesArray.AddRange(vertices);
		Debug.Log ("There are "+areas.Length+" areas and "+indices.Length+" indices");
		Game.Gobals.graph = Movement.Node.startGraph (verticesArray, indices);
	}
	void Update () {
		Game.Gobals.tempoPartida = Time.timeSinceLevelLoad;
	}
}
	
