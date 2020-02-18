using System.Collections.Generic;

namespace Astaroth.Graph {

public static class BellmanFord {

	public static List<GraphNode> GetPathToNearest(Graph G, GraphNode startNode, GraphNode[] endNodes)
	{
		// Initialize nodes
		GraphNode currentNode = startNode;
		foreach (GraphNode node in G.nodes) {
			node.d = 0;
			node.w = 0;
			node.p = null;
		}

		// Initialize open stack
		List<GraphNode> openStack = new List<GraphNode>();
		if (currentNode != null) {
			currentNode.p = currentNode;
			openStack.Add(currentNode);
		}

		/// Perform path search
		/// -------------------
		while (openStack.Count > 0) {

		currentNode = openStack[openStack.Count - 1];
		openStack.RemoveAt(openStack.Count - 1);

		foreach (GraphNode.LinkData linkData in currentNode.GetLinks()) {
			GraphNode nextNode = linkData.link.GetNodes()[linkData.targetId];
			int weight = currentNode.w + linkData.link.W;
			if (nextNode.p == null || nextNode.w > weight) {
				nextNode.d = currentNode.d + 1;
				nextNode.w = weight;
				nextNode.p = currentNode;
				nextNode.Propagate(currentNode);
				if (!openStack.Contains(nextNode) && !nextNode.Avoid()) openStack.Add(nextNode);
			}
		}

		}
		/// -------------------

		// Get nearest end node
		GraphNode nearestEnd = null;
		int minWeight = int.MaxValue;
		foreach (GraphNode node in endNodes) {
			if (node.p != null && node.w < minWeight) {
				minWeight = node.w;
				nearestEnd = node;
			}
		}

		// Return path to nearest end node
		if (nearestEnd != null) {
			List<GraphNode> path = new List<GraphNode>() {nearestEnd};
			GraphNode node = nearestEnd;
			while (node.p != node) {
				node = node.p;
				path.Add(node);
			};
			path.Reverse();
			return path;
		}

		// No path was found
		return null;
	}

}

}