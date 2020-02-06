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
		public void addNeighbor(float distance, Node newNeighbour){
			Debug.DrawLine (this.position, newNeighbour.position,Color.red,float.MaxValue);
			if(this.neighbors.Contains(newNeighbour)){//Array.Exists(this.neighbors, element => element == proposedNeighbour)
				return;
			}
			this.neighbors.Add (newNeighbour);
			this.neighborsDists.Add (distance);
			newNeighbour.addNeighbor (distance, this);
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

		public static void setupNeighbors(Node[] graph, int[] trianglesIndices){
			for (int i = 0; i < trianglesIndices.Length-4; i=i+3) {
				graph[trianglesIndices[i]].addNeighbor (graph[trianglesIndices[i]].distanceToNode (graph[trianglesIndices[i+1]]), graph[trianglesIndices[i+1]]);
				graph[trianglesIndices[i]].addNeighbor (graph[trianglesIndices[i]].distanceToNode (graph[trianglesIndices[i+2]]), graph[trianglesIndices[i+2]]);
				graph[trianglesIndices[i+1]].addNeighbor (graph[trianglesIndices[i+1]].distanceToNode (graph[trianglesIndices[i+2]]), graph[trianglesIndices[i+2]]);
			}
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

