using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
	public class Node
	{
		Vector3 position; //postition of this node
		Vector3[] neighboors; //neightboors of this node
		float Gcost; // distance from the starting node
		float Hcost; //Heuristic distance from the end node
		float Fcost; //Gcost + Hcost
		bool explored; // if this node was already explored before
		Node parent; // when we explore or update a node we have to store its parent node so we can make the way back

		public Node (Vector3 position, Vector3[] neighboors)
		{
			this.position = position;
			this.neighboors = neighboors;
			this.parent = null;
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
	}
}

