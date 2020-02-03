using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
	public static class Pathfinder
	{
		
		public static Vector3 findClosestNode(Vector3 curPos,Node[] vertices){
			float closestDistance = (vertices[0].GetPosition() - curPos).sqrMagnitude; //initiate with any position
			Vector3 closestVertice = vertices[0].GetPosition();
			foreach (Node vertice in vertices) {
				float thisDistance = (vertice.GetPosition() - curPos).sqrMagnitude;
				if (thisDistance < closestDistance) {
					closestDistance = thisDistance;
					closestVertice = vertice.GetPosition();
				} 
			}
			return closestVertice;
		}
	}
}
	