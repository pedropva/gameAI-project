using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
	public static class Pathfinder
	{

		public static Vector3 findClosestPos(Vector3 curPos,Node[] vertices){
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

		public static Node findClosestNode(Vector3 curPos,Node[] graph){
			float closestDistance = (graph [0].position - curPos).sqrMagnitude; //initiate with any position
			Node closestNode = graph [0];
			foreach (Node vertice in graph) {
				float thisDistance = (vertice.position - curPos).sqrMagnitude;
				if (thisDistance < closestDistance) {
					closestDistance = thisDistance;
					closestNode = vertice;
				} 
			}
			return closestNode;
		}

		private static int findSmallestCost(ArrayList open){
			float smallestCost = 100000000;
			int nodeIndex = -1;
			for (int i = 0; i < open.Count; i++) {
				Node node = (Node)open [i];
				if (node.Fcost<smallestCost) {
					smallestCost = node.Fcost;
					nodeIndex = i;
				}
			} 
			return nodeIndex;
		}

		public static void resetGraph(Node[] graph){
			foreach (Node node in graph) {
				node.ResetCosts ();
			}
		}

		public static ArrayList aStar(Vector3 startPos, Vector3 destinationPos, Node[] graph){
			resetGraph (graph);//reset all costs of the graph
			ArrayList open = new ArrayList(graph.Length); // the set of nodes to be explored
			ArrayList closed = new ArrayList(graph.Length); //the set of nodes already explored
			int closedCount = 0;
			int totalNodes = graph.Length; // total of nodes in the graph

			//destinatiion node
			Node destinationNode = findClosestNode(destinationPos, graph);

			//starting node
			Node startNode = new Node (startPos); //create the starting node
			startNode.findNeighbors(graph); // find all neighbors of this node on the graph
			startNode.updateCosts (startNode, destinationNode);
			open.Add(startNode); // Add the starting node

			bool foundPath; // If we found a path to the destination node
			Node current;
			while (closedCount < totalNodes) {
				int curIndex = Pathfinder.findSmallestCost (open);
				current = (Node) open[curIndex];
				current.Explore ();
				open.RemoveAt (curIndex);

				if (current == destinationNode) { //maybe we can flexibilize this a bit? since we cant always have a node which is the exact
					return current.getPath ();
				}
				//Debug.Log (current.CountNeighbors());
				foreach (Node neighbour in current.neighbors) {
					if (neighbour.explored) {
						continue;
					}
					//if the node is not in OPEN, OR the path passing by this node is shorter than the path to the neightbour passing by another node
					if (!open.Contains(neighbour) || (current.getPath ().Count + 1 < neighbour.getPath().Count)) {
						//set parent of neightbour to current
						neighbour.parent = current;
						//set fcost of neighbour
						neighbour.updateCosts(startNode, destinationNode);//this uses the starting node, the destination node and the parents of the node

						if (!open.Contains(neighbour)){//if neighbour is not in OPEN
							open.Add (neighbour);//add neighbour to OPEN
						}
					}
				}
			}
			return null;
		}
	}
}
