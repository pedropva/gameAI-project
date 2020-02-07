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
			float closestDistance = Node.distanceFunction(graph [0].position,curPos); //initiate with any position
			Node closestNode = graph [0];
			foreach (Node vertice in graph) {
				float thisDistance = Node.distanceFunction(vertice.position,curPos);
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
				//Debug.Log ("Fcost: "+node.Fcost +" smallestCost: "+smallestCost);
				if (node.Fcost<smallestCost) {
					smallestCost = node.Fcost;
					nodeIndex = i;
				}
			} 
			//Debug.Log ("Choosen node index: "+nodeIndex);
			return nodeIndex;
		}

		public static void resetGraph(Node[] graph){
			foreach (Node node in graph) {
				node.ResetCosts ();
			}
		}
		public static void drawPath(ArrayList path){
			for (int i = 1; i < path.Count; i++) {
				Node node = (Node)path [i];
				Node previousNode = (Node)path [i-1];
				Debug.DrawLine (node.position, previousNode.position,Color.blue,0.5f);
			}
		}

		public static bool comparePaths(ArrayList path1, ArrayList path2){
			if (path1 == null || path2 ==null) {
				return false;
			}
			if (path1.Count != path2.Count) {
				return false;
			}
			for (int i = 0; i < path1.Count; i++) {
				Node node1 = (Node)path1 [i];
				Node node2 = (Node)path2 [i];
				if (node1.position != node2.position) {
					return false;
				}
			}
			return true;
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
			Node closestNodeToStart =  findClosestNode(startPos, graph);
			open.Add(closestNodeToStart); // Add the starting node
			// If we found a path to the destination node
			Node current;
			while (closedCount < totalNodes) {
				if (open.Count == 0) {
					Debug.LogError ("Open queue is empty! Failed to find a valid path!");
					return null;
				}
				int curIndex = Pathfinder.findSmallestCost (open);
				current = (Node) open[curIndex];
				current.Explore ();
				closed.Add (current);
				closedCount++;
				open.RemoveAt (curIndex);

				if (current == destinationNode) { //maybe we can flexibilize this a bit? since we cant always have a node which is the exact
					ArrayList path = current.getPath ();
					drawPath(path);
					return path;
				}
				for (int i=0 ; i<current.neighbors.Count;i++) {
					Node neighbour = (Node) current.neighbors [i];
					//if the node is not in OPEN, OR the path passing by this node is shorter than the path to the neightbour passing by another node
					if (!neighbour.explored){
						if (!open.Contains (neighbour) || (current.getPath ().Count + 1 < neighbour.getPath ().Count)) {
							//set parent of neightbour to current
							int indexOfNeighbour = current.neighbors.IndexOf (neighbour);
							float distanceToParent = (float)current.neighborsDists [indexOfNeighbour];
							neighbour.setParent (distanceToParent, current);
							//set fcost of neighbour
							neighbour.updateCosts (closestNodeToStart, destinationNode);//this uses the starting node, the destination node and the parents of the node

							if (!open.Contains (neighbour)) {//if neighbour is not in OPEN
								open.Add (neighbour);//add neighbour to OPEN
								Debug.DrawLine (current.position, neighbour.position,Color.green,0.5f);
							}
						}
					}
				}
			}
			return null;
		}
	}
}
