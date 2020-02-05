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
		public Node[] neighbors; //neightboors of this node
		public float[] neighborsDists; //distance to each neightboor of this node
		public float Gcost; // distance from the starting node
		public float Hcost; //Heuristic distance from the end node
		public float Fcost; //Gcost + Hcost
		public bool explored; // if this node was already explored before
		public Node parent; // when we explore or update a node we have to store its parent node so we can make the way back

		public Node (Vector3 position)
		{
			this.position = position;
			this.parent = null;
			this.neighbors = new Node[8] {null,null,null,null,null,null,null,null};
			this.neighborsDists = new float[8]{-1,-1,-1,-1,-1,-1,-1,-1};  //our vector with the 8 smallest distances from a node
		}
		public void ResetCosts ()
		{
			this.Gcost = -1; // distance from the starting node
			this.Hcost = -1; //Heuristic distance from the end node
			this.Fcost = -1; //Gcost + Hcost
			this.explored = false; // if this node was already explored 
			this.parent = null;
		}
		public int CountNeighbors(){
			int count = 0;
			foreach (Node item in neighbors) {
				if(item != null){
					count++;
				}
			}
			return count;
		}
		public void Explore()
		{
			this.explored = true; // if this node was already explored 
		}
		public void updateNeighborDists(float distance, Node proposedNeighbour){
			//receives a proposed neighbor, if its closer than any of its 8 neightbors, then it becomes a neighbor
			if(Array.Exists(this.neighbors, element => element == proposedNeighbour)){
				return;
			}
			for(int k = 0; k < this.neighborsDists.Length; k++){
				if (distance < this.neighborsDists[k] || this.neighborsDists[k] == -1){ // if the new value is smaller or there was no number there
					this.neighborsDists[k] = distance;
					this.neighbors[k] = proposedNeighbour;
					proposedNeighbour.updateNeighborDists (distance,this);
					return;
				}
			}
		}
		public static float distanceFunction(Vector3 start, Vector3 end){
			return (start - end).sqrMagnitude;
		}

		private float distanceToNode(Node target){
			return Movement.Node.distanceFunction (this.position, target.position);
		}

		public void findNeighbors(Node[] graph){
			foreach (Node node in graph) {
				float distance = this.distanceToNode(node);
				this.updateNeighborDists (distance, node);
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

