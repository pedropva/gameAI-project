using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Movement
{
	public class Node
	{
		public Vector3 position; //postition of this node
		public ArrayList neighbors; //neightboors of this node
		public ArrayList neighborsDists; //distance to each neightboor of this node
		public float Gcost; // distance from the starting node
		public float Hcost; //Heuristic distance from the end node
		public float Fcost; //Gcost + Hcost
		public bool explored; // if this node was already explored before
		public Node parent; // when we explore or update a node we have to store its parent node so we can make the way back
		public float parentDistance; // distance to parent node

		public Node (Vector3 position)
		{
			this.position = position;
			this.parent = null;
			this.parentDistance = 10000000f;
			this.neighbors = new ArrayList();
			this.neighborsDists = new ArrayList();  //our vector with the 8 smallest distances from a node
		}
		public void ResetCosts ()
		{
			this.Gcost = -1; // distance from the starting node
			this.Hcost = -1; //Heuristic distance from the end node
			this.Fcost = -1; //Gcost + Hcost
			this.explored = false; // if this node was already explored 
			this.parent = null;
		}
		public void Explore()
		{
			this.explored = true; // if this node was already explored 
		}
		public void addNeighbor(Node newNeighbour, float distance){
			Debug.DrawLine (this.position, newNeighbour.position,Color.red,float.MaxValue);
			if(this.neighbors.Contains(newNeighbour)){//Array.Exists(this.neighbors, element => element == proposedNeighbour)
				return;
			}
			this.neighbors.Add (newNeighbour);
			this.neighborsDists.Add (distance);
			newNeighbour.addNeighbor (this, distance);
		}

		public void setParent(float distance, Node newParent){
			this.parent = newParent;
			this.parentDistance = distance;
		}

		public static float distanceFunction(Vector3 start, Vector3 end){
			return (start - end).sqrMagnitude;
		}

		private float distanceToNode(Node target){
			return Movement.Node.distanceFunction (this.position, target.position);
		}

		public static void setupNeighbors(Node[] graph, int[] trianglesIndices, IDictionary<int, Node> map){
			//Debug.Log ("vertices: " + trianglesIndices.Length + " map:" + map.Count);
			for (int i = 0; i < trianglesIndices.Length; i=i+3) {
				//Debug.Log (trianglesIndices[i] +" " +trianglesIndices[i+1]+" "+trianglesIndices[i+2]);
				Node node1 = (Node) map[trianglesIndices[i]];
				Node node2 = (Node) map[trianglesIndices[i+1]];
				Node node3 = (Node) map[trianglesIndices[i+2]];
				node1.addNeighbor (node2, node1.distanceToNode (node2));
				node2.addNeighbor (node3, node2.distanceToNode (node3));
				node1.addNeighbor (node3, node3.distanceToNode (node3));
			}
		}

		public static Node[] startGraph(ArrayList vertices, int[] indices){
			ArrayList uniqueVertices = getUniqueVertices (vertices);
			Debug.Log (uniqueVertices.Count + " Vertices loaded into the pahtfinding graph!");
			Node[] graph = new Movement.Node[uniqueVertices.Count];
			//create graph
			for(int i=0;i < uniqueVertices.Count;i++) {
				Vector3 uniqueVertice = (Vector3) uniqueVertices [i];
				Node cur = new Node (uniqueVertice);
				graph[i] = cur;
			}

			IDictionary<int, Node> map = Node.getIndicesMapping (vertices, graph);
			Node.setupNeighbors(graph,indices, map);
			return graph;
		}

		public static IDictionary<int, Node> getIndicesMapping(ArrayList vertices,Node[] graph){
			IDictionary<int, Node> map = new Dictionary<int, Node>();
			for (int i = 0; i < vertices.Count; i++) {
				for (int j = 0; j < graph.Length; j++) {
					Vector3 vertice = (Vector3) vertices [i];

					if (vertice == graph[j].position) {
						map.Add (i, graph[j]);
					}
				}
			}
			return map;
		}

		public static ArrayList getUniqueVertices(ArrayList vertices){
			ArrayList unique = new ArrayList();
			for (int i = 0; i < vertices.Count; i++) {
				if (!unique.Contains(vertices[i])) {
					unique.Add (vertices [i]);
				}
			}
			return unique;
		}
			
		public void updateCosts(Node startNode, Node destinationNode){
			//update G cost
			this.Gcost = this.getPathLenghtOnWorld ();
			//update H cost
			this.Hcost = distanceToNode(destinationNode);
			//update F cost
			this.Fcost = this.Gcost + this.Hcost;
		}

		public float getPathLenghtOnWorld(){
			ArrayList path = this.getPath ();
			float totalDistance = 0;
			for (int i = path.Count-1; i >= 0; i--) {
				Node curNode = (Node) path [i];
				if (curNode.parent != null) {
					curNode.distanceToNode (curNode.parent);	
				}
			}
			return totalDistance;
		}

		public ArrayList getPath (){
			ArrayList path = new ArrayList();
			if (this.parent == null) {
				path.Add (this);
			}else{
				path = this.parent.getPath ();
				path.Add(this);
			}
			return path;
		}
	}
}

