using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
	public static class Pathfinder
	{
		
		public static Vector3 findClosestNode(Vector3 curPos,Node[] vertices){
			float closestDistance = (vertices[0].position - curPos).sqrMagnitude; //initiate with any position
			Vector3 closestVertice = vertices[0].position;
			foreach (Node vertice in vertices) {
				float thisDistance = (vertice.position - curPos).sqrMagnitude;
				if (thisDistance < closestDistance) {
					closestDistance = thisDistance;
					closestVertice = vertice.position;
				} 
			}
			return closestVertice;
		}
		private static int findSmallestCost(Node[] open){
			float smallestCost = 100000000;
			int nodeIndex = -1;
			for (int i = 0; i < open.Length; i++) {
				if (open [i].Fcost<smallestCost) {
					smallestCost = open [i].Fcost;
					nodeIndex = i;
				}
			} 
			return nodeIndex;
		}
		public static Vector3 aStar(Vector3 curPos,Node[] graph){
			ArrayList open = new ArrayList(graph.Length); // the set of nodes to be explored
			ArrayList closed = new Node[graph.Length]; //the set of nodes already explored
			int closedCount = 0;
			int totalNodes = graph.Length; // total of nodes in the graph

			Node cur = new Node (curPos);
			cur.findNeighbors();
			open[0] = cur; // Add the starting node
			bool foundPath; // If we found a path to the destination node
			Node current;
			while (!foundPath && closedCount < totalNodes) {
				int curIndex = findSmallestCost (open);
				current = open[curIndex];
				open.RemoveAt (curIndex);

			}

		}
		
	}
}
	