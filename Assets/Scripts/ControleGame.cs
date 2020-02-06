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
		Debug.Log (verticesArray.Count + " Vertices loaded into the pahtfinding graph!");
		Debug.Log ("There are "+areas.Length+" areas and "+indices.Length+" indices");
		Game.Gobals.graph = this.startGraph (verticesArray, indices);
	}
	void Update () {
		Game.Gobals.tempoPartida = Time.timeSinceLevelLoad;
	}

	Movement.Node[] startGraph(ArrayList vertices, int[] indices){
		Movement.Node[] graph = new Movement.Node[vertices.Count];
		//create graph
		for(int i = 0; i < vertices.Count; i++){
			Movement.Node cur = new Movement.Node ((Vector3) vertices [i]);
			graph[i] = cur;
		}
		Movement.Node.setupNeighbors(graph,indices);
		return graph;
	}

	ArrayList trimTriangles(Vector3[] vertices, int[] triangles){
		//here we exclude of vertices of triangles who dont connect to any other vertices
		// Create a dictionary to hold key-value pairs of words and counts
		IDictionary<int, int> counts = new Dictionary<int, int>();
		ArrayList newVertices = new ArrayList();
			
		for(int i = 0; i < triangles.Length; i++){
			if (!counts.ContainsKey(triangles[i]))
				counts.Add(triangles[i], 1);
			else
				counts[triangles[i]]++; 
		}
		foreach (int index in counts.Keys) {
			if (counts[index]<2) {
				newVertices.Add (vertices [index]);
			}
		}
		return newVertices;
	}

}
	
