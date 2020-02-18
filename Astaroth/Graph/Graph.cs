using System.Collections.Generic;

namespace Astaroth.Graph {

public class Graph {

	public List<GraphNode> nodes;
	public List<GraphLink> links;

	public Graph(List<(int, int, int)> linkData)
	{
		foreach ((int, int, int) l in linkData) CreateLink(l);
	}

	public void CreateLink((int, int, int) l)
	{
		GraphNode sourceNode = nodes.Find(n => n.GetId() == l.Item1);
		if (sourceNode == null)
		{
			nodes.Add(new GraphNode(l.Item1));
			sourceNode = nodes[nodes.Count - 1];
		}

		GraphNode targetNode = nodes.Find(n => n.GetId() == l.Item2);
		if (targetNode == null)
		{
			nodes.Add(new GraphNode(l.Item2));
			targetNode = nodes[nodes.Count - 1];
		}

		GraphLink link = new GraphLink(sourceNode, targetNode, l.Item3);
		links.Add(link);

		sourceNode.GetLinks().Add(new GraphNode.LinkData(link, link.GetNodes()[0] == sourceNode ? 1 : 0));
		targetNode.GetLinks().Add(new GraphNode.LinkData(link, link.GetNodes()[1] == sourceNode ? 1 : 0));
	}

}

}