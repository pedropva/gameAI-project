using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Movement
{
	public class Node
	{
		Vector3 position; //postition of this node
		Node[] neighboors; //neightboors of this node
		float[] neighboorsDists; //distance to each neightboor of this node
		float Gcost; // distance from the starting node
		float Hcost; //Heuristic distance from the end node
		float Fcost; //Gcost + Hcost
		bool explored; // if this node was already explored before
		Node parent; // when we explore or update a node we have to store its parent node so we can make the way back

		public Node (Vector3 position)
		{
			this.position = position;
			this.parent = null;
			this.neighboors = new Node[8];
			this.neighboorsDists = new float[8]{-1,-1,-1,-1,-1,-1,-1,-1};  //our vector with the 8 smallest distances from a node
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
			//update G costs for all neighboors
			//if the new G cost is less then point to the one who updated it
		}
		public Vector3 GetPosition(){
			return this.position;
		}
		public void updateNeighborDists(float distance, Node proposedNeighbor){
			//receives a proposed neighbor, if its closer than any of its 8 neightbors, then it becomes a neighbor
			if(Array.Exists(this.neighboors, element => element == proposedNeighbor)){
				return;
			}
			for(int k = 0; k < this.neighboorsDists.Length; k++){
				if (distance < this.neighboorsDists[k] || this.neighboorsDists[k] == -1){ // if the new value is smaller or there was no number there
					this.neighboorsDists[k] = distance;
					this.neighboors[k] = proposedNeighbor;
					proposedNeighbor.updateNeighborDists (distance,this);
					return;
				}
			}
		}
	}
}

