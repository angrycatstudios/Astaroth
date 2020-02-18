using System.Collections.Generic;

namespace Astaroth.Graph {

public class GraphNode {

	protected readonly int id;
	protected readonly List<LinkData> links;

	public int d;
	public int w;
	public GraphNode p;

	public GraphNode(int id)
	{
		this.id = id;
		links = new List<LinkData>();
	}

	public int GetId()
	{
		return id;
	}

	public List<LinkData> GetLinks()
	{
		return links;
	}

	// Override to carry over additional data between nodes
	public virtual void Propagate(GraphNode parent) { }

	public virtual bool Avoid()
	{
		return false;
	}

	public struct LinkData
	{
		public GraphLink link;
		public int targetId;

		public LinkData(GraphLink link, int targetId)
		{
			this.link = link;
			this.targetId = targetId;
		}
	}

}

}