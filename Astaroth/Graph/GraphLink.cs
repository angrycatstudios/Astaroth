namespace Astaroth.Graph {

public class GraphLink {

	public int W { get; set; }

	private readonly GraphNode[] nodes;

	public GraphLink(GraphNode node1, GraphNode node2, int w)
	{
		nodes = new GraphNode[2];
		nodes[0] = node1;
		nodes[1] = node2;
		W = w;
	}

	public GraphNode[] GetNodes()
	{
		return nodes;
	}

}

}